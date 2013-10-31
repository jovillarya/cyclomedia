using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;

namespace IntegrationArcMap.Elements
{
  [ComVisible(true)]
  [Guid("426E8050-13A9-4449-8BF8-74EF603D9473")]
  public interface IGlobespotterFrame
  {
    ISimpleFillSymbol FillSymbol { get; set; }
    double Size { get; set; }
    double Angle { get; set; }
  }

  [Guid(Classguid)]
  [ClassInterface(ClassInterfaceType.None)]
  [ProgId("GlobespotterFrame.GlobespotterFrameClass")]
  public sealed class GlobespotterFrameClass : IGlobespotterFrame,
                                               IElement,
                                               IElementProperties,
                                               IElementProperties2,
                                               IElementProperties3,
                                               IBoundsProperties,
                                               ITransform2D,
                                               IGraphicElement,
                                               IPersistVariant,
                                               IClone,
                                               IDocumentVersionSupportGEN,
                                               IFrameDraw,
                                               IFrameElement,
                                               IFrameProperties
  {
    #region class members

    //some win32 imports and constants
    [DllImport("gdi32", EntryPoint = "GetDeviceCaps", ExactSpelling = true,
      CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern int GetDeviceCaps(int hDc, int nIndex);

    private const double CCosine30 = 0.866025403784439;
    private const double CDeg2Rad = (Math.PI/180.0);
    private const double CRad2Deg = (180.0/Math.PI);
    private const int CVersion = 2;
    public const string Classguid = "D32CC58C-6450-42EA-AE7C-E4AB68D3A40D";
    public const int Logpixelsx = 88;
    public const int Logpixelsy = 90;

    private IPolygon _mTriangle;
    private IPoint _mPointGeometry;
    private ISimpleFillSymbol _mFillSymbol;
    private double _mRotation;
    private double _mSize = 20.0;
    private ISelectionTracker _mSelectionTracker;
    private IDisplay _mCachedDisplay;
    private ISpatialReference _mNativeSr;
    private string _mElementName = string.Empty;
    private string _mElementType = "TriangleElement";
    private object _mCustomProperty;
    private bool _mAutoTrans = true;
    private double _mScaleRef;
    private esriAnchorPointEnum _mAnchorPointType = esriAnchorPointEnum.esriCenterPoint;
    private double _mDDeviceRatio;

    #endregion

    #region class constructor

    public GlobespotterFrameClass()
    {
      //initialize the element's geometry
      _mTriangle = new PolygonClass();
      _mTriangle.SetEmpty();

      InitMembers();
    }

    #endregion

    #region IGlobespotterFrame Members

    public ISimpleFillSymbol FillSymbol
    {
      get { return _mFillSymbol; }
      set { _mFillSymbol = value; }
    }

    public double Size
    {
      get { return _mSize; }
      set { _mSize = value; }
    }

    public double Angle
    {
      get { return _mRotation; }
      set { _mRotation = value; }
    }

    #endregion

    #region IElement Members

    public void Activate(IDisplay display)
    {
      //cache the display
      _mCachedDisplay = display;

      SetupDeviceRatio(display.hDC, display);

      //need to calculate the points of the triangle polygon
      if (_mTriangle.IsEmpty)
        BuildTriangleGeometry(_mPointGeometry);

      //need to refresh the element's tracker
      RefreshTracker();
    }

    public void Deactivate()
    {
      _mCachedDisplay = null;
    }

    public void Draw(IDisplay display, ITrackCancel trackCancel)
    {
      if (null != _mTriangle && null != _mFillSymbol)
      {
        display.SetSymbol((ISymbol) _mFillSymbol);
        display.DrawPolygon(_mTriangle);
      }
    }

    public IGeometry Geometry
    {
      get { return Clone(_mPointGeometry) as IGeometry; }
      set
      {
        try
        {
          _mPointGeometry = Clone(value) as IPoint;

          UpdateElementSpatialRef();
        }
        catch (Exception ex)
        {
          System.Diagnostics.Trace.WriteLine(ex.Message);
        }
      }
    }

    public bool HitTest(double x, double y, double tolerance)
    {
      if (null == _mCachedDisplay)
        return false;

      IPoint point = new PointClass();
      point.PutCoords(x, y);

      return ((IRelationalOperator) _mTriangle).Contains(point);
    }

    public bool Locked
    {
      get { return false; }
      set { }
    }

    public void QueryBounds(IDisplay display, IEnvelope bounds)
    {
      //return a bounding envelope
      IPolygon polygon = new PolygonClass();
      polygon.SetEmpty();

      ((ISymbol) _mFillSymbol).QueryBoundary(display.hDC, display.DisplayTransformation, _mTriangle, polygon);

      bounds.XMin = polygon.Envelope.XMin;
      bounds.XMax = polygon.Envelope.XMax;
      bounds.YMin = polygon.Envelope.YMin;
      bounds.YMax = polygon.Envelope.YMax;
      bounds.SpatialReference = polygon.Envelope.SpatialReference;
    }

    public void QueryOutline(IDisplay display, IPolygon outline)
    {
      //return a polygon which is the outline of the element
      IPolygon polygon = new PolygonClass();
      polygon.SetEmpty();
      ((ISymbol) _mFillSymbol).QueryBoundary(display.hDC, display.DisplayTransformation, _mTriangle, polygon);
      ((IPointCollection) outline).AddPointCollection((IPointCollection) polygon);
    }

    public ISelectionTracker SelectionTracker
    {
      get { return _mSelectionTracker; }
    }

    #endregion

    #region IElementProperties Members

    /// <summary>
    /// Indicates if transform is applied to symbols and other parts of element.
    /// False = only apply transform to geometry.
    /// Update font size in ITransform2D routines
    /// </summary>
    public bool AutoTransform
    {
      get { return _mAutoTrans; }
      set { _mAutoTrans = value; }
    }

    public object CustomProperty
    {
      get { return _mCustomProperty; }
      set { _mCustomProperty = value; }
    }

    public string Name
    {
      get { return _mElementName; }
      set { _mElementName = value; }
    }

    public string Type
    {
      get { return _mElementType; }
      set { _mElementType = value; }
    }

    #endregion

    #region IElementProperties2 Members

    public bool CanRotate()
    {
      return true;
    }

    public double ReferenceScale
    {
      get { return _mScaleRef; }
      set { _mScaleRef = value; }
    }

    #endregion

    #region IElementProperties3 Members

    public esriAnchorPointEnum AnchorPoint
    {
      get { return _mAnchorPointType; }
      set { _mAnchorPointType = value; }
    }

    #endregion

    #region IBoundsProperties Members

    public bool FixedAspectRatio
    {
      get { return true; }
      set { throw new Exception("The method or operation is not implemented."); }
    }

    public bool FixedSize
    {
      get { return true; }
    }

    #endregion

    #region ITransform2D Members

    public void Move(double dx, double dy)
    {
      if (null == _mTriangle)
        return;

      ((ITransform2D) _mTriangle).Move(dx, dy);
      ((ITransform2D) _mPointGeometry).Move(dx, dy);

      RefreshTracker();
    }

    public void MoveVector(ILine v)
    {
      if (null == _mTriangle)
        return;

      ((ITransform2D) _mTriangle).MoveVector(v);
      ((ITransform2D) _mPointGeometry).MoveVector(v);

      RefreshTracker();
    }

    public void Rotate(IPoint origin, double rotationAngle)
    {
      if (null == _mTriangle)
        return;

      ((ITransform2D) _mTriangle).Rotate(origin, rotationAngle);
      ((ITransform2D) _mPointGeometry).Rotate(origin, rotationAngle);

      _mRotation = rotationAngle*CRad2Deg;

      RefreshTracker();
    }

    public void Scale(IPoint origin, double sx, double sy)
    {
      if (null == _mTriangle)
        return;

      ((ITransform2D) _mTriangle).Scale(origin, sx, sy);
      ((ITransform2D) _mPointGeometry).Scale(origin, sx, sy);

      if (_mAutoTrans)
      {
        _mSize *= Math.Max(sx, sy);
      }

      RefreshTracker();
    }

    public void Transform(esriTransformDirection direction, ITransformation transformation)
    {
      if (null == _mTriangle)
        return;

      //Geometry
      ((ITransform2D) _mTriangle).Transform(direction, transformation);

      var affineTrans = (IAffineTransformation2D) transformation;
      if (affineTrans.YScale != 1.0)
        _mSize *= Math.Max(affineTrans.YScale, affineTrans.XScale);

      RefreshTracker();
    }

    #endregion

    #region IGraphicElement Members

    public ISpatialReference SpatialReference
    {
      get { return _mNativeSr; }
      set
      {
        _mNativeSr = value;
        UpdateElementSpatialRef();
      }
    }

    #endregion

    #region IPersistVariant Members

    public UID ID
    {
      get
      {
        UID uid = new UIDClass();
        uid.Value = "{" + Classguid + "}";
        return uid;
      }
    }

    public void Load(IVariantStream stream)
    {
      var ver = (int) stream.Read();
      if (ver > CVersion || ver <= 0)
        throw new Exception("Wrong version!");

      InitMembers();

      _mSize = (double) stream.Read();
      _mScaleRef = (double) stream.Read();
      _mAnchorPointType = (esriAnchorPointEnum) stream.Read();
      _mAutoTrans = (bool) stream.Read();
      _mElementType = (string) stream.Read();
      _mElementName = (string) stream.Read();
      _mNativeSr = stream.Read() as ISpatialReference;
      _mFillSymbol = stream.Read() as ISimpleFillSymbol;
      _mPointGeometry = stream.Read() as IPoint;
      _mTriangle = stream.Read() as IPolygon;

      if (ver == 2)
      {
        _mRotation = (double) stream.Read();
      }
    }

    public void Save(IVariantStream stream)
    {
      stream.Write(CVersion);

      stream.Write(_mSize);
      stream.Write(_mScaleRef);
      stream.Write(_mAnchorPointType);
      stream.Write(_mAutoTrans);
      stream.Write(_mElementType);
      stream.Write(_mElementName);
      stream.Write(_mNativeSr);
      stream.Write(_mFillSymbol);
      stream.Write(_mPointGeometry);
      stream.Write(_mTriangle);

      stream.Write(_mRotation);
    }

    #endregion

    #region IClone Members

    public void Assign(IClone src)
    {

      //1. make sure that src is pointing to a valid object
      if (null == src)
      {
        throw new COMException("Invalid object.");
      }

      //2. make sure that the type of src is of type 'TriangleElementClass'
      if (!(src is GlobespotterFrameClass))
      {
        throw new COMException("Bad object type.");
      }

      //3. assign the properties of src to the current instance
      var srcTriangle = (GlobespotterFrameClass)src;
      _mElementName = srcTriangle.Name;
      _mElementType = srcTriangle.Type;
      _mAutoTrans = srcTriangle.AutoTransform;
      _mScaleRef = srcTriangle.ReferenceScale;
      _mRotation = srcTriangle.Angle;
      _mSize = srcTriangle.Size;
      _mAnchorPointType = srcTriangle.AnchorPoint;

      IObjectCopy objCopy = new ObjectCopyClass();

      //take care of the custom property
      if (null != srcTriangle.CustomProperty)
      {
        if (srcTriangle.CustomProperty is IClone)
          _mCustomProperty = ((IClone) srcTriangle.CustomProperty).Clone();
        else if (srcTriangle.CustomProperty is IPersistStream)
        {
          _mCustomProperty = objCopy.Copy(srcTriangle.CustomProperty);
        }
        else if (srcTriangle.CustomProperty.GetType().IsSerializable)
        {
          //serialize to a memory stream
          var memoryStream = new MemoryStream();
          var binaryFormatter = new BinaryFormatter();
          binaryFormatter.Serialize(memoryStream, srcTriangle.CustomProperty);
          byte[] bytes = memoryStream.ToArray();

          memoryStream = new MemoryStream(bytes);
          _mCustomProperty = binaryFormatter.Deserialize(memoryStream);
        }
      }

      if (null != srcTriangle.SpatialReference)
        _mNativeSr = objCopy.Copy(srcTriangle.SpatialReference) as ISpatialReference;
      else
        _mNativeSr = null;

      if (null != srcTriangle.FillSymbol)
      {
        _mFillSymbol = objCopy.Copy(srcTriangle.FillSymbol) as ISimpleFillSymbol;
      }
      else
        _mFillSymbol = null;

      if (null != srcTriangle.Geometry)
      {
        _mTriangle = objCopy.Copy(srcTriangle.Geometry) as IPolygon;
        _mPointGeometry = objCopy.Copy(((IArea) _mTriangle).Centroid) as IPoint;
      }
      else
      {
        _mTriangle = null;
        _mPointGeometry = null;
      }
    }

    public IClone Clone()
    {
      var triangle = new GlobespotterFrameClass();
      triangle.Assign(this);

      return triangle;
    }

    public bool IsEqual(IClone other)
    {
      //1. make sure that the 'other' object is pointing to a valid object
      if (null == other)
        throw new COMException("Invalid object.");

      //2. verify the type of 'other'
      if (!(other is GlobespotterFrameClass))
        throw new COMException("Bad object type.");

      var otherTriangle = (GlobespotterFrameClass)other;
      //test that all of the object's properties are the same.
      //please note the usage of IsEqual when using ArcObjects components that
      //supports cloning
      if (otherTriangle.Name == _mElementName &&
          otherTriangle.Type == _mElementType &&
          otherTriangle.AutoTransform == _mAutoTrans &&
          otherTriangle.ReferenceScale == _mScaleRef &&
          otherTriangle.Angle == _mRotation &&
          otherTriangle.Size == _mSize &&
          otherTriangle.AnchorPoint == _mAnchorPointType &&
          ((IClone) otherTriangle.Geometry).IsEqual((IClone) _mTriangle) &&
          ((IClone) otherTriangle.FillSymbol).IsEqual((IClone) _mFillSymbol) &&
          ((IClone) otherTriangle.SpatialReference).IsEqual((IClone) _mNativeSr))
        return true;

      return false;
    }

    public bool IsIdentical(IClone other)
    {
      //1. make sure that the 'other' object is pointing to a valid object
      if (null == other)
        throw new COMException("Invalid object.");

      //2. verify the type of 'other'
      if (!(other is GlobespotterFrameClass))
        throw new COMException("Bad object type.");

      //3. test if the other is the 'this'
      if (other == this)
        return true;

      return false;
    }

    #endregion

    #region IDocumentVersionSupportGEN Members

    public object ConvertToSupportedObject(esriArcGISVersion docVersion)
    {
      //in case of 8.3, create a character marker element and use a triangle marker...
      ICharacterMarkerSymbol charMarkerSymbol = new CharacterMarkerSymbolClass();
      charMarkerSymbol.Color = _mFillSymbol.Color;
      charMarkerSymbol.Angle = _mRotation;
      charMarkerSymbol.Size = _mSize;
      charMarkerSymbol.Font =
        ESRI.ArcGIS.ADF.Connection.Local.Converter.ToStdFont(new Font("ESRI Default Marker", (float) _mSize,
                                                                      FontStyle.Regular));
      charMarkerSymbol.CharacterIndex = 184;

      IMarkerElement markerElement = new MarkerElementClass();
      markerElement.Symbol = charMarkerSymbol;

      var point = ((IClone) _mPointGeometry).Clone() as IPoint;
      var element = (IElement) markerElement;
      element.Geometry = point;

      return element;
    }

    public bool IsSupportedAtVersion(esriArcGISVersion docVersion)
    {
      //support all versions except 8.3
      if (esriArcGISVersion.esriArcGISVersion83 == docVersion)
        return false;
      else
        return true;
    }

    #endregion

    #region private methods

    private IClone Clone(object obj)
    {
      if (null == obj || !(obj is IClone))
        return null;

      return ((IClone) obj).Clone();
    }

    private int TwipsPerPixelX()
    {
      return 16;
    }

    private int TwipsPerPixelY()
    {
      return 16;
    }

    private void SetupDeviceRatio(int hDc, IDisplay display)
    {
      if (display.DisplayTransformation != null)
      {
        if (display.DisplayTransformation.Resolution != 0)
        {
          _mDDeviceRatio = display.DisplayTransformation.Resolution/72;
          //  Check the ReferenceScale of the display transformation. If not zero, we need to
          //  adjust the Size, XOffset and YOffset of the Symbol we hold internally before drawing.
          if (display.DisplayTransformation.ReferenceScale != 0)
            _mDDeviceRatio = _mDDeviceRatio*display.DisplayTransformation.ReferenceScale/
                             display.DisplayTransformation.ScaleRatio;
        }
      }
      else
      {
        // If we don't have a display transformation, calculate the resolution
        // from the actual device.
        if (display.hDC != 0)
        {
          // Get the resolution from the device context hDC.
          _mDDeviceRatio = Convert.ToDouble(GetDeviceCaps(hDc, Logpixelsx))/72;
        }
        else
        {
          // If invalid hDC assume we're drawing to the screen.
          _mDDeviceRatio = 1/(TwipsPerPixelX()/20); // 1 Point = 20 Twips.
        }
      }
    }

    private double PointsToMap(IDisplayTransformation displayTransform, double dPointSize)
    {
      double tempPointsToMap = 0;
      if (displayTransform == null)
        tempPointsToMap = dPointSize*_mDDeviceRatio;
      else
      {
        tempPointsToMap = displayTransform.FromPoints(dPointSize);
      }
      return tempPointsToMap;
    }

    private void BuildTriangleGeometry(IPoint pointGeometry)
    {
      try
      {
        if (null == _mTriangle || null == pointGeometry || null == _mCachedDisplay)
          return;

        _mTriangle.SpatialReference = pointGeometry.SpatialReference;
        _mTriangle.SetEmpty();

        object missing = System.Reflection.Missing.Value;
        var pointCollection = (IPointCollection) _mTriangle;

        double radius = PointsToMap(_mCachedDisplay.DisplayTransformation, _mSize);

        double X = pointGeometry.X;
        double Y = pointGeometry.Y;

        IPoint point = new PointClass();
        point.X = X + radius*CCosine30;
        point.Y = Y - 0.5*radius;
        pointCollection.AddPoint(point, ref missing, ref missing);

        point = new PointClass();
        point.X = X;
        point.Y = Y + radius;
        pointCollection.AddPoint(point, ref missing, ref missing);

        point = new PointClass();
        point.X = X - radius*CCosine30;
        point.Y = Y - 0.5*radius;
        pointCollection.AddPoint(point, ref missing, ref missing);

        _mTriangle.Close();

        if (_mRotation != 0.0)
        {
          ((ITransform2D) pointCollection).Rotate(pointGeometry, _mRotation*CDeg2Rad);
        }

        return;
      }
      catch (Exception ex)
      {
        System.Diagnostics.Trace.WriteLine(ex.Message);
      }
    }

    private void SetDefaultDymbol()
    {
      IColor color = ESRI.ArcGIS.ADF.Connection.Local.Converter.ToRGBColor(Color.Black);
      ISimpleLineSymbol lineSymbol = new SimpleLineSymbolClass();
      lineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
      lineSymbol.Width = 1.0;
      lineSymbol.Color = color;

      color = ESRI.ArcGIS.ADF.Connection.Local.Converter.ToRGBColor(Color.Navy);
      if (null == _mFillSymbol)
        _mFillSymbol = new SimpleFillSymbolClass();
      _mFillSymbol.Color = color;
      _mFillSymbol.Style = esriSimpleFillStyle.esriSFSSolid;
      _mFillSymbol.Outline = lineSymbol;
    }


    /// <summary>
    /// assign the triangle's geometry to the selection tracker
    /// </summary>
    private void RefreshTracker()
    {
      if (null == _mCachedDisplay)
        return;

      _mSelectionTracker.Display = (IScreenDisplay) _mCachedDisplay;


      IPolygon outline = new PolygonClass();
      this.QueryOutline(_mCachedDisplay, outline);

      _mSelectionTracker.Geometry = (IGeometry) outline;
    }

    private void UpdateElementSpatialRef()
    {
      if (null == _mCachedDisplay ||
          null == _mNativeSr ||
          null == _mTriangle ||
          null == _mCachedDisplay.DisplayTransformation.SpatialReference)
        return;

      if (null == _mTriangle.SpatialReference)
        _mTriangle.SpatialReference = _mCachedDisplay.DisplayTransformation.SpatialReference;

      _mTriangle.Project(_mNativeSr);

      RefreshTracker();
    }

    private void InitMembers()
    {
      //initialize the selection tracker
      _mSelectionTracker = new PolygonTrackerClass();
      _mSelectionTracker.Locked = false;
      _mSelectionTracker.ShowHandles = true;

      //set a default symbol
      SetDefaultDymbol();
    }

    #endregion

    public void DrawBackground(IDisplay display, ITrackCancel cancelTracker)
    {
      throw new NotImplementedException();
    }

    public void DrawDraftMode(IDisplay display, ITrackCancel cancelTracker)
    {
      throw new NotImplementedException();
    }

    public void DrawForeground(IDisplay display, ITrackCancel cancelTracker)
    {
      throw new NotImplementedException();
    }

    public object Object { get; private set; }
    IBorder IFrameElement.Border { get; set; }
    IBackground IFrameProperties.Background { get; set; }
    public IShadow Shadow { get; set; }
    IBorder IFrameProperties.Border { get; set; }
    IBackground IFrameElement.Background { get; set; }
    public int Thumbnail { get; private set; }
    public bool DraftMode { get; set; }
  }
}
