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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using IntegrationArcMap.Client;
using Path = System.IO.Path;
using Point = ESRI.ArcGIS.Geometry.Point;

namespace IntegrationArcMap.Utilities
{
  class ArcUtils
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static IActiveView _activeView;
    private static IMxDocument _mxDocument;
    private static IMap _map;
    private static IEditor3 _editor;
    private static IEditTool _editTool;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public static string FileDir
    {
      get
      {
        string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string result = Path.Combine(folder, "IntegrationArcMap");

        if (!Directory.Exists(result))
        {
          Directory.CreateDirectory(result);
        }

        return result;
      }
    }

    public static IMxDocument MxDocument
    {
      get { return _mxDocument ?? (_mxDocument = GetDocument()); }
    }

    public static IActiveViewEvents_Event ActiveViewEventsPageLayout
    {
      get { return MxDocument.PageLayout as IActiveViewEvents_Event; }
    }

    public static IDocumentEvents_Event MxDocumentEvents
    {
      get { return MxDocument as IDocumentEvents_Event; }
    }

    public static IActiveViewEvents_Event ActiveViewEvents
    {
      get { return ActiveView as IActiveViewEvents_Event; }
    }

    public static IActiveView ActiveView
    {
      get { return _activeView ?? ((MxDocument == null) ? null : (_activeView = MxDocument.ActiveView)); }
    }

    public static IMap Map
    {
      get { return _map ?? ((ActiveView == null) ? null : (_map = ActiveView.FocusMap)); }
    }

    public static ISpatialReference SpatialReference
    {
      get { return (Map == null) ? null : Map.SpatialReference; }
    }

    public static string EpsgCode
    {
      get
      {
        Config config = Config.Instance;
        string defaultRecordingSrs = config.DefaultRecordingSrs;
        int factoryCode;

        if ((string.IsNullOrEmpty(defaultRecordingSrs)) || (!int.TryParse(defaultRecordingSrs, out factoryCode)))
        {
          factoryCode = (SpatialReference == null) ? 0 : SpatialReference.FactoryCode;
        }

        return string.Format("EPSG:{0}", factoryCode);
      }
    }

    public static IEditor3 Editor
    {
      get
      {
        if (_editor == null)
        {
          //Get the editor.
          var editorUid = new UID {Value = "esriEditor.Editor"};
          IApplication application = ArcMap.Application;
          _editor = application.FindExtensionByCLSID(editorUid) as IEditor3;
        }

        return _editor;
      }
    }

    public static IEditTool EditTool
    {
      get
      {
        if (_editTool == null)
        {
          //Get the editor.
          IApplication application = ArcMap.Application;
          IDocument document = application.Document;
          ICommandBars commandBars = document.CommandBars;
          ICommandItem commandItem = commandBars.Find("Editor_EditTool", true, true);
          _editTool = (commandItem == null) ? null : (commandItem.Command as IEditTool);
        }

        return _editTool;
      }
    }

    public static IEditToolEvents_Event EditToolEvents
    {
      get { return (EditTool == null) ? null : (EditTool.EventSource as IEditToolEvents_Event); }
    }

    public static IEditEvents_Event EditorEvents
    {
      get { return Editor as IEditEvents_Event; }
    }

    public static IEditEvents2_Event EditorEvents2
    {
      get { return Editor as IEditEvents2_Event; }
    }

    public static IEditEvents3_Event EditorEvents3
    {
      get { return Editor as IEditEvents3_Event; }
    }

    public static IEditEvents5_Event EditorEvents5
    {
      get { return Editor as IEditEvents5_Event; }
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public static void SetZOffset()
    {
      var sketch = Editor as IEditSketch3;
      var editorZ = Editor as IEditorZ;

      if ((editorZ != null) && (sketch != null))
      {
        if (sketch.ZAware)
        {
          editorZ.UseZOffset = true;
        }
      }
    }

    public static IDockableWindow GetDocWindow(string name)
    {
      IApplication application = ArcMap.Application;
      application.CurrentTool = null;
      var dockWindowManager = application as IDockableWindowManager;
      IDockableWindow result = null;

      if (dockWindowManager != null)
      {
        UID windowId = new UIDClass {Value = name};
        result = dockWindowManager.GetDockableWindow(windowId);
      }

      return result;
    }

    public static void SetToolActiveInToolBar(string name)
    {
      IApplication application = ArcMap.Application;
      IDocument document = application.Document;
      ICommandBars commandBars = document.CommandBars;

      UID commandId = new UIDClass {Value = name};
      ICommandItem commandItem = commandBars.Find(commandId);

      if (commandItem != null)
      {
        application.CurrentTool = commandItem;
      }
    }

    public static void AddCommandItem(string menu, string command)
    {
      AddCommandItem(menu, command, Type.Missing);
    }

    public static void AddCommandItem(string menu, string command, object index)
    {
      IApplication application = ArcMap.Application;
      IDocument document = application.Document;
      ICommandBars commandBars = document.CommandBars;

      var menuUid = new UID {Value = menu};
      var contextMenu = commandBars.Find(menuUid) as ICommandBar;

      if (contextMenu != null)
      {
        var commandUid = new UID {Value = command};
        ICommandItem commandItem = contextMenu.Find(commandUid);

        if (commandItem == null)
        {
          contextMenu.Add(commandUid, ref index);
        }
      }
    }

    public static void RemoveCommandItem(string menu, string command)
    {
      IApplication application = ArcMap.Application;
      IDocument document = application.Document;
      ICommandBars commandBars = document.CommandBars;

      var menuUid = new UID { Value = menu };
      var contextMenu = commandBars.Find(menuUid) as ICommandBar;

      if (contextMenu != null)
      {
        var commandUid = new UID { Value = command };
        ICommandItem commandItem = contextMenu.Find(commandUid);

        if (commandItem != null)
        {
          commandItem.Delete();
        }
      }
    }

    public static Color GetColorFromLayer(ILayer layer)
    {
      Color result = Color.White;

      if (layer != null)
      {
        var geoFeatureLayer = layer as IGeoFeatureLayer;

        if (geoFeatureLayer != null)
        {
          RgbColor rgbColor = null;
          var simpleRenderer = geoFeatureLayer.Renderer as ISimpleRenderer;
          var uniqueValueRenderer = geoFeatureLayer.Renderer as IUniqueValueRenderer;

          if (simpleRenderer != null)
          {
            rgbColor = GetColorFromSymbol(simpleRenderer.Symbol);
          }

          if (uniqueValueRenderer != null)
          {
            rgbColor = GetColorFromSymbol(uniqueValueRenderer.DefaultSymbol);
          }

          if (rgbColor != null)
          {
            // ReSharper disable CSharpWarnings::CS0612
            // ReSharper disable CSharpWarnings::CS0618
            result = Converter.FromRGBColor(rgbColor);
            // ReSharper restore CSharpWarnings::CS0618
            // ReSharper restore CSharpWarnings::CS0612
          }
        }
      }

      return result;
    }

    public static ISymbol GetPipSymbol(int sizeLayer, Color color)
    {
      var size025 = sizeLayer;
      var size05 = size025 * 2;
      var size075 = size025 * 3;
      var size = size025 * 4;
      var size15 = size025 * 6;
      var size25 = size025 * 10;
      var size275 = size025 * 11;
      var size3 = size025 * 12;
      var bitmap = new Bitmap(size3, size3);
      const int sizeLine = 2;
      color = Color.FromArgb(255, color);

      using (Graphics ga = Graphics.FromImage(bitmap))
      {
        var points = new PointF[6];
        points[0] = new PointF(size05, size025);
        points[1] = new PointF(size25, size025);
        points[2] = new PointF(size15, size15);
        points[3] = new PointF(size25, size275);
        points[4] = new PointF(size05, size275);
        points[5] = new PointF(size15, size15);
        var pathd = new GraphicsPath();
        pathd.AddPolygon(points);
        ga.Clear(Color.White);
        ga.FillPath(Brushes.Yellow, pathd);
        ga.DrawPath(new Pen(Brushes.Gray, sizeLine), pathd);
        ga.DrawEllipse(new Pen(color, sizeLine), size, size, size, size);
        ga.FillEllipse(new SolidBrush(color), size, size, size, size);
        pathd.Dispose();
      }

      // ReSharper disable CSharpWarnings::CS0612
      // ReSharper disable CSharpWarnings::CS0618
      Bitmap bitmap8B = bitmap.To8BppIndexed();
      string tempPath = Path.GetTempPath();
      string writePath = Path.Combine(tempPath, string.Format("{0}pip.bmp", color.Name));
      bitmap8B.Save(writePath, ImageFormat.Bmp);
      IPictureMarkerSymbol symbol = new PictureMarkerSymbolClass();
      symbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureBitmap, writePath);
      symbol.Size = size075;
      symbol.BitmapTransparencyColor = Converter.ToRGBColor(Color.White);
      return symbol as ISymbol;
      // ReSharper restore CSharpWarnings::CS0618
      // ReSharper restore CSharpWarnings::CS0612
    }

    public static ISymbol GetForbiddenSymbol(int sizeLayer, Color color)
    {
      var size025 = sizeLayer;
      var size075 = size025*3;
      var size = size025*4;
      var size15 = size025*6;
      var size175 = size025*7;
      var size3 = size025*12;
      var bitmap = new Bitmap(size3, size3);
      const int sizeLine2 = 2;
      const int sizeLine3 = 3;
      const int sizeLine6 = 6;
      color = Color.FromArgb(255, color);

      using (Graphics ga = Graphics.FromImage(bitmap))
      {
        ga.Clear(Color.White);
        ga.DrawEllipse(new Pen(color, sizeLine2), size, size, size, size);
        ga.FillEllipse(new SolidBrush(color), size, size, size, size);
        ga.DrawEllipse(new Pen(Color.Red, sizeLine2), size15, size15, size075, size075);
        ga.FillEllipse(Brushes.Red, size15, size15, size075, size075);
        ga.DrawRectangle(new Pen(Color.WhiteSmoke, sizeLine2), size15 + sizeLine3, size175, size075 - sizeLine6, size025);
        ga.FillRectangle(Brushes.WhiteSmoke, size15 + sizeLine3, size175, size075 - sizeLine6, size025);
      }

      // ReSharper disable CSharpWarnings::CS0612
      // ReSharper disable CSharpWarnings::CS0618
      Bitmap bitmap8B = bitmap.To8BppIndexed();
      string tempPath = Path.GetTempPath();
      string writePath = Path.Combine(tempPath, string.Format("{0}forbidden.bmp", color.Name));
      bitmap8B.Save(writePath, ImageFormat.Bmp);
      IPictureMarkerSymbol symbol = new PictureMarkerSymbolClass();
      symbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureBitmap, writePath);
      symbol.Size = size075;
      symbol.BitmapTransparencyColor = Converter.ToRGBColor(Color.White);
      return symbol as ISymbol;
      // ReSharper restore CSharpWarnings::CS0618
      // ReSharper restore CSharpWarnings::CS0612
    }

    private static RgbColor GetColorFromSymbol(ISymbol symbol)
    {
      RgbColor rgbColor = null;

      if (symbol is ISimpleFillSymbol)
      {
        rgbColor = (symbol as ISimpleFillSymbol).Color as RgbColor;
      }

      if (symbol is IMultiLayerFillSymbol)
      {
        rgbColor = (symbol as IMultiLayerFillSymbol).Color as RgbColor;
      }

      if (symbol is ILineSymbol)
      {
        rgbColor = (symbol as ILineSymbol).Color as RgbColor;
      }

      if (symbol is ISimpleMarkerSymbol)
      {
        rgbColor = (symbol as ISimpleMarkerSymbol).Color as RgbColor;
      }

      return rgbColor;
    }

    public static void SetColorToLayer(ILayer layer, Color color, string classValue = null)
    {
      if (layer != null)
      {
        var geoFeatureLayer = layer as IGeoFeatureLayer;

        if (geoFeatureLayer != null)
        {
          var simpleRenderer = geoFeatureLayer.Renderer as ISimpleRenderer;
          var uniqueValueRenderer = geoFeatureLayer.Renderer as IUniqueValueRenderer;

          if (simpleRenderer != null)
          {
            SetColorToSymbol(simpleRenderer.Symbol, color);
          }

          if (uniqueValueRenderer != null)
          {
            if (classValue == null)
            {
              SetColorToSymbol(uniqueValueRenderer.DefaultSymbol, color);
            }
            else
            {
              // ReSharper disable UseIndexedProperty
              ISymbol symbol = uniqueValueRenderer.get_Symbol(classValue);
              SetColorToSymbol(symbol, color);
              // ReSharper restore UseIndexedProperty
            }
          }
        }
      }
    }

    public static void SetSymbolToLayer(ILayer layer, ISymbol symbol, string classValue)
    {
      if (layer != null)
      {
        var geoFeatureLayer = layer as IGeoFeatureLayer;

        if (geoFeatureLayer != null)
        {
          var uniqueValueRenderer = geoFeatureLayer.Renderer as IUniqueValueRenderer;

          if (uniqueValueRenderer != null)
          {
            // ReSharper disable UseIndexedProperty
            uniqueValueRenderer.set_Symbol(classValue, symbol);
            // ReSharper restore UseIndexedProperty
          }
        }
      }
    }

    public static IPoint GsToMapPoint(double x, double y, double z)
    {
      Config config = Config.Instance;
      SpatialReference spatRel = config.SpatialReference;
      ISpatialReference gsSpatialReference = (spatRel == null) ? SpatialReference : spatRel.SpatialRef;
      IPoint point = new Point { X = x, Y = y, Z = z, SpatialReference = gsSpatialReference };
      point.Project(SpatialReference);
      return point;
    }

    #endregion

    #region functions (private)

    // =========================================================================
    // Functions (Private)
    // =========================================================================
    private static void SetColorToSymbol(ISymbol symbol, Color color)
    {
      // ReSharper disable CSharpWarnings::CS0612
      // ReSharper disable CSharpWarnings::CS0618
      IRgbColor rgbColor = Converter.ToRGBColor(color);
      // ReSharper restore CSharpWarnings::CS0618
      // ReSharper restore CSharpWarnings::CS0612

      if (symbol is ISimpleFillSymbol)
      {
        (symbol as ISimpleFillSymbol).Color = rgbColor;
      }

      if (symbol is IMultiLayerFillSymbol)
      {
        (symbol as IMultiLayerFillSymbol).Color = rgbColor;
      }

      if (symbol is ILineSymbol)
      {
        (symbol as ILineSymbol).Color = rgbColor;
      }

      if (symbol is ISimpleMarkerSymbol)
      {
        (symbol as ISimpleMarkerSymbol).Color = rgbColor;
      }
    }

    private static IMxDocument GetDocument()
    {
      IApplication application = ArcMap.Application;

      if (application != null)
      {
        _mxDocument = application.Document as IMxDocument;

        if (_mxDocument != null)
        {
          var docEvents = _mxDocument as IDocumentEvents_Event;

          if (docEvents != null)
          {
            docEvents.ActiveViewChanged += OnActiveViewChanged;
          }
        }
      }

      return _mxDocument;
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Event handlers
    // =========================================================================
    private static void OnActiveViewChanged()
    {
      try
      {
        var docEvents = MxDocumentEvents;

        if (docEvents != null)
        {
          docEvents.ActiveViewChanged -= OnActiveViewChanged;
        }

        _activeView = null;
        _mxDocument = null;
        _map = null;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "ArcUtils.OnActiveViewChanged");
      }
    }

    #endregion
  }
}
