using System;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF.Connection.Local;
using GlobeSpotterAPI;
using stdole;
using Path = System.IO.Path;
using Point = ESRI.ArcGIS.Geometry.Point;
using Timer = System.Threading.Timer;

namespace IntegrationArcMap.Symbols
{
  class Arrow: IDisposable
  {
    private const int WidthArrow = 128;
    private const int HeightArrow = 128;
    private const int BorderSizeArrow = 0;
    private const int BorderSizeBlinkingArrow = 5;
    private const int ArrowSize = 48;
    private const int BlinkAlpha = 63;
    private const int NormalAlpha = 255;

    private const int MaxTimeUpdate = 100;
    private const int BlinkTime = 200;

    private readonly IPoint _point;
    private readonly Color _color;

    private double _angle;
    private double _hFov;
    private bool _redraw;
    private ISymbol _symbol;
    private bool _blinking;
    private Timer _blinkTimer;
    private Timer _updateTimer;
    private bool _toUpdateArrow;

    private Arrow(RecordingLocation location, double angle, double hFov, Color color)
    {
      _angle = angle;
      _hFov = hFov;
      _color = color;
      _blinking = false;
      _redraw = true;
      _symbol = null;
      _toUpdateArrow = false;

      double x = location.X;
      double y = location.Y;
      _point = new Point {X = x, Y = y};
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.AfterDraw += AvEventsAfterDraw;
      }

      VectorLayer.StartMeasurementEvent += OnMeasurementCreated;
      Update();
    }

    public void Dispose()
    {
      var avEvents = ArcUtils.ActiveViewEvents;
      VectorLayer.StartMeasurementEvent -= OnMeasurementCreated;

      if (avEvents != null)
      {
        avEvents.AfterDraw -= AvEventsAfterDraw;
      }
      
      Update();
    }

    public void Update(double angle, double hFov)
    {
      const double epsilon = 1.0;
      _redraw = (!(Math.Abs(_hFov - hFov) < epsilon)) || _redraw;
      bool update = (!(Math.Abs(_angle - angle) < epsilon)) || _redraw;

      if (update)
      {
        _hFov = hFov;
        _angle = angle;
        Update();
      }
    }

    public void DrawBlinking()
    {
      _blinking = true;
      _redraw = true;
      Update();
    }

    public static Arrow Create(RecordingLocation location, double angle, double hFov, Color color)
    {
      return new Arrow(location, angle, hFov, color);
    }

    private void Redraw(object eventInfo)
    {
      IActiveView activeView = ArcUtils.ActiveView;

      if (activeView != null)
      {
        var screenDisplay = activeView.ScreenDisplay;
        IDisplayTransformation dispTrans = screenDisplay.DisplayTransformation;
        const float arrowSizeh = ((float) ArrowSize)/2;
        double size = dispTrans.FromPoints(arrowSizeh);
        double x = _point.X;
        double y = _point.Y;
        double xmin = x - size;
        double xmax = x + size;
        double ymin = y - size;
        double ymax = y + size;
        IEnvelope envelope = new EnvelopeClass { XMin = xmin, XMax = xmax, YMin = ymin, YMax = ymax };
        screenDisplay.Invalidate(envelope, true, (short) esriScreenCache.esriNoScreenCache);

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

    private void Update()
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

    private void OnMeasurementCreated(IGeometry geometry)
    {
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.AfterDraw -= AvEventsAfterDraw;
        avEvents.AfterDraw += AvEventsAfterDraw;
      }

      Update();
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

            if ((_symbol == null) || _redraw)
            {
              var bitmap = new Bitmap(WidthArrow, HeightArrow);

              using (Graphics ga = Graphics.FromImage(bitmap))
              {
                const float withh = ((float) WidthArrow)/2;
                const float heighth = ((float) HeightArrow)/2;
                double angleh = (_hFov*Math.PI)/360;
                var x = (float) ((withh*Math.Cos(angleh)) + withh - 1);
                var yp = (float) (heighth*Math.Sin(angleh));
                var points = new PointF[3];
                points[0] = new PointF(withh, heighth);
                points[1] = new PointF(x, heighth - yp);
                points[2] = new PointF(x, heighth + yp - 1);
                var pathd = new GraphicsPath();
                pathd.AddPolygon(points);
                var colorBrush = new SolidBrush(Color.FromArgb(_blinking ? BlinkAlpha : NormalAlpha, _color));
                ga.Clear(Color.White);
                ga.FillPath(colorBrush, pathd);
                ga.DrawPath(new Pen(Brushes.Yellow, _blinking ? BorderSizeBlinkingArrow : BorderSizeArrow), pathd);
                pathd.Dispose();
              }

              Bitmap bitmap8B = bitmap.To8BppIndexed();
              string tempPath = Path.GetTempPath();
              string writePath = Path.Combine(tempPath, "Arrow.bmp");
              bitmap8B.Save(writePath, ImageFormat.Bmp);
              IPictureMarkerSymbol pictureMarkerSymbol = new PictureMarkerSymbolClass();
              pictureMarkerSymbol.CreateMarkerSymbolFromFile(esriIPictureType.esriIPictureBitmap, writePath);
              pictureMarkerSymbol.Size = ArrowSize;
              pictureMarkerSymbol.BitmapTransparencyColor = Converter.ToRGBColor(Color.White);
              _symbol = pictureMarkerSymbol as ISymbol;
/*
              _symbol = new PictureMarkerSymbolClass
                {
                  Size = ArrowSize,
                  BitmapTransparencyColor = Converter.ToRGBColor(Color.White),
                  Picture = OLE.GetIPictureDispFromBitmap(bitmap8B) as IPictureDisp
                };
*/

              /*
public void DrawMarker(int x, int y)
{
	IActiveView pActiveView;
	pActiveView = axMapControl1.ActiveView;
	IPoint pPoint;
	pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(x, y);

	IMarkerElement pMarkerElement;
	pMarkerElement = new MarkerElementClass();

	IElement pElement;
	pElement = (IElement)pMarkerElement;
	pElement.Geometry = pPoint;

	IMap pMap;
	pMap = this.axMapControl1.Map;
	IGraphicsContainer pGraphics;
	pGraphics = (IGraphicsContainer)pMap.ActiveGraphicsLayer;
	
	pGraphics.AddElement(pElement, 0);
	IActiveView pView;
	pView = this.axMapControl1.ActiveView;
	pView.PartialRefresh(esriViewDrawPhase.esriViewGraphics,pElement, null);
               * http://forums.esri.com/Thread.asp?c=159&f=1707&t=161631
}

 */
              _redraw = false;
            }

            ((IPictureMarkerSymbol)_symbol).Angle = (450 - _angle) % 360;
            display.SetSymbol(_symbol);
            display.DrawPoint(_point);
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

    private void StartRedraw()
    {
      var redrawEvent = new AutoResetEvent(true);
      var redrawTimerCallBack = new TimerCallback(Redraw);
      _updateTimer = new Timer(redrawTimerCallBack, redrawEvent, MaxTimeUpdate, -1);
      _toUpdateArrow = false;
    }

    private void ResetBlinking(object eventInfo)
    {
      _blinking = false;
      _redraw = true;
      Update();
    }
  }
}
