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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using IntegrationArcMap.Client;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Model;
using IntegrationArcMap.Model.Atlas;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace IntegrationArcMap.Layers
{
  public class HistoricalLayer : CycloMediaLayer
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static Color _color;
    private static double _minimumScale;
    private static SortedDictionary<int, Color> _yearToColor;
    private static List<int> _yearPip;
    private static List<int> _yearForbidden; 

    public override string Name { get { return "Historical Recordings"; } }
    public override string FcName { get { return "FCHistoricalRecordings"; } }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    private static SortedDictionary<int, Color> YearToColor
    {
      get { return _yearToColor ?? (_yearToColor = new SortedDictionary<int, Color>()); }
    }

    private static List<int> YearPip
    {
      get { return _yearPip ?? (_yearPip = new List<int>()); }
    }

    private static List<int> YearForbidden
    {
      get { return _yearForbidden ?? (_yearForbidden = new List<int>()); }
    }

    public override Color Color
    {
      get { return _color; }
      set { _color = value; }
    }

    public override double MinimumScale
    {
      get { return _minimumScale; }
      set { _minimumScale = value; }
    }

    public override string[] FieldNames
    {
      get { return new[] {"Year", "PIP", "IsAuthorized"}; }
    }

    public override int SizeLayer { get { return 7; } }
    public override bool UseDateRange { get { return true; } }

    public override string WfsRequest
    {
      get
      {
        return
          "<wfs:GetFeature service=\"WFS\" version=\"1.1.0\" resultType=\"results\" outputFormat=\"text/xml; subtype=gml/3.1.1\" xmlns:wfs=\"http://www.opengis.net/wfs\">" +
          "<wfs:Query typeName=\"atlas:Recording\" srsName=\"{0}\" xmlns:atlas=\"http://www.cyclomedia.com/atlas\"><ogc:Filter xmlns:ogc=\"http://www.opengis.net/ogc\">" +
          "<ogc:And><ogc:BBOX><gml:Envelope srsName=\"{0}\" xmlns:gml=\"http://www.opengis.net/gml\"><gml:lowerCorner>{1} {2}</gml:lowerCorner>" +
          "<gml:upperCorner>{3} {4}</gml:upperCorner></gml:Envelope></ogc:BBOX><ogc:PropertyIsBetween><ogc:PropertyName>recordedAt</ogc:PropertyName><ogc:LowerBoundary>" +
          "<ogc:Literal>1991-12-31T23:00:00-00:00</ogc:Literal></ogc:LowerBoundary><ogc:UpperBoundary><ogc:Literal>{5}</ogc:Literal></ogc:UpperBoundary></ogc:PropertyIsBetween>" +
          "</ogc:And></ogc:Filter></wfs:Query></wfs:GetFeature>";
      }
    }

    #endregion

    #region functions (protected)

    // =========================================================================
    // Functions (Protected)
    // =========================================================================
    protected override IMappedFeature CreateMappedFeature(XElement mappedFeatureElement)
    {
      return new Recording(mappedFeatureElement);
    }

    protected override bool Filter(IMappedFeature mappedFeature)
    {
      var recording = mappedFeature as Recording;
      bool result = (recording != null);

      if (result)
      {
        DateTime? recordedAt = recording.RecordedAt;
        result = (recordedAt != null);

        if (result)
        {
          var dateTime = (DateTime) recordedAt;
          int year = dateTime.Year;
          int month = dateTime.Month;
          result = YearInsideRange(year, month);

          if (!YearMonth.ContainsKey(year))
          {
            YearMonth.Add(year, month);
            ChangeHistoricalDate();
          }
          else
          {
            YearMonth[year] = month;
          }
        }
      }

      return result;
    }

    protected override void PostEntryStep()
    {
      const string objectId = "RecordedAt";
      const string object2Id = "PIP";
      const string object3Id = "IsAuthorized";
      IActiveView activeView = ArcUtils.ActiveView;
      IEnvelope envelope = activeView.Extent;

      ISpatialFilter spatialFilter = new SpatialFilterClass
        {
          Geometry = envelope,
          GeometryField = FeatureClass.ShapeFieldName,
          SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
          SubFields = string.Format("{0},{1},{2}", objectId, object2Id, object3Id)
        };

      var existsResult = FeatureClass.Search(spatialFilter, false);
      IFeature feature;
      var added = new List<int>();
      var pipAdded = new List<int>();
      var forbiddenAdded = new List<int>();

      while ((feature = existsResult.NextFeature()) != null)
      {
        // ReSharper disable UseIndexedProperty
        int imId = existsResult.FindField(objectId);
        object value = feature.get_Value(imId);
        var dateTime = (DateTime) value;
        int year = dateTime.Year;
        int month = dateTime.Month;
        int calcYear = (year*4) + (int) Math.Floor(((double) (month - 1))/3);

        if (!YearToColor.ContainsKey(calcYear))
        {
          YearToColor.Add(calcYear, Color.Transparent);
          added.Add(calcYear);
        }

        int pipId = existsResult.FindField(object2Id);
        object pipValue = feature.get_Value(pipId);

        if (pipValue != null)
        {
          bool pip = bool.Parse((string) pipValue);

          if (pip && (!YearPip.Contains(calcYear)))
          {
            YearPip.Add(calcYear);
            pipAdded.Add(calcYear);
          }
        }

        int forbiddenId = existsResult.FindField(object3Id);
        object forbiddenValue = feature.get_Value(forbiddenId);
        // ReSharper restore UseIndexedProperty

        if (forbiddenValue != null)
        {
          bool forbidden = !bool.Parse((string)forbiddenValue);

          if (forbidden && (!YearForbidden.Contains(calcYear)))
          {
            YearForbidden.Add(calcYear);
            forbiddenAdded.Add(calcYear);
          }
        }
      }

      var geoFeatureLayer = Layer as IGeoFeatureLayer;

      if (geoFeatureLayer != null)
      {
        IFeatureRenderer featureRenderer = geoFeatureLayer.Renderer;
        var uniqueValueRenderer = featureRenderer as IUniqueValueRenderer;

        if (uniqueValueRenderer != null)
        {
          foreach (var value in added)
          {
            bool realAdd = true;
            var newValue = (int) Math.Floor(((double) value)/4);

            for (int i = newValue; i < (newValue + 4); i++)
            {
              realAdd = (!YearToColor.ContainsKey(i)) && realAdd;
            }

            if (realAdd)
            {
              // ReSharper disable CSharpWarnings::CS0612
              // ReSharper disable CSharpWarnings::CS0618

              var symbol = new SimpleMarkerSymbol
              {
                Color = Converter.ToRGBColor(Color.Transparent),
                Size = SizeLayer
              };

              // ReSharper restore CSharpWarnings::CS0618
              // ReSharper restore CSharpWarnings::CS0612
              var markerSymbol = symbol as ISymbol;
              string classValue = string.Format("{0}, {1}, {2}", newValue, false, true);
              uniqueValueRenderer.AddValue(classValue, string.Empty, markerSymbol);

              // ReSharper disable UseIndexedProperty
              string label = newValue.ToString(CultureInfo.InvariantCulture);
              uniqueValueRenderer.set_Label(classValue, label);
              // ReSharper restore UseIndexedProperty
            }
          }

          foreach (var value in pipAdded)
          {
            bool realAdd = true;
            var newValue = (int) Math.Floor(((double) value)/4);

            for (int i = newValue; i < (newValue + 4); i++)
            {
              realAdd = (!YearToColor.ContainsKey(i)) && realAdd;
            }

            if (realAdd)
            {
              var rotationRenderer = uniqueValueRenderer as IRotationRenderer;

              if (rotationRenderer != null)
              {
                rotationRenderer.RotationField = "PIP1Yaw";
                rotationRenderer.RotationType = esriSymbolRotationType.esriRotateSymbolGeographic;
              }

              Color color = YearToColor.ContainsKey(value) ? YearToColor[value] : Color.Transparent;
              ISymbol symbol = ArcUtils.GetPipSymbol(SizeLayer, color);
              string classValue = string.Format("{0}, {1}, {2}", newValue, true, true);
              uniqueValueRenderer.AddValue(classValue, string.Empty, symbol);

              // ReSharper disable UseIndexedProperty
              string label = string.Format("{0} (Detail images)", newValue);
              uniqueValueRenderer.set_Label(classValue, label);
              // ReSharper restore UseIndexedProperty
              activeView.ContentsChanged();
            }
          }

          foreach (var value in forbiddenAdded)
          {
            bool realAdd = true;
            var newValue = (int) Math.Floor(((double) value)/4);

            for (int i = newValue; i < (newValue + 4); i++)
            {
              realAdd = (!YearToColor.ContainsKey(i)) && realAdd;
            }

            if (realAdd)
            {
              Color color = YearToColor.ContainsKey(value) ? YearToColor[value] : Color.Transparent;
              ISymbol symbol = ArcUtils.GetForbiddenSymbol(SizeLayer, color);
              string classValue = string.Format("{0}, {1}, {2}", newValue, false, false);
              uniqueValueRenderer.AddValue(classValue, string.Empty, symbol);

              // ReSharper disable UseIndexedProperty
              string label = string.Format("{0} (No Authorization)", newValue);
              uniqueValueRenderer.set_Label(classValue, label);
              // ReSharper restore UseIndexedProperty

              if (pipAdded.Contains(value))
              {
                classValue = string.Format("{0}, {1}, {2}", newValue, true, false);
                uniqueValueRenderer.AddValue(classValue, string.Empty, symbol);

                // ReSharper disable UseIndexedProperty
                label = string.Format("{0} (Detail images, No Authorization)", newValue);
                uniqueValueRenderer.set_Label(classValue, label);
                // ReSharper restore UseIndexedProperty
              }

              activeView.ContentsChanged();
            }
          }

          var removed = (from yearColor in YearToColor
                         select yearColor.Key
                         into year
                         where ((!YearInsideRange((int)Math.Floor(((double)year) / 4), (((year % 4) * 3) + 1))) && (!added.Contains(year)))
                         select year).ToList();

          foreach (var year in removed)
          {
            var newYear = (int) Math.Floor(((double) year)/4);

            if (YearPip.Contains(year))
            {
              string classValuePip = string.Format("{0}, {1}, {2}", newYear, true, true);
              uniqueValueRenderer.RemoveValue(classValuePip);
              YearPip.Remove(year);
            }

            string classValue = string.Format("{0}, {1}, {2}", newYear, false, true);
            uniqueValueRenderer.RemoveValue(classValue);
            YearToColor.Remove(year);
          }
        }
      }

      foreach (var value in added)
      {
        var newValue = (int) Math.Floor(((double) value)/4);
        FrmGlobespotter.UpdateColor(this, newValue);
      }
    }

    protected override void Remove()
    {
      base.Remove();
      YearToColor.Clear();
      YearPip.Clear();
      YearForbidden.Clear();
    }

    public override void UpdateColor(Color color, int? year)
    {
      if (year != null)
      {
        var doYear = (int) year;
        int calcYear = doYear*4;
        bool update = false;

        for (int i = calcYear; i < (calcYear + 4); i++)
        {
          update = (YearToColor.ContainsKey(i)) || update;
        }

        if (update)
        {
          string classValue = string.Format("{0}, {1}, {2}", doYear, false, true);

          for (int j = calcYear; j < calcYear + 4; j++)
          {
            if (YearToColor.ContainsKey(j))
            {
              YearToColor[j] = color;
            }
          }

          ArcUtils.SetColorToLayer(Layer, color, classValue);

          if (YearPip.Contains(calcYear))
          {
            classValue = string.Format("{0}, {1}, {2}", doYear, true, true);
            ISymbol symbol = ArcUtils.GetPipSymbol(SizeLayer, color);
            ArcUtils.SetSymbolToLayer(Layer, symbol, classValue);
          }

          if (YearForbidden.Contains(calcYear))
          {
            classValue = string.Format("{0}, {1}, {2}", doYear, false, false);
            ISymbol symbol = ArcUtils.GetForbiddenSymbol(SizeLayer, color);
            ArcUtils.SetSymbolToLayer(Layer, symbol, classValue);

            if (YearPip.Contains(calcYear))
            {
              classValue = string.Format("{0}, {1}, {2}", doYear, true, false);
              ArcUtils.SetSymbolToLayer(Layer, symbol, classValue);
            }
          }

          Refresh();
        }
      }

    }

    #endregion

    #region functions (private)

    // =========================================================================
    // Functions (Private)
    // =========================================================================
    private bool YearInsideRange(int year, int month)
    {
      Config config = Config.Instance;
      var fromDateTime = new DateTime(config.YearFrom, config.MonthFrom, 1);
      var toDateTime = new DateTime(config.YearTo, config.MonthTo, 1);
      var checkDateTime = new DateTime(year, month, 1);
      return (checkDateTime.CompareTo(fromDateTime) >= 0) && (checkDateTime.CompareTo(toDateTime) < 0);
      // return (year >= config.YearFrom) && (year < config.YearTo);
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static HistoricalLayer()
    {
      _color = Color.Transparent;
      _minimumScale = 2000.0;
    }

    public HistoricalLayer(CycloMediaGroupLayer layer)
      : base(layer)
    {
      // empty
    }

    #endregion
  }
}
