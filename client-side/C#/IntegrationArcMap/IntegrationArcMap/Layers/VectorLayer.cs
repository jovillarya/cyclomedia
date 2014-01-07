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

  public delegate void FeatureStartEditDelegate(IEnumFeature features);
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
        OnLayerChanged(this);
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
        return (sameFactoryCode || zCoord) && GeometryDef.HasZ;
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
      }

      if (editEvents != null)
      {
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
            result = string.Format("{0}<gml:featureMember><xs:Geometry>{1}</xs:Geometry></gml:featureMember>", result,
                                   gml);
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
                                          IsVisibleInGlobespotter = true
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
        if (FeatureUpdateEditEvent != null)
        {
          var feature = obj as IFeature;

          if (feature != null)
          {
            if (!EditFeatures.Contains(feature))
            {
              EditFeatures.Add(feature);
            }

            FeatureUpdateEditEvent(feature);
            AvContentChanged();
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.OnChangeFeature");
      }
    }

    private static void OnSelectionChanged()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        IEnumFeature editSelection = editor.EditSelection;
        editSelection.Reset();
        EditFeatures.Clear();
        IFeature feature;

        while ((feature = editSelection.Next()) != null)
        {
          EditFeatures.Add(feature);
        }

        if (FeatureStartEditEvent != null)
        {
          FeatureStartEditEvent(editSelection);
          AvContentChanged();
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.OnSelectionChanged");
      }
    }

    private static void OnDeleteFeature(IObject obj)
    {
      try
      {
        if (FeatureDeleteEvent != null)
        {
          var feature = obj as IFeature;

          if (feature != null)
          {
            if (EditFeatures.Contains(feature))
            {
              EditFeatures.Remove(feature);
            }

            FeatureDeleteEvent(feature);
            AvContentChanged();
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.OnDeleteFeature");
      }
    }

    private static void OnStopEditing(bool save)
    {
      try
      {
        EditFeatures.Clear();

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
        Trace.WriteLine(ex.Message, "VectorLayer.OnStopEditing");
      }
    }

    private static void OnSketchModified()
    {
      try
      {
        if (_editToolCheckTimer == null)
        {
          var checkEvent = new AutoResetEvent(true);
          var checkTimerCallBack = new TimerCallback(EditToolCheck);
          const int checkTime = 1000;
          _editToolCheckTimer = new Timer(checkTimerCallBack, checkEvent, checkTime, checkTime);
        }

        IEditor3 editor = ArcUtils.Editor;
        var sketch = editor as IEditSketch3;

        if (sketch != null)
        {
          IGeometry geometry = sketch.Geometry;
          IPoint lastPoint = sketch.LastPoint;
          IEditTask task = editor.CurrentTask;
          // ReSharper disable CompareOfFloatsByEqualityOperator

          if (task != null)
          {
            string name = task.Name;

            if ((name == "Create New Feature") || (name == "Reshape Feature"))
            {
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
        Trace.WriteLine(ex.Message, "VectorLayer.OnSketchModified");
      }
    }

    private static void OnSketchFinished()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        IEditTask task = editor.CurrentTask;

        if (task != null)
        {
          string name = task.Name;

          if ((name == "Create New Feature") || (name == "Reshape Feature"))
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
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "VectorLayer.OnSketchFinished");
      }
    }

    private static void OnCurrentTaskChanged()
    {
      IEditor3 editor = ArcUtils.Editor;
      IEditTask task = editor.CurrentTask;
      var sketch = editor as IEditSketch3;

      if ((sketch != null) && (task != null))
      {
        IGeometry geometry = sketch.Geometry;
        string name = task.Name;

        if (name == "Modify Feature")
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

    public static void OnVertexSelectionChanged()
    {
      IEditor3 editor = ArcUtils.Editor;
      var sketch = editor as IEditSketch3;

      if (sketch != null)
      {
        IGeometry geometry = sketch.Geometry;
        Measurement measurement = Measurement.Get(geometry, false);

        if (measurement != null)
        {
          measurement.CheckSelectedVertex();
        }
      }
    }

    #endregion

    #region thread functions

    // =========================================================================
    // Thread functions
    // =========================================================================
    private static void EditToolCheck(object context)
    {
      IApplication application = ArcMap.Application;
      ICommandItem tool = application.CurrentTool;

      if ((tool != null) && (tool != _beforeTool))
      {
        _beforeTool = tool;
        ICommand command = tool.Command;
        string category = tool.Category;

        if (!FrmMeasurement.IsPointOpen())
        {
          if (((command is IEditTool) || (category != "Editor")) && (category != "CycloMedia"))
          {
            OnSketchFinished();
          }
        }
        else
        {
          if ((!(command is IEditTool)) && (category == "Editor"))
          {
            FrmMeasurement.DoCloseMeasurementPoint();
          }
        }

        if (category == "Editor")
        {
          IEditor3 editor = ArcUtils.Editor;
          var sketch = editor as IEditSketch3;

          if (sketch != null)
          {
            IGeometry geometry = sketch.Geometry;

            if ((editor.EditState != esriEditState.esriStateNotEditing) && (StartMeasurementEvent != null))
            {
              StartMeasurementEvent(geometry);
            }
          }
        }
      }
    }

    #endregion
  }
}
