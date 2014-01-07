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
using System.Threading;
using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Client;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using GlobeSpotterAPI;
using Point = ESRI.ArcGIS.Geometry.Point;

namespace IntegrationArcMap.Objects
{
  class Arrow: IDisposable
  {
    #region constants

    // =========================================================================
    // Constants
    // =========================================================================
    private const double BorderSizeArrow = 1;
    private const double BorderSizeBlinkingArrow = 2.5;
    private const int ArrowSize = 48;
    private const byte BlinkAlpha = 0;
    private const byte NormalAlpha = 255;

    private const int MaxTimeUpdate = 100;
    private const int BlinkTime = 200;

    #endregion

    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private readonly IPoint _point;
    private readonly Color _color;

    private double _angle;
    private double _hFov;
    private bool _blinking;
    private Timer _blinkTimer;
    private Timer _updateTimer;
    private bool _toUpdateArrow;
    private bool _active;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    private Arrow(RecordingLocation location, double angle, double hFov, Color color)
    {
      _angle = angle;
      _hFov = hFov;
      _color = color;
      _blinking = false;
      _toUpdateArrow = false;
      _active = true;

      double x = location.X;
      double y = location.Y;
      Config config = Config.Instance;
      SpatialReference spatRel = config.SpatialReference;
      ISpatialReference spatialReference = (spatRel == null) ? ArcUtils.SpatialReference : spatRel.SpatialRef;

      _point = new Point
        {
          X = x,
          Y = y,
          SpatialReference = spatialReference
        };

      _point.Project(ArcUtils.SpatialReference);
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.AfterDraw += AvEventsAfterDraw;
      }

      VectorLayer.StartMeasurementEvent += OnMeasurementCreated;
      Redraw();
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public void Dispose()
    {
      var avEvents = ArcUtils.ActiveViewEvents;
      VectorLayer.StartMeasurementEvent -= OnMeasurementCreated;

      if (avEvents != null)
      {
        avEvents.AfterDraw -= AvEventsAfterDraw;
      }
      
      Redraw();
    }

    public void Update(double angle, double hFov)
    {
      const double epsilon = 1.0;
      bool update = (!(Math.Abs(_angle - angle) < epsilon)) || (!(Math.Abs(_hFov - hFov) < epsilon));

      if (update)
      {
        _hFov = hFov;
        _angle = angle;
        Redraw();
      }
    }

    public void SetActive(bool active)
    {
      _blinking = active;
      _active = active;

      if (active)
      {
        ToForeGround();
      }
      else
      {
        Redraw();
      }
    }

    public void Redraw()
    {
      GsExtension extension = GsExtension.GetExtension();

      if (extension.InsideScale())
      {
        if (_updateTimer == null)
        {
          StartRedraw();
        }
        else
        {
          _toUpdateArrow = true;
        }
      }
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static Arrow Create(RecordingLocation location, double angle, double hFov, Color color)
    {
      return new Arrow(location, angle, hFov, color);
    }

    #endregion

    #region thread functions

    // =========================================================================
    // Thread functions
    // =========================================================================
    private void Redraw(object eventInfo)
    {
      IActiveView activeView = ArcUtils.ActiveView;

      if (activeView != null)
      {
        var display = activeView.ScreenDisplay;
        IDisplayTransformation dispTrans = display.DisplayTransformation;
        const float arrowSizeh = ((float) ArrowSize)/2;
        double size = dispTrans.FromPoints(arrowSizeh);
        double x = _point.X;
        double y = _point.Y;
        double xmin = x - size;
        double xmax = x + size;
        double ymin = y - size;
        double ymax = y + size;
        IEnvelope envelope = new EnvelopeClass {XMin = xmin, XMax = xmax, YMin = ymin, YMax = ymax};
        display.Invalidate(envelope, true, (short) esriScreenCache.esriNoScreenCache);

        if (_toUpdateArrow)
        {
          StartRedraw();
        }
        else
        {
          _updateTimer = null;
        }
      }
    }

    private void ResetBlinking(object eventInfo)
    {
      _blinking = false;
      Redraw();
    }

    #endregion

    #region event handlers

    // =========================================================================
    // event handlers
    // =========================================================================
    private void OnMeasurementCreated(IGeometry geometry)
    {
      ToForeGround();
    }

    private void AvEventsAfterDraw(IDisplay display, esriViewDrawPhase phase)
    {
      try
      {
        if (phase == esriViewDrawPhase.esriViewForeground)
        {
          GsExtension extension = GsExtension.GetExtension();

          if (extension.InsideScale())
          {
            // ReSharper disable UseIndexedProperty
            // ReSharper disable CSharpWarnings::CS0612
            display.StartDrawing(display.hDC, (short) esriScreenCache.esriNoScreenCache);

            IDisplayTransformation dispTrans = display.DisplayTransformation;
            const float arrowSizeh = ((float) ArrowSize)/2;
            double size = dispTrans.FromPoints(arrowSizeh);

            double angleh = (_hFov*Math.PI)/360;
            double angle = (((450 - _angle)%360)*Math.PI)/180;
            double angle1 = angle - angleh;
            double angle2 = angle + angleh;
            double x = _point.X;
            double y = _point.Y;

            IPoint point1 = new PointClass {X = x, Y = y};
            IPoint point2 = new PointClass {X = x + (size*Math.Cos(angle1)), Y = y + (size*Math.Sin(angle1))};
            IPoint point3 = new PointClass {X = x + (size*Math.Cos(angle2)), Y = y + (size*Math.Sin(angle2))};

            IPolygon4 polygon = new PolygonClass();
            var polygonPoint = polygon as IPointCollection4;
            polygonPoint.AddPoint(point1);
            polygonPoint.AddPoint(point2);
            polygonPoint.AddPoint(point3);
            polygonPoint.AddPoint(point1);

            IColor color = Converter.ToRGBColor(_color);
            color.Transparency = _blinking ? BlinkAlpha : NormalAlpha;
            color.UseWindowsDithering = true;
            var fillSymbol = new SimpleFillSymbolClass {Color = color, Outline = null};
            display.SetSymbol(fillSymbol);
            display.DrawPolygon(polygon);

            IPolyline polyline = new PolylineClass();
            var polylinePoint = polyline as IPointCollection4;
            polylinePoint.AddPoint(point2);
            polylinePoint.AddPoint(point1);
            polylinePoint.AddPoint(point3);

            var outlineSymbol = new SimpleLineSymbolClass
            {
              Color = Converter.ToRGBColor(_active ? Color.Yellow : Color.Gray),
              Width = _blinking ? BorderSizeBlinkingArrow : BorderSizeArrow
            };

            display.SetSymbol(outlineSymbol);
            display.DrawPolyline(polyline);
            display.FinishDrawing();

            if (_blinking)
            {
              var blinkEvent = new AutoResetEvent(true);
              var blinkTimerCallBack = new TimerCallback(ResetBlinking);
              _blinkTimer = new Timer(blinkTimerCallBack, blinkEvent, BlinkTime, -1);
            }

            // ReSharper restore CSharpWarnings::CS0612
            // ReSharper restore UseIndexedProperty
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "Arrow.avEventsAfterDraw");
      }
    }

    #endregion

    #region functions (private)

    // =========================================================================
    // Functions (Private)
    // =========================================================================
    private void ToForeGround()
    {
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.AfterDraw -= AvEventsAfterDraw;
        avEvents.AfterDraw += AvEventsAfterDraw;
      }

      Redraw();
    }

    private void StartRedraw()
    {
      var redrawEvent = new AutoResetEvent(true);
      var redrawTimerCallBack = new TimerCallback(Redraw);
      _updateTimer = new Timer(redrawTimerCallBack, redrawEvent, MaxTimeUpdate, -1);
      _toUpdateArrow = false;
    }

    #endregion
  }
}
