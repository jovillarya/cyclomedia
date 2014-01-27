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
using System.Globalization;
using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Utilities;
using GlobeSpotterAPI;
using stdole;
using MeasurementPointS = GlobeSpotterAPI.MeasurementPoint;

namespace IntegrationArcMap.Objects
{
  class MeasurementPoint: IDisposable
  {
    #region constants

    // =========================================================================
    // Constants
    // =========================================================================
    private const double PointSize = 5.0;
    private const double FontSize = 8;

    #endregion

    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static readonly Dictionary<string, Color> ObsColor;

    private readonly Measurement _measurement;
    private readonly Dictionary<string, double[]> _observations;

    private IEnvelope _envelope;
    private IEnvelope _oldEnvelope;
    private IPoint _point;
    private int _index;
    private int _intId;
    private bool _added;
    private bool _open;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public int PointId { get; private set; }

    public int M
    {
      get { return (_index != 0) ? _index : (_index = _measurement.GetMeasurementPointIndex(PointId)); }
    }

    public double Z
    {
      get { return (_point == null) ? 0.0 : (_point.Z); }
    }

    public IPoint Point
    {
      get { return _point; }
    }

    public bool NotCreated
    {
      get { return (_point == null); }
    }

    public bool IsFirstNumber
    {
      get { return _intId == 1; }
    }

    public bool IsLastNumber
    {
      get { return ((_measurement == null) || (_intId == _measurement.Count)); }
    }

    public MeasurementPoint PreviousPoint
    {
      get { return ((_measurement != null) && (!IsFirstNumber)) ? _measurement.GetPointByNr(_intId - 2) : null; }
    }

    public MeasurementPoint NextPoint
    {
      get { return ((_measurement != null) && (!IsLastNumber)) ? _measurement.GetPointByNr(_intId) : null; }
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static MeasurementPoint()
    {
      ObsColor = new Dictionary<string, Color>();
    }

    public MeasurementPoint(int pointId, int intId, Measurement measurement)
    {
      _measurement = measurement;
      _envelope = null;
      _oldEnvelope = null;
      _index = 0;
      _intId = intId;
      _point = null;
      PointId = pointId;
      _added = false;
      _open = false;
      _observations = new Dictionary<string, double[]>();
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static void UpdateObsColor(string imageId, Color color)
    {
      if (ObsColor.ContainsKey(imageId))
      {
        ObsColor[imageId] = color;
      }
      else
      {
        ObsColor.Add(imageId, color);
      }

      Measurement.Update();
    }

    public static void RemoveObsColor(string imageId)
    {
      if (ObsColor.ContainsKey(imageId))
      {
        ObsColor.Remove(imageId);
      }

      Measurement.Update();
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public void UpdateObservation(string imageId, double x, double y, double z)
    {
      IPoint point = ArcUtils.GsToMapPoint(x, y, z);

      if (_observations.ContainsKey(imageId))
      {
        _observations[imageId] = new[] { point.X, point.Y, point.Z };
      }
      else
      {
        _observations.Add(imageId, new[] { point.X, point.Y, point.Z });
      }
    }

    public void RemoveObservation(string imageId)
    {
      if (_observations.ContainsKey(imageId))
      {
        _observations.Remove(imageId);
      }
    }

    public bool CheckSelected()
    {
      IEditor3 editor = ArcUtils.Editor;
      var sketch = editor as IEditSketch3;
      bool result = false;

      if (sketch != null)
      {
        result = sketch.IsVertexSelected(0, (_intId - 1));
      }

      return result;
    }

    public void Closed()
    {
      _open = false;
    }

    public void Opened()
    {
      _open = true;
    }

    public void OpenPoint()
    {
      if (_measurement != null)
      {
        _open = true;
        _measurement.OpenPoint(PointId);
      }
    }

    public void ClosePoint()
    {
      if ((_measurement != null) && _open)
      {
        _open = false;
        _measurement.ClosePoint(PointId);
      }
    }

    public void Dispose()
    {
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.AfterDraw -= AvEventsAfterDraw;
      }

      Update();
    }

    public void SetIntId(int intId)
    {
      _intId = intId;
    }

    public void UpdatePoint(PointMeasurementData measurementData, int index)
    {
      _index = index;
      bool notCreated = NotCreated;
      MeasurementPointS measurementPoint = measurementData.measurementPoint;
      double x = measurementPoint.x;
      double y = measurementPoint.y;
      double z = measurementPoint.z;
      _point = ArcUtils.GsToMapPoint(x, y, z);

      IActiveView activeView = ArcUtils.ActiveView;
      var display = activeView.ScreenDisplay;
      IDisplayTransformation dispTrans = display.DisplayTransformation;
      double size = dispTrans.FromPoints(PointSize);
      double xmin = x - size;
      double xmax = x + size;
      double ymin = y - size;
      double ymax = y + size;
      _oldEnvelope = _envelope;

      foreach (var observation in _observations)
      {
        double[] obs = observation.Value;

        if (obs.Length >= 2)
        {
          double xdir = (_point.X - obs[0])/2;
          double ydir = (_point.Y - obs[1])/2;
          xmin = Math.Min(xmin, _point.X + xdir);
          ymin = Math.Min(ymin, _point.Y + ydir);
          xmax = Math.Max(xmax, obs[0]);
          ymax = Math.Max(ymax, obs[1]);
        }
      }

      _envelope = new EnvelopeClass { XMin = xmin, XMax = xmax, YMin = ymin, YMax = ymax };
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        if (!notCreated)
        {
          avEvents.AfterDraw -= AvEventsAfterDraw;
        }

        avEvents.AfterDraw += AvEventsAfterDraw;
      }

      IEditor3 editor = ArcUtils.Editor;
      var sketch = editor as IEditSketch3;

      if ((sketch != null) && (_measurement != null))
      {
        IGeometry geometry = sketch.Geometry;
        int nrPoints;
        var ptColl = _measurement.ToPointCollection(geometry, out nrPoints);

        if ((ptColl != null) && _measurement.IsSketch)
        {
          if (_intId <= nrPoints)
          {
            IPoint pointC = ptColl.Point[_intId - 1];

            if (!IsSame(pointC))
            {
              ISketchOperation2 sketchOp = new SketchOperationClass();
              sketchOp.Start(editor);
              IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = _index, ZAware = sketch.ZAware };

              if (_measurement.IsPointMeasurement)
              {
                sketch.Geometry = point;
              }
              else
              {
                ptColl.UpdatePoint((_intId - 1), point);

                if ((_intId == 1) && ((nrPoints + 1) == ptColl.PointCount))
                {
                  ptColl.UpdatePoint((ptColl.PointCount - 1), point);
                }

                sketch.Geometry = ptColl as IGeometry;
              }

              geometry = sketch.Geometry;

              if (geometry != null)
              {
                sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
              }
            }
          }
          else
          {
            ISketchOperation2 sketchOp = new SketchOperationClass();
            sketchOp.Start(editor);
            IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = _index, ZAware = sketch.ZAware };
            int nrPoints2 = ptColl.PointCount;

            switch (nrPoints2)
            {
              case 0:
                ptColl.AddPoint(point);

                if (geometry is IPolygon4)
                {
                  ptColl.AddPoint(point);
                }
                break;
              case 1:
                ptColl.AddPoint(point);
                break;
              default:
                if (_intId <= (nrPoints + 1))
                {
                  object point1 = ((_intId - 1) == nrPoints2) ? Type.Missing : (_intId - 1);
                  object point2 = Type.Missing;
                  ptColl.AddPoint(point, ref point1, ref point2);
                }

                break;
            }

            sketch.Geometry = ptColl as IGeometry;
            geometry = sketch.Geometry;

            if (geometry != null)
            {
              sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
            }
          }
        }
        else
        {
          if (geometry is IPoint)
          {
            if (geometry.IsEmpty)
            {
              if ((!double.IsNaN(_point.X)) && (!double.IsNaN(_point.Y)))
              {
                if (!_added)
                {
                  IApplication application = ArcMap.Application;
                  ICommandItem tool = application.CurrentTool;
                  ICommand command = tool.Command;

                  if (!(command is IEditTool))
                  {
                    _added = true;
                    var editorZ = editor as IEditorZ;
                    double zOffset = 0.0;

                    if (editorZ != null)
                    {
                      zOffset = editorZ.ZOffset;
                      editorZ.ZOffset = _point.Z;
                    }

                    ISketchOperation2 sketchOp = new SketchOperationClass();
                    sketchOp.Start(editor);
                    IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = _index, ZAware = sketch.ZAware };
                    sketch.Geometry = point;
                    geometry = sketch.Geometry;
                    sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
                    sketch.FinishSketch();

                    if (editorZ != null)
                    {
                      editorZ.ZOffset = zOffset;
                    }

                    _added = false;
                  }
                }
              }
            }
            else
            {
              var pointC = geometry as IPoint;

              if (!IsSame(pointC))
              {
                if ((!double.IsNaN(_point.X)) && (!double.IsNaN(_point.Y)))
                {
                  ISketchOperation2 sketchOp = new SketchOperationClass();
                  sketchOp.Start(editor);
                  IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = _index, ZAware = sketch.ZAware };
                  sketch.Geometry = point;
                  geometry = sketch.Geometry;
                  sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
                }
              }
            }
          }
        }
      }

      Update();
    }

    public bool IsSame(IPoint point)
    {
      return IsSame(point, true);
    }

    public bool IsSame(IPoint point, bool includeZ)
    {
      const double distance = 0.01;
      return InsideDistance(point, distance, includeZ);
    }

    public void Update()
    {
      GsExtension extension = GsExtension.GetExtension();
      IActiveView activeView = ArcUtils.ActiveView;

      if (extension.InsideScale() && (activeView != null))
      {
        var screenDisplay = activeView.ScreenDisplay;

        if (_oldEnvelope != null)
        {
          screenDisplay.Invalidate(_oldEnvelope, true, (short)esriScreenCache.esriNoScreenCache);
        }

        if (_envelope != null)
        {
          screenDisplay.Invalidate(_envelope, true, (short)esriScreenCache.esriNoScreenCache);
        }
      }
    }

    #endregion

    #region functions (private)

    // =========================================================================
    // Functions (Private)
    // =========================================================================
    private bool InsideDistance(IPoint point, double dinstance, bool includeZ)
    {
      return ((_point != null) && (point != null)) &&
             ((Math.Abs(_point.X - point.X) < dinstance) && (Math.Abs(_point.Y - point.Y) < dinstance) &&
              ((!includeZ) || (Math.Abs(_point.Z - point.Z) < dinstance)));
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Event handlers
    // =========================================================================
    private void AvEventsAfterDraw(IDisplay display, esriViewDrawPhase phase)
    {
      try
      {
        if ((_measurement != null) && (_point != null))
        {
          if ((phase == esriViewDrawPhase.esriViewForeground) && _measurement.IsOpen && _measurement.DrawPoint
            && (!double.IsNaN(_point.X)) && (!double.IsNaN(_point.Y)))
          {
            GsExtension extension = GsExtension.GetExtension();

            if (extension.InsideScale())
            {
              // ReSharper disable UseIndexedProperty
              // ReSharper disable CSharpWarnings::CS0612
              display.StartDrawing(display.hDC, (short) esriScreenCache.esriNoScreenCache);
              IDisplayTransformation dispTrans = display.DisplayTransformation;
              double distance = dispTrans.FromPoints(PointSize);

              IColor color = Converter.ToRGBColor(Color.Black);
              var lineSymbol = new SimpleLineSymbolClass { Color = color, Width = 1 };
              display.SetSymbol(lineSymbol);

              var polylineClass1 = new PolylineClass();
              polylineClass1.AddPoint(new PointClass {X = _point.X - distance, Y = _point.Y});
              polylineClass1.AddPoint(new PointClass {X = _point.X + distance, Y = _point.Y});
              display.DrawPolyline(polylineClass1);

              var polylineClass2 = new PolylineClass();
              polylineClass2.AddPoint(new PointClass {X = _point.X, Y = _point.Y - distance});
              polylineClass2.AddPoint(new PointClass {X = _point.X, Y = _point.Y + distance});              
              display.DrawPolyline(polylineClass2);

              var fontDisp = new StdFontClass
                {
                  Bold = false,
                  Name = "Arial",
                  Italic = false,
                  Underline = false,
                  Size = (decimal) FontSize
                };

              ISymbol textSymbol = new TextSymbolClass {Font = fontDisp as IFontDisp};
              display.SetSymbol(textSymbol);

              double distanceP = (distance*3)/4;
              IPoint pointText = new PointClass {X = _point.X + distanceP, Y = _point.Y + distanceP};
              CultureInfo ci = CultureInfo.InvariantCulture;
              string text = _index.ToString(ci);
              display.DrawText(pointText, text);

              foreach (var observation in _observations)
              {
                double[] obs = observation.Value;

                if (obs.Length >= 2)
                {
                  double xdir = (_point.X - obs[0]) / 2;
                  double ydir = (_point.Y - obs[1]) / 2;
                  string imageId = observation.Key;

                  color = Converter.ToRGBColor(ObsColor.ContainsKey(imageId) ? ObsColor[imageId] : Color.DarkGray);
                  lineSymbol = new SimpleLineSymbolClass { Color = color, Width = 1.25 };
                  display.SetSymbol(lineSymbol);

                  var polylineClass3 = new PolylineClass();
                  polylineClass3.AddPoint(new PointClass {X = _point.X + xdir, Y = _point.Y + ydir});
                  polylineClass3.AddPoint(new PointClass {X = obs[0], Y = obs[1]});
                  display.DrawPolyline(polylineClass3);

                  color = Converter.ToRGBColor(Color.LightGray);
                  lineSymbol = new SimpleLineSymbolClass { Color = color, Width = 0.75 };
                  display.SetSymbol(lineSymbol);
                  display.DrawPolyline(polylineClass3);
                }
              }

              display.FinishDrawing();
              // ReSharper restore CSharpWarnings::CS0612
              // ReSharper restore UseIndexedProperty
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "MeasurementPoint.avEventsAfterDraw");
      }
    }

    #endregion
  }
}
