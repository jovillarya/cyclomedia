/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2014, CycloMedia, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Client;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Model;
using IntegrationArcMap.Model.Capabilities;
using IntegrationArcMap.Model.Wfs;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ADF.COMSupport;
using stdole;

// ReSharper disable CSharpWarnings::CS0612
// ReSharper disable CSharpWarnings::CS0618
using Converter = ESRI.ArcGIS.ADF.Converter;
// ReSharper restore CSharpWarnings::CS0618
// ReSharper restore CSharpWarnings::CS0612

namespace IntegrationArcMap.Layers
{
  public class WfsLayer : CycloMediaLayer
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private readonly object _getImageLock;
    private readonly Web _web;
    private readonly Dictionary<string, Image> _imageToAdd;

    private string _name;
    private Thread _getImageThread;
    private Thread _refreshDataThread;
    private IFeatureCursor _featureCursor;
    private IFeature _feature;
    private bool _addedImage;
    private double _minimumScale;
    private IMappedFeature _mappedFeature;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public override string[] FieldNames
    {
      get { return new[] {"image_url"}; }
    }

    public override string Name
    {
      get { return _name; }
    }

    public override string FcName
    {
      get { return string.Format("FC{0}", Name.Replace(":", "_")); }
    }

    public override Color Color { get; set; }

    public override double MinimumScale
    {
      get
      {
        const double epsilon = 0.1;

        if (Math.Abs(_minimumScale) < epsilon)
        {
          _minimumScale = 2000.0;

          switch (MinZoomLevel)
          {
            case 0:
              _minimumScale = 24000.0;
              break;
            case 1:
              _minimumScale = 20000.0;
              break;
            case 2:
              _minimumScale = 15000.0;
              break;
            case 3:
              _minimumScale = 12500.0;
              break;
            case 4:
              _minimumScale = 10000.0;
              break;
            case 5:
              _minimumScale = 8000.0;
              break;
            case 6:
              _minimumScale = 6000.0;
              break;
            case 7:
              _minimumScale = 5000.0;
              break;
            case 8:
              _minimumScale = 4000.0;
              break;
            case 9:
              _minimumScale = 3000.0;
              break;
            case 10:
              _minimumScale = 2500.0;
              break;
            case 11:
              _minimumScale = 2000.0;
              break;
            case 12:
              _minimumScale = 1500.0;
              break;
            case 13:
              _minimumScale = 1250.0;
              break;
            case 14:
              _minimumScale = 1000.0;
              break;
            case 15:
              _minimumScale = 800.0;
              break;
          }
        }

        return _minimumScale;
      }

      set { _minimumScale = value; }
    }

    public override int SizeLayer
    {
      get { return 7; }
    }

    public override bool UseDateRange
    {
      get { return false; }
    }

    public override string WfsRequest
    {
      get { return string.Empty; }
    }

    public string TypeName
    {
      get
      {
        string result = string.Empty;

        if (FeatureType != null)
        {
          var name = FeatureType.Name;

          if (name != null)
          {
            result = name.Value;
          }
        }

        return result;
      }
    }

    public string Url { get; private set; }
    public string Version { get; private set; }
    public FeatureType FeatureType { get; private set; }
    public int MinZoomLevel { get; private set; }
    public bool UseProxy { get; private set; }

    #endregion

    #region functions (protected)

    // =========================================================================
    // Functions (Protected)
    // =========================================================================
    protected override IMappedFeature CreateMappedFeature(XElement mappedFeatureElement)
    {
      if (_mappedFeature == null)
      {
        _mappedFeature = new WfsFeature(mappedFeatureElement, FeatureType);
      }
      else
      {
        _mappedFeature.Update(mappedFeatureElement);
      }

      return _mappedFeature;
    }

    protected override bool Filter(IMappedFeature mappedFeature)
    {
      return true;
    }

    protected override void PostEntryStep()
    {
      // ReSharper disable UseIndexedProperty
      // ReSharper disable CSharpWarnings::CS0612
      var geoFeatureLayer = Layer as IGeoFeatureLayer;

      if (geoFeatureLayer != null)
      {
        IFeatureRenderer featureRenderer = geoFeatureLayer.Renderer;
        var uniqueValueRenderer = featureRenderer as IUniqueValueRenderer;
        var displayTable = geoFeatureLayer as IDisplayTable;

        if ((displayTable != null) && (uniqueValueRenderer != null) && (geoFeatureLayer.FeatureClass != null))
        {
          while (_imageToAdd.Count >= 1)
          {
            var element = _imageToAdd.ElementAt(0);
            string classValue = element.Key;
            Image image = element.Value;

            if (!string.IsNullOrEmpty(classValue))
            {
              string label = string.Empty;
              ISymbol markerSymbol = null;

              if (image != null)
              {
                image = MakeTransparant.ApplySrc(image as Bitmap);
                int size = Math.Min(image.Width, image.Height) * 4;
                var imageDst = new Bitmap(size, size);

                using (Graphics graphics = Graphics.FromImage(imageDst))
                {
                  graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                  graphics.SmoothingMode = SmoothingMode.HighQuality;
                  graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                  graphics.CompositingQuality = CompositingQuality.HighQuality;

                  var sRectangle = new Rectangle(0, 0, image.Width, image.Height);
                  var dRectangle = new Rectangle(0, 0, size, size);
                  graphics.DrawImage(image, dRectangle, sRectangle, GraphicsUnit.Pixel);
                }

                imageDst = MakeTransparant.ApplyDst(imageDst);
                // ReSharper disable CSharpWarnings::CS0618

                markerSymbol = new PictureMarkerSymbolClass
                {
                  Size = Math.Min(image.Width, image.Height),
                  BitmapTransparencyColor = Converter.ToRGBColor(Color.White),
                  Picture = OLE.GetIPictureDispFromBitmap(imageDst) as IPictureDisp
                };
              }

              // ReSharper restore CSharpWarnings::CS0618
              if (!string.IsNullOrEmpty(classValue))
              {
                string[] splitSign = classValue.Split('/');

                if (splitSign.Length >= 1)
                {
                  string sign = splitSign[splitSign.Length - 1];
                  string[] splitLabel = sign.Split('.');

                  if (splitLabel.Length >= 1)
                  {
                    label = splitLabel[0];
                  }
                }
              }

              if (markerSymbol == null)
              {
                markerSymbol = uniqueValueRenderer.DefaultSymbol;
              }

              uniqueValueRenderer.AddValue(classValue, FieldNames[0], markerSymbol);
              uniqueValueRenderer.set_Label(classValue, label);
              uniqueValueRenderer.set_Symbol(classValue, markerSymbol);
              _addedImage = true;
            }

            _imageToAdd.Remove(classValue);
          }

          if ((_featureCursor == null) || (_feature == null))
          {
            _featureCursor = displayTable.SearchDisplayTable(null, false) as IFeatureCursor;
          }

          if (_featureCursor != null)
          {
            _feature = _featureCursor.NextFeature();
            IFields fields = _featureCursor.Fields;
            int fieldIndex = fields.FindField(FieldNames[0]);

            while ((_feature != null) && ((_getImageThread == null) || (!_getImageThread.IsAlive)))
            {
              // Test to see if this value was added
              // to the renderer. If not, add it.
              var classValue = _feature.get_Value(fieldIndex) as string;
              bool valFound = false;

              for (int i = 0; i <= uniqueValueRenderer.ValueCount - 1; i++)
              {
                if (uniqueValueRenderer.get_Value(i) == classValue)
                {
                  // Exit the loop if the value was found.
                  valFound = true;
                  break;
                }
              }

              // If the value was not found, it is new and it will be added.
              if (!valFound)
              {
                _getImageThread = new Thread(GetImage);
                _getImageThread.Start(classValue);
              }
              else
              {
                _feature = _featureCursor.NextFeature();
              }
            }

            geoFeatureLayer.Renderer = uniqueValueRenderer as IFeatureRenderer;
          }

          if ((_feature == null) && _addedImage)
          {
            IActiveView activeView = ArcUtils.ActiveView;

            if (activeView != null)
            {
              activeView.ContentsChanged();
              _addedImage = false;
            }
          }
        }
      }

      // ReSharper restore CSharpWarnings::CS0612
      // ReSharper restore UseIndexedProperty
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public override string ToString()
    {
      Name name = FeatureType.Name;
      return name.Value;
    }

    public void AddToGlobespotter(string name, int minZoomLevel, bool useProxy, Color color)
    {
      _name = name;
      MinZoomLevel = minZoomLevel;
      UseProxy = useProxy;
      Color = color;
      FrmGlobespotter.AddWfsLayer(this);
      AddToLayers();
    }

    public bool ContainsSrsName(string srsName)
    {
      return FeatureType.OtherSrs.Aggregate((FeatureType.DefaultSrs == srsName),
                                            (current, otherSrs) => (otherSrs == srsName) || current);
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    private WfsLayer(CycloMediaGroupLayer layer)
      : base(layer)
    {
      _web = Web.Instance;
      _imageToAdd = new Dictionary<string, Image>();
      _getImageLock = new object();
      _getImageThread = null;
      _refreshDataThread = null;
      _addedImage = false;
      _minimumScale = 0.0;
      _mappedFeature = null;
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static IList<WfsLayer> GetCapabilities(string url, string version)
    {
      if (url.Substring(url.Length - 1) == "?")
      {
        url = url.Substring(0, url.Length - 1);
      }

      Web web = Web.Instance;
      List<XElement> elements = web.GetCapabilities(url, version);
      GsExtension extension = GsExtension.GetExtension();
      CycloMediaGroupLayer cycloMediaGroupLayer = (extension == null) ? null : extension.CycloMediaGroupLayer;

      return (from element in elements
              from featureType in element.Descendants(FeatureType.TypeName)
              where featureType != null
              select new WfsLayer(cycloMediaGroupLayer)
                {
                  Url = url,
                  Version = version,
                  FeatureType = new FeatureType(featureType)
                }).ToList();
    }

    #endregion

    #region thread functions

    // =========================================================================
    // Thread functions
    // =========================================================================
    private void GetImage(object content)
    {
      lock (_getImageLock)
      {
        var classValue = content as string;

        if (!string.IsNullOrEmpty(classValue))
        {
          Image image = null;

          try
          {
            image = _web.DownloadImage(classValue);
          }
          catch (Exception ex)
          {
            Trace.WriteLine(ex.Message, "WfsLayer.GetImage");
          }

          _imageToAdd.Add(classValue, image);
          _refreshDataThread = new Thread(RefreshData);
          _refreshDataThread.Start();
        }
      }
    }

    private void RefreshData()
    {
      try
      {
        lock (_getImageLock)
        {
          IActiveView activeView = ArcUtils.ActiveView;

          if (activeView != null)
          {
            activeView.Refresh();
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "WfsLayer.RefreshData");
      }
    }

    #endregion
  }
}
