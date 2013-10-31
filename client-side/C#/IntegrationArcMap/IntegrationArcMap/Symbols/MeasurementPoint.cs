using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.ADF.Connection.Local;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using GlobeSpotterAPI;
using stdole;

using Point = ESRI.ArcGIS.Geometry.Point;
using APIMeasurementPoint = GlobeSpotterAPI.MeasurementPoint;

namespace IntegrationArcMap.Symbols
{
  class MeasurementPoint: IDisposable
  {
    private const int WidthPoint = 128;
    private const int HeightPoint = 128;
    private const int BorderSizePoint = 3;
    private const int PointSize = 8;
    private const double TextMoveFromPoint = 5.0;
    private const decimal TextFontSize = 8;

    private readonly Measurement _measurement;
    private IEnvelope _envelope;
    private ISymbol _symbol;
    private ISymbol _textSymbol;
    private IPoint _point;
    private int _index;
    private int _intId;
    private bool _added;
    private bool _open;

    public int PointId { get; private set; }

    public int M
    {
      get { return _index; }
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

    public MeasurementPoint(int pointId, int intId, Measurement measurement)
    {
      _measurement = measurement;
      _symbol = null;
      _textSymbol = null;
      _envelope = null;
      _index = 0;
      _intId = intId;
      _point = null;
      PointId = pointId;
      _added = false;
      _open = false;
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
      APIMeasurementPoint measurementPoint = measurementData.measurementPoint;
      double x = measurementPoint.x;
      double y = measurementPoint.y;
      double z = measurementPoint.z;
      _point = new Point { X = x, Y = y, Z = z };

      const float pointSizeh = ((float) PointSize)/2;
      double xmin = x - pointSizeh;
      double xmax = x + pointSizeh;
      double ymin = y - pointSizeh;
      double ymax = y + pointSizeh;
      _envelope = new EnvelopeClass {XMin = xmin, XMax = xmax, YMin = ymin, YMax = ymax};

      if (notCreated)
      {
        var avEvents = ArcUtils.ActiveViewEvents;

        if (avEvents != null)
        {
          avEvents.AfterDraw += AvEventsAfterDraw;
        }
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
              IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = PointId, ZAware = sketch.ZAware };

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
            IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = PointId, ZAware = sketch.ZAware };
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
                    IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = PointId, ZAware = sketch.ZAware };
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
                  IPoint point = new PointClass { X = _point.X, Y = _point.Y, Z = _point.Z, M = PointId, ZAware = sketch.ZAware };
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

    private bool InsideDistance(IPoint point, double dinstance, bool includeZ)
    {
      return ((_point != null) && (point != null)) &&
             ((Math.Abs(_point.X - point.X) < dinstance) && (Math.Abs(_point.Y - point.Y) < dinstance) &&
              ((!includeZ) || (Math.Abs(_point.Z - point.Z) < dinstance)));
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

    private void Update()
    {
      GsExtension extension = GsExtension.GetExtension();
      IActiveView activeView = ArcUtils.ActiveView;

      if (extension.InsideScale() && (activeView != null))
      {
        var screenDisplay = activeView.ScreenDisplay;
        screenDisplay.Invalidate(_envelope, true, (short) esriScreenCache.esriNoScreenCache);
      }
    }

    private void AvEventsAfterDraw(IDisplay display, esriViewDrawPhase phase)
    {
      try
      {
        if (_measurement != null)
        {
          if ((phase == esriViewDrawPhase.esriViewForeground) && _measurement.IsOpen && _measurement.DrawPoint)
          {
            GsExtension extension = GsExtension.GetExtension();

            if (extension.InsideScale())
            {
              // ReSharper disable UseIndexedProperty
              // ReSharper disable CSharpWarnings::CS0612
              display.StartDrawing(display.hDC, (short) esriScreenCache.esriNoScreenCache);

              if (_symbol == null)
              {
                Image image = new Bitmap(WidthPoint, HeightPoint);

                using (Graphics ga = Graphics.FromImage(image))
                {
                  const float widthh = ((float) WidthPoint)/2;
                  const float heighth = ((float) HeightPoint)/2;

                  var pen = new Pen(Brushes.Black, BorderSizePoint);
                  ga.Clear(Color.White);
                  ga.DrawLine(pen, 0, heighth, WidthPoint, heighth);
                  ga.DrawLine(pen, widthh, 0, widthh, HeightPoint);
                }

                _symbol = new PictureMarkerSymbolClass
                  {
                    Size = PointSize,
                    BitmapTransparencyColor = Converter.ToRGBColor(Color.White),
                    Picture = OLE.GetIPictureDispFromBitmap(image as Bitmap) as IPictureDisp
                  };
              }

              if (_textSymbol == null)
              {
                var fontDisp = new StdFontClass
                  {
                    Bold = false,
                    Name = "Arial",
                    Italic = false,
                    Underline = false,
                    Size = TextFontSize
                  };

                _textSymbol = new TextSymbolClass {Font = fontDisp as IFontDisp};
              }

              display.SetSymbol(_symbol);
              display.DrawPoint(_point);

              display.SetSymbol(_textSymbol);
              IEnvelope envelopeText = _envelope.Envelope;

              IActiveView activeView = ArcUtils.ActiveView;
              IDisplayTransformation dispTrans = activeView.ScreenDisplay.DisplayTransformation;
              double distance = dispTrans.FromPoints(TextMoveFromPoint);

              envelopeText.Offset(distance, distance);
              display.DrawText(envelopeText, _index.ToString(CultureInfo.InvariantCulture));
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
  }
}
