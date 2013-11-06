using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using IntegrationArcMap.Model;
using IntegrationArcMap.Model.Shape;
using IntegrationArcMap.Symbols;
using IntegrationArcMap.Utilities;
using IntegrationArcMap.WebClient;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using Point = ESRI.ArcGIS.Geometry.Point;
using ShapePoint = IntegrationArcMap.Model.Shape.Point;

namespace IntegrationArcMap.Layers
{
  public delegate void CycloMediaLayerAddedDelegate(CycloMediaLayer cycloMediaLayer);
  public delegate void CycloMediaLayerChangedDelegate(CycloMediaLayer cycloMediaLayer);
  public delegate void CycloMediaLayerRemoveDelegate(CycloMediaLayer cycloMediaLayer);

  public delegate void HistoricalDateDelegate(SortedDictionary<int, int> yearMonth);

  public abstract class CycloMediaLayer : IDisposable
  {
    public static event CycloMediaLayerAddedDelegate LayerAddedEvent;
    public static event CycloMediaLayerChangedDelegate LayerChangedEvent;
    public static event CycloMediaLayerRemoveDelegate LayerRemoveEvent;

    public static event HistoricalDateDelegate HistoricalDateChanged;

    private static SortedDictionary<int, int> _yearMonth;

    private readonly CycloMediaGroupLayer _cycloMediaGroupLayer;
    private readonly object _getDataLock;

    private IFeatureClass _featureClass;
    private ILayer _layer;
    private IEnvelope _lastextent;
    private List<XElement> _addData;
    private Thread _getDataThread;
    private Thread _refreshDataThread;
    private bool _isVisibleInGlobespotter;

    public abstract string[] FieldNames { get; }
    public abstract string Name { get; }
    public abstract string FcName { get; }
    public abstract Color Color { get; set; }
    public abstract int SizeLayer { get; }
    public abstract double MinimumScale { get; set; }
    public abstract bool UseDateRange { get; }
    public abstract string WfsRequest { get; }

    protected abstract IMappedFeature CreateMappedFeature(XElement mappedFeatureElement);
    protected abstract bool Filter(IMappedFeature mappedFeature);
    protected abstract void PostEntryStep();

    protected static SortedDictionary<int, int> YearMonth
    {
      get { return _yearMonth ?? (_yearMonth = new SortedDictionary<int, int>()); }
    }

    protected static void ChangeHistoricalDate()
    {
      if (HistoricalDateChanged != null)
      {
        HistoricalDateChanged(YearMonth);
      }
    }

    public ILayer Layer
    {
      get { return _layer ?? (_layer = GetLayer()); }
    }

    public bool IsVisible
    {
      get { return (_layer != null) && _layer.Visible; }
    }

    public bool IsVisibleInGlobespotter
    {
      get { return (_isVisibleInGlobespotter && IsVisible); }
      set
      {
        _isVisibleInGlobespotter = value;
        OnContentChanged();
      }
    }

    public bool InsideScale
    {
      get
      {
        IMap map = ArcUtils.Map;
        return (map != null) && (Math.Floor(map.MapScale) <= (MinimumScale = Layer.MinimumScale));
      }
    }

    protected CycloMediaLayer(CycloMediaGroupLayer layer)
    {
      _getDataLock = new object();
      _addData = null;
      _getDataThread = null;
      _cycloMediaGroupLayer = layer;
      _isVisibleInGlobespotter = true;
      IActiveView activeView = ArcUtils.ActiveView;

      if (activeView != null)
      {
        _lastextent = activeView.Extent;
      }
    }

    public IFeatureClass FeatureClass
    {
      get { return _featureClass ?? (_featureClass = OpenFeatureClassInWorkspace()); }
    }

    public void AddToLayers()
    {
      IList<CycloMediaLayer> layers = _cycloMediaGroupLayer.Layers;

      if (!layers.Contains(this))
      {
        layers.Add(this);
      }

      CreateFeatureLayer();
      Refresh();
    }

    public virtual void UpdateColor(Color color, int? year)
    {
      if (year == null)
      {
        Color = color;
        ArcUtils.SetColorToLayer(Layer, color);
        Refresh();
      }
    }

    public virtual DateTime? GetDate()
    {
      return null;
    }

    public void CreateFeatureLayer()
    {
      if (Layer == null)
      {
        // ReSharper disable UseIndexedProperty
        // ReSharper disable CSharpWarnings::CS0612
        _layer = new FeatureLayerClass
          {
            Name = Name,
            MinimumScale = MinimumScale,
            Visible = true
          };

        IFeatureClass featureClass = OpenFeatureClassInWorkspace();

        if (featureClass != null)
        {
          var featureLayer = _layer as IFeatureLayer;
          featureLayer.FeatureClass = featureClass;
        }

        var markerSymbol = new SimpleMarkerSymbol
          {
            Color = Converter.ToRGBColor(Color),
            Size = SizeLayer
          };

        IUniqueValueRenderer renderer = new UniqueValueRendererClass
          {
            DefaultSymbol = markerSymbol as ISymbol,
            DefaultLabel = string.Empty,
            UseDefaultSymbol = this is WfsLayer,
            FieldCount = FieldNames.Length
          };

        for (int i = 0; i < FieldNames.Length; i++)
        {
          renderer.set_Field(i, FieldNames[i]);
        }

        var geoFeatureLayer = _layer as IGeoFeatureLayer;
        geoFeatureLayer.Renderer = renderer as IFeatureRenderer;
        // ReSharper restore CSharpWarnings::CS0612
        // ReSharper restore UseIndexedProperty
      }

      if (_cycloMediaGroupLayer != null)
      {
        IGroupLayer groupLayer = _cycloMediaGroupLayer.GroupLayer;

        if (groupLayer != null)
        {
          ILayer layer = GetLayer();

          if (layer == null)
          {
            groupLayer.Add(_layer);
          }
        }
      }

      if (LayerAddedEvent != null)
      {
        LayerAddedEvent(this);
      }

      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.ViewRefreshed += OnViewRefreshed;
        avEvents.ContentsChanged += OnContentChanged;
        avEvents.ItemDeleted += OnItemDeleted;
      }
    }

    public void Dispose()
    {
      IGroupLayer groupLayer = _cycloMediaGroupLayer.GroupLayer;

      if (groupLayer != null)
      {
        groupLayer.Delete(_layer);
      }

      Remove();
    }

    protected virtual void Remove()
    {
      var avEvents = ArcUtils.ActiveViewEvents;

      if (LayerRemoveEvent != null)
      {
        LayerRemoveEvent(this);
      }

      if (avEvents != null)
      {
        avEvents.ViewRefreshed -= OnViewRefreshed;
        avEvents.ContentsChanged -= OnContentChanged;
        avEvents.ItemDeleted -= OnItemDeleted;
      }
    }

    public void Refresh()
    {
      IActiveView activeView = ArcUtils.ActiveView;

      if (activeView != null)
      {
        _lastextent = activeView.Extent.Envelope;
        _lastextent.Expand(5, 5, true);
        activeView.PartialRefresh(esriViewDrawPhase.esriViewForeground, Layer, null);
        activeView.ContentsChanged();
      }
    }

    private IFeatureClass OpenFeatureClassInWorkspace()
    {
      IFeatureClass result = null;
      IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
      var workspace = featureWorkspace as IWorkspace2;
      // ReSharper disable UseIndexedProperty

      if (workspace != null)
      {
        result = workspace.get_NameExists(esriDatasetType.esriDTFeatureClass, FcName)
                   ? featureWorkspace.OpenFeatureClass(FcName)
                   : CreateFeatureClass();
      }

      // ReSharper restore UseIndexedProperty
      return result;
    }

    public string GetFeatureFromPoint(int x, int y)
    {
      string result = string.Empty;
      IActiveView activeView = ArcUtils.ActiveView;

      if ((activeView != null) && InsideScale)
      {
        IMappedFeature mapFeature = CreateMappedFeature(null);
        string objectId = mapFeature.ObjectId;
        IDisplayTransformation dispTrans = activeView.ScreenDisplay.DisplayTransformation;
        IPoint pointLu = dispTrans.ToMapPoint(x - SizeLayer, y - SizeLayer);
        IPoint pointRd = dispTrans.ToMapPoint(x + SizeLayer, y + SizeLayer);
        IEnvelope envelope = new EnvelopeClass {XMin = pointLu.X, XMax = pointRd.X, YMin = pointLu.Y, YMax = pointRd.Y};

        ISpatialFilter spatialFilter = new SpatialFilterClass
                                         {
                                           Geometry = envelope,
                                           GeometryField = FeatureClass.ShapeFieldName,
                                           SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                                           SubFields = objectId
                                         };

        var existsResult = FeatureClass.Search(spatialFilter, false);
        IFeature feature;

        // ReSharper disable UseIndexedProperty
        while ((feature = existsResult.NextFeature()) != null)
        {
          int imId = existsResult.FindField(objectId);
          result = (string) feature.get_Value(imId);
        }

        // ReSharper restore UseIndexedProperty
      }

      return result;
    }

    public virtual double GetHeight(double x, double y)
    {
      return 0.0;
    }

    public void AddZToSketch(IEditSketch3 sketch)
    {
      var editor = ArcUtils.Editor;
      var editorZ = editor as IEditorZ;
      IPoint point = sketch.LastPoint;

      if ((editorZ != null) && (point != null) && sketch.ZAware)
      {
        double z = GetHeight(point.X, point.Y);
        editorZ.UseZOffset = true;
        editorZ.ZOffset = z;

        IGeometry geometry = sketch.Geometry;
        var ptColl = geometry as IPointCollection4;
        ISketchOperation2 sketchOp = new SketchOperationClass();
        sketchOp.Start(editor);
        IPoint pointc = null;

        if ((!geometry.IsEmpty) && (geometry is IPoint) && (ptColl == null))
        {
          pointc = geometry as IPoint;
          ptColl = new MultipointClass();
          ptColl.AddPoint(pointc);
        }

        if (ptColl != null)
        {
          int nrPoints = ptColl.PointCount;

          for (int i = 0; i < nrPoints; i++)
          {
            IPoint pointC = ptColl.Point[i];
            // ReSharper disable CompareOfFloatsByEqualityOperator

            if (pointC.Z == 0)
            {
              IPoint newPoint = new PointClass {X = pointC.X, Y = pointC.Y, Z = z};
              ptColl.UpdatePoint(i, newPoint);
            }

            // ReSharper restore CompareOfFloatsByEqualityOperator
          }
        }

        sketch.Geometry = pointc ?? (ptColl as IGeometry);
        geometry = sketch.Geometry;

        if (geometry != null)
        {
          sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
        }
      }
    }

    public IFeatureClass CreateFeatureClass()
    {
      IFeatureClass result = null;
      IFieldsEdit fieldsEdit = new FieldsClass();
      fieldsEdit = CreateField(fieldsEdit, "ObjectId", esriFieldType.esriFieldTypeOID);

      IMappedFeature mapFeature = CreateMappedFeature(null);
      esriGeometryType esriGeometryType = mapFeature.EsriGeometryType;
      Dictionary<string, esriFieldType> fields = mapFeature.Fields;

      if ((esriGeometryType != esriGeometryType.esriGeometryNull) && (fields.Count >= 1))
      {
        string shapeFieldName = mapFeature.ShapeFieldName;
        ISpatialReference spatialReference = ArcUtils.SpatialReference;
        fieldsEdit = CreateGeometryField(fieldsEdit, shapeFieldName, spatialReference, esriGeometryType);
        fieldsEdit = fields.Aggregate(fieldsEdit, (current, field) => CreateField(current, field.Key, field.Value));

        foreach (var fieldName in FieldNames)
        {
          if ((!string.IsNullOrEmpty(fieldName)) &&
              (!fields.Aggregate(false, (current, field) => (field.Key == fieldName) || current)))
          {
            CreateField(fieldsEdit, fieldName, esriFieldType.esriFieldTypeString);
          }
        }

        IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
        result = featureWorkspace.CreateFeatureClass(FcName, fieldsEdit, null, null, esriFeatureType.esriFTSimple,
                                                     shapeFieldName, string.Empty);
      }

      return result;
    }

    private static IFieldsEdit CreateField(IFieldsEdit fieldsEdit, string fieldName, esriFieldType esriFieldType)
    {
      // ReSharper disable UseObjectOrCollectionInitializer
      IFieldEdit fieldEdit = new FieldClass();
      fieldEdit.Type_2 = esriFieldType;
      fieldEdit.Name_2 = fieldName;
      fieldEdit.AliasName_2 = fieldName;
      fieldsEdit.AddField(fieldEdit);
      return fieldsEdit;
      // ReSharper restore UseObjectOrCollectionInitializer
    }

    private static IFieldsEdit CreateGeometryField(IFieldsEdit fieldsEdit, string fieldName,
                                                   ISpatialReference spatialReference, esriGeometryType esriGeometryType)
    {
      // ReSharper disable UseObjectOrCollectionInitializer
      IGeometryDefEdit geometryDefEdit = new GeometryDefClass();
      geometryDefEdit.GeometryType_2 = esriGeometryType;
      geometryDefEdit.SpatialReference_2 = spatialReference;

      IFieldEdit fieldEdit = new FieldClass();
      fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
      fieldEdit.Name_2 = fieldName;
      fieldEdit.AliasName_2 = fieldName;
      fieldEdit.GeometryDef_2 = geometryDefEdit;
      fieldsEdit.AddField(fieldEdit);
      return fieldsEdit;
      // ReSharper restore UseObjectOrCollectionInitializer
    }

    private ILayer GetLayer()
    {
      ILayer result = null;
      IMap map = ArcUtils.Map;

      if (map != null)
      {
        // ReSharper disable UseIndexedProperty
        var layers = map.get_Layers();
        bool leave = false;
        ILayer layer;

        while (((layer = layers.Next()) != null) && (!leave))
        {
          if (layer.Name == Name)
          {
            if (layer is IFeatureLayer)
            {
              result = layer;
              leave = true;
            }
            else
            {
              _cycloMediaGroupLayer.GroupLayer.Delete(layer);
            }
          }
        }

        // ReSharper restore UseIndexedProperty
      }

      return result;
    }

    /// <summary>
    /// Saves the spatial and attribute data to a geodatabase.
    /// </summary>
    /// <param name="featureMemberElements">The root node of the feature element list</param>
    /// <param name="envelope"></param>
    private void SaveFeatureMembers(IEnumerable<XElement> featureMemberElements, IEnvelope envelope)
    {
      // ReSharper disable PossibleMultipleEnumeration

      if ((featureMemberElements != null) && featureMemberElements.Any())
      {
        if (FeatureClass == null)
        {
          IMappedFeature feature = CreateMappedFeature(null);
          XName name = feature.Name;

          foreach (var featureMemberElement in featureMemberElements)
          {
            IEnumerable<XElement> mappedFeatureElements = featureMemberElement.Descendants(name);

            foreach (var mappedFeatureElement in mappedFeatureElements)
            {
              CreateMappedFeature(mappedFeatureElement);
            }
          }

          if (FeatureClass != null)
          {
            var featureLayer = _layer as FeatureLayer;

            if (featureLayer != null)
            {
              featureLayer.FeatureClass = FeatureClass;
            }
          }
        }

        IMappedFeature mapFeature = CreateMappedFeature(null);
        var fieldIds = mapFeature.Fields.ToDictionary(field => FeatureClass.FindField(field.Key), field => field.Key);
        string idField = mapFeature.ObjectId;
        XName objectName = mapFeature.Name;
        IFeatureWorkspace featureWorkspace = _cycloMediaGroupLayer.FeatureWorkspace;
        var workspaceEdit = featureWorkspace as IWorkspaceEdit;

        if (workspaceEdit != null)
        {
          ISpatialFilter spatialFilter = new SpatialFilterClass
                                           {
                                             Geometry = envelope,
                                             GeometryField = FeatureClass.ShapeFieldName,
                                             SpatialRel = esriSpatialRelEnum.esriSpatialRelContains,
                                             SubFields = idField
                                           };

          workspaceEdit.StartEditing(false);
          var existsResult = FeatureClass.Search(spatialFilter, false);
          var existsCount = FeatureClass.FeatureCount(spatialFilter);
          var exists = new Dictionary<string, IFeature>();
          var imId = existsResult.FindField(idField);
          // ReSharper disable UseIndexedProperty

          for (int i = 0; i < existsCount; i++)
          {
            IFeature feature = existsResult.NextFeature();
            var recValue = feature.get_Value(imId) as string;

            if ((recValue != null) && (!exists.ContainsKey(recValue)))
            {
              exists.Add(recValue, feature);
            }
          }

          foreach (XElement featureMemberElement in featureMemberElements)
          {
            IEnumerable<XElement> mappedFeatureElements = featureMemberElement.Descendants(objectName);

            foreach (var mappedFeatureElement in mappedFeatureElements)
            {
              IMappedFeature mappedFeature = CreateMappedFeature(mappedFeatureElement);

              if (mappedFeature.Shape != null)
              {
                if (mappedFeature.Shape.Type == ShapeType.Point)
                {
                  if (!exists.ContainsKey((string) mappedFeature.FieldToItem(idField)))
                  {
                    var shapePoint = mappedFeature.Shape as ShapePoint;

                    if ((shapePoint != null) && Filter(mappedFeature))
                    {
                      IPoint point = new Point();
                      point.PutCoords(shapePoint.X, shapePoint.Y);
                      IFeature feature = FeatureClass.CreateFeature();
                      feature.Shape = point;

                      foreach (var fieldId in fieldIds)
                      {
                        feature.set_Value(fieldId.Key, mappedFeature.FieldToItem(fieldId.Value));
                      }

                      feature.Store();
                    }
                  }
                  else
                  {
                    if (Filter(mappedFeature))
                    {
                      exists.Remove((string) mappedFeature.FieldToItem(idField));
                    }
                  }
                }
              }
            }
          }

          foreach (var feature in exists)
          {
            feature.Value.Delete();
          }

          workspaceEdit.StopEditing(true);
          // ReSharper restore UseIndexedProperty
        }
      }

      // ReSharper restore PossibleMultipleEnumeration
    }

    private void OnViewRefreshed(IActiveView view, esriViewDrawPhase phase, object data, IEnvelope envelope)
    {
      try
      {
        if (InsideScale)
        {
          IActiveView activeView = ArcUtils.ActiveView;

          if ((activeView != null) && (_layer != null) && (_cycloMediaGroupLayer != null))
          {
            const double epsilon = 0.0;
            var extent = activeView.Extent;

            if ((_addData != null) && (_addData.Count >= 1))
            {
              SaveFeatureMembers(_addData, _lastextent);
              _addData.Clear();
            }

            PostEntryStep();

            if (((Math.Abs(extent.XMax - _lastextent.XMax) > epsilon) ||
                 (Math.Abs(extent.YMin - _lastextent.YMin) > epsilon)
                 || (Math.Abs(extent.XMin - _lastextent.XMin) > epsilon) ||
                 (Math.Abs(extent.YMax - _lastextent.YMax) > epsilon)))
            {
              if ((_addData == null) || (_addData.Count == 0))
              {
                if ((_getDataThread == null) || (!_getDataThread.IsAlive))
                {
                  _lastextent = extent;
                  _getDataThread = new Thread(GetDataWfs);
                  _getDataThread.Start(extent);
                }
              }
            }
          }
        }
        else
        {
          if (_addData != null)
          {
            _addData.Clear();
          }

          if (YearMonth.Count >= 1)
          {
            YearMonth.Clear();
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "CycloMediaLayer.OnViewRefreshed");
      }
    }

    private void OnContentChanged()
    {
      try
      {
        Color = ArcUtils.GetColorFromLayer(_layer);

        if (LayerChangedEvent != null)
        {
          LayerChangedEvent(this);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "CycloMediaLayer.OnContentChanged");
      }
    }

    private void OnItemDeleted(object item)
    {
      try
      {
        if (item != null)
        {
          var featureLayer = item as ILayer;

          if (featureLayer != null)
          {
            if (_layer == featureLayer)
            {
              _cycloMediaGroupLayer.Layers.Remove(this);
              Remove();
              _layer = null;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "CycloMediaLayer.OnItemDeleted");
      }
    }

    private void GetDataWfs(object context)
    {
      try
      {
        lock (_getDataLock)
        {
          var extent = context as IEnvelope;

          if (extent != null)
          {
            ClientWeb webClient = ClientWeb.Instance;
            _addData = webClient.GetByBbox(extent, this);

            if (_addData.Count >= 1)
            {
              _refreshDataThread = new Thread(RefreshDataWfs);
              _refreshDataThread.Start();
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "CycloMediaLayer.GetDataWfs");
      }
    }

    private void RefreshDataWfs()
    {
      try
      {
        lock (_getDataLock)
        {
          IActiveView activeView = ArcUtils.ActiveView;

          if (activeView != null)
          {
            activeView.PartialRefresh(esriViewDrawPhase.esriViewForeground, Layer, null);
          }
        }

        IActiveView view = ArcUtils.ActiveView;
        IScreenDisplay display = view.ScreenDisplay;
        display.StartDrawing(display.hDC, (short)esriScreenCache.esriNoScreenCache);
        ITrackCancel cancel = new TrackCancelClass();
        Layer.Draw(esriDrawPhase.esriDPGeography, display, cancel);
        display.FinishDrawing();
        Viewer.Redraw();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "CycloMediaLayer.RefreshDataWfs");
      }
    }
  }
}
