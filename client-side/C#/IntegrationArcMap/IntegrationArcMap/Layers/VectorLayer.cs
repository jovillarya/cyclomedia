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
using System.Linq;
using System.Threading;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Client;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Objects;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.TrackingAnalyst;
using ESRI.ArcGIS.SystemUI;
using GlobeSpotterAPI;
using MeasurementPoint = IntegrationArcMap.Objects.MeasurementPoint;

namespace IntegrationArcMap.Layers
{
  // ===========================================================================
  // Delegates
  // ===========================================================================
  public delegate void VectorLayerAddDelegate(VectorLayer layer);
  public delegate void VectorLayerChangedDelegate(VectorLayer layer);
  public delegate void VectorLayerRemoveDelegate(VectorLayer layer);

  public delegate void FeatureStartEditDelegate(IList<IGeometry> geometries);
  public delegate void FeatureUpdateEditDelegate(IFeature feature);
  public delegate void FeatureDeleteDelegate(IFeature feature);

  public delegate void StopEditDelegate();
  public delegate void StartMeasurementDelegate(IGeometry geometry);

  public delegate void SketchCreateDelegate(IEditSketch3 sketch);
  public delegate void SketchModifiedDelegate(IGeometry geometry);
  public delegate void SketchFinishedDelegate();

  // ===========================================================================
  // Type of layer
  // ===========================================================================
  public enum TypeOfLayer
  {
    None = 0,
    Point = 1,
    Line = 2,
    Polygon = 3
  }

  public class VectorLayer
  {
    #region constants

    // =========================================================================
    // Constants
    // =========================================================================
    private const string WfsHeader =
      "<wfs:FeatureCollection xmlns:xs=\"http://www.w3.org/2001/XMLSchema\" xmlns:wfs=\"http://www.opengis.net/wfs\" xmlns:gml=\"http://www.opengis.net/gml\">";
    private const string WfsFinished = "</wfs:FeatureCollection>";

    #endregion

    #region members

    // =========================================================================
    // Members
    // =========================================================================
    public static event VectorLayerAddDelegate LayerAddEvent;
    public static event VectorLayerRemoveDelegate LayerRemoveEvent;
    public static event VectorLayerChangedDelegate LayerChangedEvent;

    public static event FeatureStartEditDelegate FeatureStartEditEvent;
    public static event FeatureUpdateEditDelegate FeatureUpdateEditEvent;
    public static event FeatureDeleteDelegate FeatureDeleteEvent;

    public static event StopEditDelegate StopEditEvent;
    public static event StartMeasurementDelegate StartMeasurementEvent;

    public static event SketchCreateDelegate SketchCreateEvent;
    public static event SketchModifiedDelegate SketchModifiedEvent;
    public static event SketchFinishedDelegate SketchFinishedEvent;

    private static IList<IFeature> _editFeatures;
    private static IList<VectorLayer> _layers;
    private static Timer _editToolCheckTimer;
    private static ICommandItem _beforeTool;
    private static readonly LogClient LogClient;
    private static readonly object LockObject;
    private static bool _doSelection;
    private static Timer _nextSelectionTimer;

    private IFeatureClass _featureClass;
    private ILayer _layer;
    private bool _isVisibleInGlobespotter;
    private string _gml;
    private Color _color;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static VectorLayer()
    {
      _editToolCheckTimer = null;
      _beforeTool = null;
      LogClient = new LogClient(typeof(VectorLayer));
      LockObject = new object();
      _doSelection = true;
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public bool GmlChanged { get; private set; }

    public string Name
    {
      get { return (_layer == null) ? string.Empty : _layer.Name; }
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
        StoredLayers.Instance.Update(Name, value);
        OnLayerChanged(this);
        IEditor3 editor = ArcUtils.Editor;

        if (editor != null)
        {
          var editLayers = editor as IEditLayers;

          if (editLayers != null)
          {
            ILayer currentLayer = editLayers.CurrentLayer;

            if (currentLayer != null)
            {
              VectorLayer vectorLayer = GetLayer(currentLayer);

              if (vectorLayer == this)
              {
                if (value)
                {
                  OnSketchModified();
                }
                else
                {
                  if (CheckEditTask())
                  {
                    if (SketchFinishedEvent != null)
                    {
                      SketchFinishedEvent();
                      AvContentChanged();
                    }

                    OnSelectionChanged();
                  }
                }
              }
            }

            foreach (var editFeature in EditFeatures)
            {
              VectorLayer vectorLayer = GetLayer(editFeature);

              if (vectorLayer == this)
              {
                if (value)
                {
                  OnSelectionChanged();
                  var sketch = editor as IEditSketch3;

                  if (sketch != null)
                  {
                    StartMeasurementEvent(sketch.Geometry);
                  }
                }
                else
                {
                  FeatureDeleteEvent(editFeature);
                }
              }
            }
          }
        }
      }
    }

    public TypeOfLayer TypeOfLayer
    {
      get
      {
        TypeOfLayer result;

        switch (_featureClass.ShapeType)
        {
          case esriGeometryType.esriGeometryPoint:
            result = TypeOfLayer.Point;
            break;
          case esriGeometryType.esriGeometryPolyline:
            result = TypeOfLayer.Line;
            break;
          case esriGeometryType.esriGeometryPolygon:
            result = TypeOfLayer.Polygon;
            break;
          default:
            result = TypeOfLayer.None;
            break;
        }

        return result;
      }
    }

    public IGeometryDef GeometryDef
    {
      get
      {
        // ReSharper disable UseIndexedProperty
        IFields fields = _featureClass.Fields;
        int fieldId = fields.FindField(_featureClass.ShapeFieldName);
        IField field = fields.get_Field(fieldId);
        return field.GeometryDef;
        // ReSharper restore UseIndexedProperty
      }
    }

    public bool HasZ
    {
      get
      {
        Config config = Config.Instance;
        SpatialReference spatRel = config.SpatialReference;
        ISpatialReference gsSpatialReference = (spatRel == null) ? ArcUtils.SpatialReference : spatRel.SpatialRef;
        bool zCoord = gsSpatialReference.ZCoordinateUnit != null;
        bool sameFactoryCode = SpatialReference.FactoryCode == gsSpatialReference.FactoryCode;
//        return (sameFactoryCode || zCoord) && GeometryDef.HasZ;
        return ((spatRel == null) || spatRel.CanMeasuring) && GeometryDef.HasZ;
      }
    }

    public ISpatialReference SpatialReference
    {
      get { return GeometryDef.SpatialReference; }
    }

    public static IList<IFeature> EditFeatures
    {
      get { return _editFeatures ?? (_editFeatures = new List<IFeature>()); }
    }

    public static IList<VectorLayer> Layers
    {
      get { return _layers ?? (_layers = DetectVectorLayers(true)); }
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static VectorLayer GetLayer(ILayer layer)
    {
      return Layers.Aggregate<VectorLayer, VectorLayer>(null,
                                                        (current, layerCheck) =>
                                                        (layerCheck._layer == layer) ? layerCheck : current);
    }

    public static VectorLayer GetLayer(IFeatureClass featureClass)
    {
      return Layers.Aggregate<VectorLayer, VectorLayer>(null,
                                                        (current, layerCheck) =>
                                                        (layerCheck._featureClass == featureClass)
                                                          ? layerCheck
                                                          : current);
    }

    public static VectorLayer GetLayer(IFeature feature)
    {
      VectorLayer result = null;

      if (feature != null)
      {
        var featureClass = feature.Class as IFeatureClass;

        if (featureClass != null)
        {
          result = GetLayer(featureClass);
        }
      }

      return result;
    }

    private static IList<VectorLayer> DetectVectorLayers(bool initEvents)
    {
      _layers = new List<VectorLayer>();
      IMap map = ArcUtils.Map;

      if (map != null)
      {
        // ReSharper disable UseIndexedProperty
        var layers = map.get_Layers();
        ILayer layer;

        while ((layer = layers.Next()) != null)
        {
          AvItemAdded(layer);
        }

        // ReSharper restore UseIndexedProperty
      }

      var editor = ArcUtils.Editor;

      if (editor.EditState != esriEditState.esriStateNotEditing)
      {
        OnSelectionChanged();
      }

      if (initEvents)
      {        
        AddEvents();
        var docEvents = ArcUtils.MxDocumentEvents;

        if (docEvents != null)
        {
          docEvents.OpenDocument += OpenDocument;
          docEvents.CloseDocument += CloseDocument;
        }
      }

      return _layers;
    }

    private static void AddEvents()
    {
      var avEvents = ArcUtils.ActiveViewEvents;
      var editEvents = ArcUtils.EditorEvents;
      var editEvents5 = ArcUtils.EditorEvents5;

      if (avEvents != null)
      {
        avEvents.ItemAdded += AvItemAdded;
        avEvents.ItemDeleted += AvItemDeleted;
        avEvents.ContentsChanged += AvContentChanged;
        avEvents.ViewRefreshed += AvViewRefreshed;
      }

      if (editEvents != null)
      {
        editEvents.OnStartEditing += OnStartEditing;
        editEvents.OnChangeFeature += OnChangeFeature;
        editEvents.OnSelectionChanged += OnSelectionChanged;
        editEvents.OnStopEditing += OnStopEditing;
        editEvents.OnDeleteFeature += OnDeleteFeature;
        editEvents.OnSketchModified += OnSketchModified;
        editEvents.OnSketchFinished += OnSketchFinished;
        editEvents.OnCurrentTaskChanged += OnCurrentTaskChanged;
      }

      if (editEvents5 != null)
      {
        editEvents5.OnVertexSelectionChanged += OnVertexSelectionChanged;
      }
    }

    private static bool CheckEditTask()
    {
      bool result = false;
      IEditor3 editor = ArcUtils.Editor;

      if (editor != null)
      {
        IEditTask task = editor.CurrentTask;

        if (task != null)
        {
          var uniqueName = task as IEditTaskName;          

          if (uniqueName != null)
          {
            string name = uniqueName.UniqueName;
            result = (name == "GarciaUI_CreateNewFeatureTask") || (name == "GarciaUI_ReshapeFeatureTask");
          }
        }
      }

      return result;
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public string GetGmlFromLocation(List<RecordingLocation> recordingLocations, double distance, out Color color, SpatialReference cyclSpatialRef)
    {
      string result = WfsHeader;
      // ReSharper disable UseIndexedProperty

      if (_featureClass != null)
      {
        IGeometry geometryBag = new GeometryBagClass();
        var geometryCollection = geometryBag as IGeometryCollection;
        Config config = Config.Instance;
        SpatialReference spatRel = config.SpatialReference;
        ISpatialReference gsSpatialReference = (spatRel == null) ? ArcUtils.SpatialReference : spatRel.SpatialRef;
        var projCoord = gsSpatialReference as IProjectedCoordinateSystem;

        if (projCoord == null)
        {
          var geoCoord = gsSpatialReference as IGeographicCoordinateSystem;

          if (geoCoord != null)
          {
            IAngularUnit unit = geoCoord.CoordinateUnit;
            double factor = unit.ConversionFactor;
            distance = distance*factor;
          }
        }
        else
        {
          ILinearUnit unit = projCoord.CoordinateUnit;
          double factor = unit.ConversionFactor;
          distance = distance/factor;
        }

        foreach (var recordingLocation in recordingLocations)
        {
          double x = recordingLocation.X;
          double y = recordingLocation.Y;

          IEnvelope envelope = new EnvelopeClass
            {
              XMin = x - distance,
              XMax = x + distance,
              YMin = y - distance,
              YMax = y + distance,
              SpatialReference = gsSpatialReference
            };

          envelope.Project(SpatialReference);
          geometryCollection.AddGeometry(envelope);
        }

        ITopologicalOperator unionedPolygon = new PolygonClass();
        unionedPolygon.ConstructUnion(geometryBag as IEnumGeometry);
        var polygon = unionedPolygon as IPolygon;

        ISpatialFilter spatialFilter = new SpatialFilterClass
          {
            Geometry = polygon,
            GeometryField = _featureClass.ShapeFieldName,
            SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects
          };

        var featureCursor = _featureClass.Search(spatialFilter, false);
        var featureCount = _featureClass.FeatureCount(spatialFilter);
        var shapeId = featureCursor.FindField(_featureClass.ShapeFieldName);
        var gmlConverter = new GMLConverter();

        for (int i = 0; i < featureCount; i++)
        {
          IFeature feature = featureCursor.NextFeature();

          if (!EditFeatures.Contains(feature))
          {
            IFields fields = feature.Fields;
            var fieldvalues = new Dictionary<string, string> {{"FEATURECLASSNAME", _featureClass.AliasName}};

            for (int j = 0; j < fields.FieldCount; j++)
            {
              IField field = fields.Field[j];
              string name = field.Name;
              int id = featureCursor.FindField(name);

              string value = (id != shapeId)
                ? feature.get_Value(id).ToString()
                : _featureClass.ShapeType.ToString().Replace("esriGeometry", string.Empty);
              fieldvalues.Add(name, value);
            }

            var shapeVar = feature.get_Value(shapeId);
            var geometry = shapeVar as IGeometry;

            if (geometry != null)
            {
              geometry.Project((cyclSpatialRef == null) ? gsSpatialReference : cyclSpatialRef.SpatialRef);

              if (!HasZ)
              {
                var pointCollection = geometry as IPointCollection4;

                if (pointCollection != null)
                {
                  for (int j = 0; j < pointCollection.PointCount; j++)
                  {
                    IPoint point = pointCollection.Point[j];

                    if (point != null)
                    {
                      point.Z = double.NaN;
                    }

                    pointCollection.ReplacePoints(j, 1, 1, point);
                  }

                  shapeVar = pointCollection as IGeometry;
                }
                else
                {
                  var point = geometry as IPoint;

                  if (point != null)
                  {
                    point.Z = double.NaN;
                    shapeVar = point;
                  }
                }
              }
            }

            gmlConverter.ESRIGeometry = shapeVar;
            string gml = gmlConverter.GML;
            gml = gml.Replace("<Polygon>", string.Format("<Polygon srsDimension=\"{0}\" >", HasZ ? 3 : 2));
            gml = gml.Replace("<LineString>", string.Format("<LineString srsDimension=\"{0}\" >", HasZ ? 3 : 2));
            gml = gml.Replace("<point>", string.Format("<point srsDimension=\"{0}\" >", HasZ ? 3 : 2));
            gml = gml.Replace("point", "Point");
            gml = gml.Replace(",1.#QNAN", string.Empty);
            gml = gml.Replace("<", "<gml:");
            gml = gml.Replace("<gml:/", "</gml:");
            string fieldValueStr = fieldvalues.Aggregate(string.Empty,
              (current, fieldvalue) => string.Format("{0}<{1}>{2}</{1}>", current, fieldvalue.Key, fieldvalue.Value));
            result = string.Format("{0}<gml:featureMember><xs:Geometry>{1}{2}</xs:Geometry></gml:featureMember>", result,
              fieldValueStr, gml);
          }
        }
      }

      // ReSharper restore UseIndexedProperty
      color = ArcUtils.GetColorFromLayer(_layer);
      GmlChanged = (_color != color);
      _color = color;
      string newGml = string.Concat(result, WfsFinished);
      GmlChanged = ((newGml != _gml) || GmlChanged);
      return (_gml = newGml);
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Event handlers
    // =========================================================================
    private static void OpenDocument()
    {
      try
      {
        DetectVectorLayers(false);
        AddEvents();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.OpenDocument");
      }
    }

    private static void CloseDocument()
    {
      try
      {
        var avEvents = ArcUtils.ActiveViewEvents;

        if (avEvents != null)
        {
          avEvents.ItemAdded -= AvItemAdded;
          avEvents.ItemDeleted -= AvItemDeleted;
          avEvents.ContentsChanged -= AvContentChanged;
        }

        while (Layers.Count >= 1)
        {
          AvItemDeleted(Layers[0]);
          Layers.RemoveAt(0);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.CloseDocument");
      }
    }

    private static void AvItemAdded(object item)
    {
      try
      {
        if (item != null)
        {
          var featureLayer = item as IFeatureLayer;
          var extension = GsExtension.GetExtension();

          if ((featureLayer != null) && (extension != null))
          {
            CycloMediaGroupLayer cycloGrouplayer = extension.CycloMediaGroupLayer;

            if (cycloGrouplayer != null)
            {
              if (!cycloGrouplayer.IsKnownName(featureLayer.Name))
              {
                var dataset = item as IDataset;

                if (dataset != null)
                {
                  var featureWorkspace = dataset.Workspace as IFeatureWorkspace;

                  if (featureWorkspace != null)
                  {
                    var featureClass = featureLayer.FeatureClass;

                    var vectorLayer = new VectorLayer
                                        {
                                          _featureClass = featureClass,
                                          _layer = featureLayer,
                                          IsVisibleInGlobespotter = StoredLayers.Instance.Get(featureLayer.Name)
                                        };

                    _layers.Add(vectorLayer);

                    if (LayerAddEvent != null)
                    {
                      LayerAddEvent(vectorLayer);
                    }
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.AvItemAdded");
      }
    }

    private static void AvItemDeleted(object item)
    {
      try
      {
        if (item != null)
        {
          var featureLayer = item as IFeatureLayer;

          if (featureLayer != null)
          {
            int i = 0;

            while (Layers.Count > i)
            {
              if (Layers[i]._layer == featureLayer)
              {
                if (LayerRemoveEvent != null)
                {
                  LayerRemoveEvent(Layers[i]);
                }

                Layers.RemoveAt(i);
              }
              else
              {
                i++;
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.AvItemDeleted");
      }
    }

    private static void AvContentChanged()
    {
      OnLayerChanged(null);

      _layers = new List<VectorLayer>();
      IMap map = ArcUtils.Map;

      if (map != null)
      {
        // ReSharper disable UseIndexedProperty
        var layers = map.get_Layers();
        ILayer layer;

        while ((layer = layers.Next()) != null)
        {
          AvItemAdded(layer);
        }

        // ReSharper restore UseIndexedProperty
      }
    }

    private static void OnStartEditing()
    {
      IEditor3 editor = ArcUtils.Editor;
      LogClient.Info("On StartEditing");

      if (editor != null)
      {
        IMap map = editor.Map;

        if (map != null)
        {
          map.ClearSelection();
        }
      }
    }

    private static void OnLayerChanged(VectorLayer layer)
    {
      try
      {
        if (LayerChangedEvent != null)
        {
          LayerChangedEvent(layer);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.AvContentChanged");
      }
    }

    private static void OnChangeFeature(IObject obj)
    {
      try
      {
        var feature = obj as IFeature;
        LogClient.Info(string.Format("On Change Feature: {0}", ((feature != null ) ? feature.Class.AliasName:string.Empty)));

        if ((FeatureUpdateEditEvent != null) && (feature != null))
        {
          if (!EditFeatures.Contains(feature))
          {
            EditFeatures.Add(feature);
          }

          VectorLayer vectorLayer = GetLayer(feature);

          if ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter))
          {
            FeatureUpdateEditEvent(feature);
            AvContentChanged();
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnChangeFeature", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnChangeFeature");
      }
    }

    private static void OnSelectionChanged()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        LogClient.Info("On Selection Changed");

        if (editor != null)
        {
          IEnumFeature editSelection = editor.EditSelection;

          if (editSelection != null)
          {
            editSelection.Reset();
            EditFeatures.Clear();
            IFeature feature;

            while ((feature = editSelection.Next()) != null)
            {
              EditFeatures.Add(feature);
            }

            if (FeatureStartEditEvent != null)
            {
              var geometries = new List<IGeometry>();
              editSelection.Reset();
              bool isPointLayer = false;

              while ((feature = editSelection.Next()) != null)
              {
                VectorLayer vectorLayer = GetLayer(feature);

                if ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter))
                {
                  geometries.Add(feature.Shape);
                  isPointLayer = isPointLayer || (Measurement.GetTypeOfLayer(feature.Shape) == TypeOfLayer.Point);
                }
              }

              if (_doSelection || (!isPointLayer))
              {
                FeatureStartEditEvent(geometries);
                AvContentChanged();

                _doSelection = false;

                if (_nextSelectionTimer == null)
                {
                  var checkEvent = new AutoResetEvent(true);
                  const int timeOutTime = 1500;

                  var checkTimerCallBack = new TimerCallback(state =>
                  {
                    _doSelection = true;
                    _nextSelectionTimer = null;
                  });

                  _nextSelectionTimer = new Timer(checkTimerCallBack, checkEvent, timeOutTime, -1);
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnSelectionChanged", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnSelectionChanged");
      }
    }

    private static void OnDeleteFeature(IObject obj)
    {
      try
      {
        var feature = obj as IFeature;
        LogClient.Info(string.Format("On Delete Feature: {0}", ((feature != null) ? feature.Class.AliasName : string.Empty)));

        if ((FeatureDeleteEvent != null) && (feature != null))
        {
          if (EditFeatures.Contains(feature))
          {
            EditFeatures.Remove(feature);
          }

          VectorLayer vectorLayer = GetLayer(feature);

          if ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter))
          {
            FeatureDeleteEvent(feature);
            AvContentChanged();
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnDeleteFeature", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnDeleteFeature");
      }
    }

    private static void OnStopEditing(bool save)
    {
      try
      {
        LogClient.Info(string.Format("On Stop Editing: {0}", save));
        EditFeatures.Clear();
        _doSelection = true;

        if (StopEditEvent != null)
        {
          StopEditEvent();
          AvContentChanged();
        }

        if (_editToolCheckTimer != null)
        {
          _editToolCheckTimer.Dispose();
          _editToolCheckTimer = null;
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnStopEditing", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnStopEditing");
      }
    }

    private static void OnSketchModified()
    {
      try
      {
        LogClient.Info("On Sketch Modified");
        _doSelection = true;

        if (_editToolCheckTimer == null)
        {
          var checkEvent = new AutoResetEvent(true);
          var checkTimerCallBack = new TimerCallback(EditToolCheck);
          const int checkTime = 1000;
          _editToolCheckTimer = new Timer(checkTimerCallBack, checkEvent, checkTime, checkTime);
        }

        IEditor3 editor = ArcUtils.Editor;

        if (editor != null)
        {
          var sketch = editor as IEditSketch3;
          var editLayers = editor as IEditLayers;

          if ((sketch != null) && (editLayers != null))
          {
            ILayer currentLayer = editLayers.CurrentLayer;
            VectorLayer vectorLayer = GetLayer(currentLayer);

            if ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter) && CheckEditTask())
            {
              IGeometry geometry = sketch.Geometry;
              IPoint lastPoint = sketch.LastPoint;
              // ReSharper disable CompareOfFloatsByEqualityOperator

              if (lastPoint != null)
              {
                if ((lastPoint.Z == 0) && (SketchCreateEvent != null) && sketch.ZAware)
                {
                  SketchCreateEvent(sketch);
                }
              }
              else
              {
                if ((editor.EditState != esriEditState.esriStateNotEditing) && (StartMeasurementEvent != null))
                {
                  StartMeasurementEvent(geometry);
                }
              }

              // ReSharper restore CompareOfFloatsByEqualityOperator
              if (SketchModifiedEvent != null)
              {
                SketchModifiedEvent(geometry);
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnSketchModified", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnSketchModified");
      }
    }

    private static void OnSketchFinished()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        LogClient.Info("On Sketch Finished");
        _doSelection = true;

        if (editor != null)
        {
          var editLayers = editor as IEditLayers;

          if (editLayers != null)
          {
            ILayer currentLayer = editLayers.CurrentLayer;
            VectorLayer vectorLayer = GetLayer(currentLayer);

            if (CheckEditTask() && ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter)))
            {
              if (SketchFinishedEvent != null)
              {
                SketchFinishedEvent();
                AvContentChanged();
              }

              OnSelectionChanged();
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnSketchFinished", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnSketchFinished");
      }
    }

    private static void OnCurrentTaskChanged()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        LogClient.Info("On CurrentTask Changed");
        _doSelection = true;

        if (editor != null)
        {
          var sketch = editor as IEditSketch3;
          var editLayers = editor as IEditLayers;

          if ((sketch != null) && (editLayers != null))
          {
            IEditTask task = editor.CurrentTask;
            ILayer currentLayer = editLayers.CurrentLayer;
            VectorLayer vectorLayer = (EditFeatures.Count != 1)
              ? ((currentLayer == null) ? null : GetLayer(currentLayer))
              : GetLayer(EditFeatures[0]);

            if ((task != null) && ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter)))
            {
              var taskName = task as IEditTaskName;

              if (taskName != null)
              {
                IGeometry geometry = sketch.Geometry;
                string name = taskName.UniqueName;

                if (name == "GarciaUI_ModifyFeatureTask")
                {
                  Measurement measurement = Measurement.Get(geometry, false);

                  if (measurement != null)
                  {
                    int nrPoints;
                    var ptColl = measurement.ToPointCollection(geometry, out nrPoints);

                    if (ptColl != null)
                    {
                      ISketchOperation2 sketchOp = new SketchOperationClass();
                      sketchOp.Start(editor);

                      for (int j = 0; j < nrPoints; j++)
                      {
                        IPoint point = ptColl.Point[j];
                        MeasurementPoint mpoint = measurement.IsPointMeasurement
                          ? measurement.GetPoint(point, false)
                          : measurement.GetPoint(point);

                        double m = (mpoint == null) ? double.NaN : mpoint.M;
                        double z = (mpoint == null) ? double.NaN : mpoint.Z;
                        IPoint point2 = new PointClass {X = point.X, Y = point.Y, Z = z, M = m, ZAware = sketch.ZAware};
                        ptColl.UpdatePoint(j, point2);

                        if (measurement.IsPointMeasurement)
                        {
                          sketch.Geometry = point2;
                        }
                      }

                      if (!measurement.IsPointMeasurement)
                      {
                        sketch.Geometry = ptColl as IGeometry;
                      }

                      geometry = sketch.Geometry;

                      if (geometry != null)
                      {
                        sketchOp.Finish(geometry.Envelope, esriSketchOperationType.esriSketchOperationGeneral, geometry);
                      }
                    }

                    measurement.SetSketch();
                    measurement.OpenMeasurement();
                    measurement.DisableMeasurementSeries();
                  }
                }
                else
                {
                  Measurement measurement = Measurement.Get(geometry, false);

                  if (measurement != null)
                  {
                    measurement.EnableMeasurementSeries();
                  }

                  OnSelectionChanged();
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnCurrentTaskChanged", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnCurrentTaskChanged");
      }
    }

    private static void OnVertexSelectionChanged()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        LogClient.Info("On vertex selection Changed");
        _doSelection = true;

        if (editor != null)
        {
          var sketch = editor as IEditSketch3;
          var editLayers = editor as IEditLayers;

          if ((sketch != null) && (editLayers != null))
          {
            ILayer currentLayer = editLayers.CurrentLayer;
            IGeometry geometry = sketch.Geometry;
            VectorLayer vectorLayer = (EditFeatures.Count != 1)
              ? ((currentLayer == null) ? null : GetLayer(currentLayer))
              : GetLayer(EditFeatures[0]);

            if ((geometry != null) && ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter)))
            {
              Measurement measurement = Measurement.Get(geometry, false);

              if (measurement != null)
              {
                if (!measurement.CheckSelectedVertex())
                {
                  SketchFinishedEvent();
                }
              }
              else
              {
                SketchFinishedEvent();
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        LogClient.Error("VectorLayer.OnVertexSelectionChanged", ex.Message, ex);
        Trace.WriteLine(ex.Message, "VectorLayer.OnVertexSelectionChanged");
      }
    }

    #endregion

    #region thread functions

    // =========================================================================
    // Thread functions
    // =========================================================================
    private static void EditToolCheck(object context)
    {
      try
      {
        lock (LockObject)
        {
          IApplication application = ArcMap.Application;
          IActiveView activeView = ArcUtils.ActiveView;

          if ((application != null) && (activeView != null))
          {
            ICommandItem tool = application.CurrentTool;

            if ((tool != null) && ((_beforeTool == null) || (_beforeTool.Name != tool.Name)))
            {
              activeView.Refresh();
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.OnEditToolCheck");
      }
    }

    private static void AvViewRefreshed(IActiveView view, esriViewDrawPhase phase, object data, IEnvelope envelope)
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        IApplication application = ArcMap.Application;

        if ((editor != null) && (application != null))
        {
          var editLayers = editor as IEditLayers;

          if (editLayers != null)
          {
            ILayer currentLayer = editLayers.CurrentLayer;
            VectorLayer vectorLayer = (currentLayer == null) ? null : GetLayer(currentLayer);
            ICommandItem tool = application.CurrentTool;

            if ((tool != null) && ((_beforeTool == null) || (_beforeTool.Name != tool.Name)))
            {
              _beforeTool = tool;

              if ((vectorLayer != null) && (vectorLayer.IsVisibleInGlobespotter))
              {
                ICommandItem editorMenu = application.Document.CommandBars.Find("Editor_EditTool");
                string editorCategory = (editorMenu != null) ? editorMenu.Category : string.Empty;
                ICommand command = tool.Command;
                string category = tool.Category;

                if (!FrmMeasurement.IsPointOpen())
                {
                  if (((command is IEditTool) || (category != editorCategory)) && (category != "CycloMedia"))
                  {
                    OnSketchFinished();
                  }
                }
                else
                {
                  if ((!(command is IEditTool)) && (category == editorCategory))
                  {
                    FrmMeasurement.DoCloseMeasurementPoint();
                  }
                }

                if (category == editorCategory)
                {
                  var sketch = editor as IEditSketch3;

                  if (sketch != null)
                  {
                    IGeometry geometry = sketch.Geometry;

                    if ((!(command is IEditTool)) && (editor.EditState != esriEditState.esriStateNotEditing) && (StartMeasurementEvent != null))
                    {
                      StartMeasurementEvent(geometry);
                    }
                  }
                }
              }
              else
              {
                SketchFinishedEvent();
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.AvViewRefreshed");
      }
    }

    #endregion
  }
}
