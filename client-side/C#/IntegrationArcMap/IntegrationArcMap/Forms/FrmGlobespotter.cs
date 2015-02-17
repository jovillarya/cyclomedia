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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Client;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Objects;
using IntegrationArcMap.Properties;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using GlobeSpotterAPI;
using MapExtent = GlobeSpotterAPI.MapExtent;
using MeasurementPoint = IntegrationArcMap.Objects.MeasurementPoint;
using GsMeasurementPoint = GlobeSpotterAPI.MeasurementPoint;

namespace IntegrationArcMap.Forms
{
  public partial class FrmGlobespotter : UserControl, IAPIClient
  {
    #region Members

    // =========================================================================
    // Members
    // =========================================================================
    private static FrmGlobespotter _frmGlobespotter;
    private static IDockableWindow _window;
    private readonly IDictionary<VectorLayer, uint> _vectorLayers;
    private readonly IDictionary<WfsLayer, uint?> _wfsLayers;
    private readonly List<int?> _colorYears;
    private readonly List<int> _recentLayers;
    private readonly List<CycloMediaLayer> _visibleLayers;
    private readonly List<string> _restartImages;

    // ReSharper disable InconsistentNaming
    private API _api;
    private string _imageId;
    private CycloMediaLayer _layer;
    private Login _login;
    private Config _config;
    private APIKey _apiKey;
    private ICommandItem _tool;
    private string _measurementName;
    private bool _drawingSketch;
    private bool _sketchModified;
    private bool _screenPointAdded;
    private bool _mapPointAdded;
    private bool _drawPoint;
    private bool _observationAdded;
    private bool _toInitialize;
    private Point3D _lookAtCoord;
    private SortedDictionary<int, int> _yearMonth;
    // ReSharper restore InconsistentNaming

    private readonly IList<Color> _colors = new List<Color>
    {
      Color.FromArgb(0x80B3FF),
      Color.FromArgb(0x0067FF),
      Color.FromArgb(0x405980),
      Color.FromArgb(0x001F4D),
      Color.FromArgb(0xFFD080),
      Color.FromArgb(0xFFA100),
      Color.FromArgb(0x806840),
      Color.FromArgb(0x4D3000),
      Color.FromArgb(0xDDFF80),
      Color.FromArgb(0xBBFF00),
      Color.FromArgb(0x6F8040),
      Color.FromArgb(0x384D00),
      Color.FromArgb(0xFF80D9),
      Color.FromArgb(0xFF00B2),
      Color.FromArgb(0x80406C),
      Color.FromArgb(0x4D0035),
      Color.FromArgb(0xF2F2F2),
      Color.FromArgb(0xBFBFBF),
      Color.FromArgb(0x404040),
      Color.FromArgb(0x000000)
    };

    #endregion

    #region Constructors

    // =========================================================================
    // Constructors
    // =========================================================================
    private FrmGlobespotter()
    {
      InitializeComponent();
      _vectorLayers = new Dictionary<VectorLayer, uint>();
      _wfsLayers = new Dictionary<WfsLayer, uint?>();
      _visibleLayers = new List<CycloMediaLayer>();
      _colorYears = new List<int?>();
      _recentLayers = new List<int>();
      _restartImages = new List<string>();
      _api = null;
      _imageId = String.Empty;
      _layer = null;
      _config = null;
      _login = null;
      _apiKey = null;
      _drawingSketch = false;
      _sketchModified = false;
      _screenPointAdded = false;
      _mapPointAdded = false;
      _measurementName = string.Empty;
      _drawPoint = true;
      _observationAdded = false;
      _yearMonth = null;
      _toInitialize = false;
      _lookAtCoord = null;

      CycloMediaLayer.HistoricalDateChanged += OnHistoricalDateChanged;
      FrmRecordingHistory.DateRangeChanged += OnDateRangeChanged;
    }

    #endregion

    #region Globespotter properties

    // =========================================================================
    // Properties
    // =========================================================================
    private static FrmGlobespotter Instance
    {
      get { return _frmGlobespotter ?? (_frmGlobespotter = new FrmGlobespotter()); }
    }

    public static IntPtr FrmHandle
    {
      get { return Instance.Handle; }
    }

    private static IDockableWindow Window
    {
      get { return _window ?? (_window = GetDocWindow()); }
    }

    private bool ApiReady
    {
      get { return ((_api != null) && _api.GetAPIReadyState()); }
    }

    private bool Started
    {
      get { return (_api != null); }
    }

    #endregion

    #region Globespotter event handlers

    // =========================================================================
    // Globespotter event handlers
    // =========================================================================
    private void FrmGlobespotterLoad(object sender, EventArgs e)
    {
      lock (this)
      {
        if (string.IsNullOrEmpty(_imageId))
        {
          var closeGlobespotter = new Thread(CloseGlobespotter);
          closeGlobespotter.Start();
        }
      }
    }

    private void PlGlobespotterMouseEnter(object sender, EventArgs e)
    {
      IApplication application = ArcMap.Application;
      _tool = application.CurrentTool;
      application.CurrentTool = null;
    }

    private void PlGlobespotterMouseLeave(object sender, EventArgs e)
    {
      IApplication application = ArcMap.Application;
      application.CurrentTool = _tool;
    }

    #endregion

    #region Globespotter private functions

    // =========================================================================
    // Globespotter functions
    // =========================================================================
    private void CloseGlobespotter()
    {
      lock (this)
      {
        ShowGlobespotter(false);
      }
    }

    private void Clear()
    {
      if (_api != null)
      {
        foreach (var layer in _vectorLayers)
        {
          _api.RemoveLayer(layer.Value);
        }

        foreach (var layer in _wfsLayers)
        {
          uint? layerId = layer.Value;

          if (layerId != null)
          {
            _api.RemoveLayer((uint) layerId);
          }
        }

        _vectorLayers.Clear();
        _wfsLayers.Clear();
        CloseGlobespotter();

        VectorLayer.LayerAddEvent -= OnAddVectorLayer;
        VectorLayer.LayerRemoveEvent -= OnRemoveVectorLayer;
        VectorLayer.LayerChangedEvent -= OnRefreshVectorLayer;

        VectorLayer.FeatureStartEditEvent -= OnStartEditFeature;
        VectorLayer.FeatureUpdateEditEvent -= OnUpdateEditFeature;
        VectorLayer.FeatureDeleteEvent -= OnDeleteFeature;

        VectorLayer.StopEditEvent -= OnStopEdit;
        VectorLayer.StartMeasurementEvent -= OnStartMeasurement;

        VectorLayer.SketchCreateEvent -= OnCreateSketch;
        VectorLayer.SketchModifiedEvent -= OnModifiedSketch;
        VectorLayer.SketchFinishedEvent -= OnSketchFinished;

        CycloMediaLayer.LayerAddedEvent -= OnAddCycloMediaLayer;
        CycloMediaLayer.LayerChangedEvent -= OnRefreshCycloMediaLayer;
        CycloMediaLayer.LayerRemoveEvent -= OnRemoveCycloMediaLayer;

        IActiveViewEvents_Event events = ArcUtils.ActiveViewEvents;

        if (events != null)
        {
          events.ViewRefreshed -= OnViewRefreshed;
        }

        if (_api.gui != null)
        {
          if (plGlobespotter.Controls.Contains(_api.gui))
          {
            plGlobespotter.Controls.Remove(_api.gui);
            _api.gui.Dispose();
          }
        }

        _api = null;
      }
    }

    private void ShowLoc(string imageId, CycloMediaLayer layer, bool replace)
    {
      _imageId = imageId;

      if (layer != null)
      {
        _layer = layer;
      }

      ShowGlobespotter(true);

      if (ApiReady)
      {
        OpenImage(replace);
      }
      else
      {
        if (_colorYears.Count == 0)
        {
          Initialize();
        }
      }
    }

    private void UpdateCol(CycloMediaLayer layer, int? year)
    {
      _imageId = string.Empty;
      _layer = layer;

      if (!_colorYears.Contains(year))
      {
        _colorYears.Add(year);
      }

      if ((!layer.UseDateRange) && (year != null))
      {
        _recentLayers.Add((int) year);
      }

      UpdateCol();

      if (!ApiReady)
      {
        if (_colorYears.Count == 1)
        {
          Initialize();
        }
      }
    }

    private void UpdateCol()
    {
      lock (this)
      {
        if (_layer != null)
        {
          while (_colorYears.Count >= 1)
          {
            DateTime? dateTime;
            int? colorYear = _colorYears[0];

            if (colorYear == null)
            {
              dateTime = _layer.GetDate();
            }
            else
            {
              var year = (int) colorYear;
              dateTime = new DateTime(year, 4, 1);
            }

            if (dateTime != null)
            {
              var dateTime2 = (DateTime) dateTime;
              int year = dateTime2.Year;
              Color color = GetCol(year);
              _layer.UpdateColor(color, colorYear);
            }

            _colorYears.Remove(colorYear);
          }
        }
      }
    }

    private void Initialize()
    {
      _config = Config.Instance;
      _login = Login.Instance;
      _apiKey = APIKey.Instance;

      if (_login.Credentials)
      {
        _api = _config.SwfUrlDefault ? new API(InitType.REMOTE) : new API(InitType.REMOTE, _config.SwfUrl);
        plGlobespotter.Controls.Add(_api.gui);
        _api.Initialize(this);
      }
      else
      {
        _toInitialize = true;
      }
    }

    private void OpenImage(bool replace)
    {
      if ((_api != null) && (_layer != null))
      {
        UpdateDateRange();
        uint nrViewers = _api.GetViewerCount();
        _api.SetActiveViewerReplaceMode((nrViewers >= _config.MaxViewers) || replace);
        _api.SetRecordingLocationsVisible(_layer.IsVisibleInGlobespotter);
        _api.OpenImage(_imageId);
      }
    }

    private void UpdateDateRange()
    {
      if (_visibleLayers.Count >= 1)
      {
        bool useDateRange = _visibleLayers.Aggregate(false, (current, visibleLayer) =>
                                                            (visibleLayer.UseDateRange &&
                                                             visibleLayer.IsVisibleInGlobespotter) || current);
        _api.SetUseDateRange(useDateRange);

        if (useDateRange)
        {
          bool dateRange = _visibleLayers.Aggregate(false, (current, visibleLayer) =>
                                                           ((!visibleLayer.UseDateRange) &&
                                                            visibleLayer.IsVisibleInGlobespotter) || current);
          _api.SetDateFrom(String.Format("{0}-{1}-01", _config.YearFrom, _config.MonthFrom));
          int yearTo = _config.YearTo;

          if (dateRange)
          {
            DateTime dateTimeTo = DateTime.Now;
            yearTo = Math.Max((_recentLayers.Concat(new[] {dateTimeTo.Year}).Min()), yearTo);
          }

          _api.SetDateTo(String.Format("{0}-{1}-01", yearTo, _config.MonthTo));
        }
      }
    }

    private void UpdateParam()
    {
      if (ApiReady)
      {
        if (GlobeSpotterConfiguration.MeasureSmartClick)
        {
          _api.SetMeasurementSmartClickModeEnabled(_config.SmartClickEnabled);
        }

        _api.SetViewerDetailImagesVisible(_config.DetailImagesEnabled);
        _api.SetMaxViewers(_config.MaxViewers);
        OnRefreshVectorLayer(null);
      }
    }

    private void AddWfsLay(WfsLayer layer)
    {
      if (GlobeSpotterConfiguration.AddLayerWfs)
      {
        uint? layerId = null;

        if (_api != null)
        {
          layerId = _api.AddWFSLayer(layer.Name, layer.Url, layer.TypeName, layer.Version, layer.Color, true, false,
            layer.MinZoomLevel, layer.UseProxy);
        }

        if (!_wfsLayers.ContainsKey(layer))
        {
          _wfsLayers.Add(layer, layerId);
        }
        else
        {
          _wfsLayers[layer] = layerId;
        }
      }
    }

    public void CloseViewer(uint viewerId)
    {
      if (_api != null)
      {
        _api.CloseImage(viewerId);
      }
    }

    public void LoginSucces()
    {
      if (_toInitialize)
      {
        _toInitialize = false;
        Initialize();
      }
    }

    private Color GetCol(int year)
    {
      DateTime now = DateTime.Now;
      int nowYear = now.Year;
      int yearDiff = nowYear - year;
      int nrColors = _colors.Count;
      int index = Math.Min(yearDiff, (nrColors - 1));
      return _colors[index];
    }

    private void MoveToLocation(uint viewerId)
    {
      if (_api != null)
      {
        RecordingLocation location = _api.GetRecordingLocation(viewerId);
        IActiveView activeView = ArcUtils.ActiveView;

        if ((location != null) && (activeView != null))
        {
          IPoint point = ArcUtils.GsToMapPoint(location.X, location.Y, location.Z);
          IEnvelope envelope = activeView.Extent;

          if ((point != null) && (envelope != null))
          {
            const double percent = 10.0;
            double xBorder = ((envelope.XMax - envelope.XMin)*percent)/100;
            double yBorder = ((envelope.YMax - envelope.YMin)*percent)/100;
            bool inside = (point.X > (envelope.XMin + xBorder)) && (point.X < (envelope.XMax - xBorder)) &&
                          (point.Y > (envelope.YMin + yBorder)) && (point.Y < (envelope.YMax - yBorder));

            if (!inside)
            {
              envelope.CenterAt(point);
              activeView.Extent = envelope;
              activeView.Refresh();
            }
          }
        }
      }
    }

    #endregion

    #region public static functions

    // =========================================================================
    // Static functions
    // =========================================================================
    private static void ShowGlobespotter(bool show)
    {
      Window.Show(show);

      if (show)
      {
        IActiveView activeView = ArcUtils.ActiveView;
        activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
      }
    }

    private static IDockableWindow GetDocWindow()
    {
      IApplication application = ArcMap.Application;
      ICommandItem tool = application.CurrentTool;
      const string windowName = "IntegrationArcMap_GsFrmGlobespotter";
      IDockableWindow result = ArcUtils.GetDocWindow(windowName);
      application.CurrentTool = tool;
      return result;
    }

    public static void DisposeFrm(bool disposing)
    {
      if (_frmGlobespotter != null)
      {
        _frmGlobespotter.Dispose(disposing);
      }
    }

    public static void CheckVisible()
    {
      if (!Window.IsVisible())
      {
        ShutDown(false);
      }
    }

    public static void ShutDown(bool includeGlobespotter)
    {
      Viewer.Clear(Instance);
      FrmIdentify.Close();

      if ((_frmGlobespotter != null) && includeGlobespotter)
      {
        _frmGlobespotter.Clear();
        GlobeSpotterConfiguration.Delete();
      }

      if (_frmGlobespotter != null)
      {
        _frmGlobespotter._imageId = string.Empty;
      }

      if (_frmGlobespotter != null)
      {
        FrmCycloMediaOptions.ReloadData();
      }
    }

    public static void Restart()
    {
      string imageId = Instance._imageId;
      CycloMediaLayer layer = Instance._layer;
      List<string> imageIds = Viewer.ImageIds;

      if (imageIds.Contains(imageId))
      {
        imageIds.Remove(imageId);
      }
      else
      {
        if (imageIds.Count >= 1)
        {
          Instance._imageId = imageIds[0];
          imageIds.RemoveAt(0);
        }
      }

      Instance._restartImages.AddRange(imageIds);
      ShutDown(true);

      if (!string.IsNullOrEmpty(imageId))
      {
        ShowLocation(imageId, layer);
      }
    }

    public static Color GetColor(int year)
    {
      return Instance.GetCol(year);
    }

    public static void ShowLocation(string imageId, CycloMediaLayer layer)
    {
      Instance.ShowLoc(imageId, layer, false);
    }

    public static void ShowLocation(string imageId, CycloMediaLayer layer, bool replace)
    {
      Instance.ShowLoc(imageId, layer, replace);
    }

    public static void UpdateParameters()
    {
      Instance.UpdateParam();
    }

    public static void AddWfsLayer(WfsLayer layer)
    {
      Instance.AddWfsLay(layer);
    }

    public static void UpdateColor(CycloMediaLayer layer, int? year = null)
    {
      Instance.UpdateCol(layer, year);
    }

    public static void LoginSuccesfull()
    {
      Instance.LoginSucces();
    }

    public static List<Bitmap> GetViewerScreenShot()
    {
      return Instance.GetViewerScreen();
    }

    public static bool IsStarted()
    {
      return Instance.Started;
    }

    #endregion

    #region IAPIClient Members

    // =========================================================================
    // IAPIClient Members
    // =========================================================================
    public void OnComponentReady()
    {
      if (_api != null)
      {
        SpatialReference spatialRef = _config.SpatialReference;
        string epsgCode = ArcUtils.EpsgCode;

        if ((spatialRef != null) && spatialRef.KnownInArcMap)
        {
          epsgCode = spatialRef.SRSName;
        }
        else
        {
          SpatialReferences spatialReferences = SpatialReferences.Instance;
          spatialRef = spatialReferences.GetItem(epsgCode);

          if ((spatialRef == null) || (!spatialRef.KnownInArcMap))
          {
            spatialRef = spatialReferences.Aggregate<SpatialReference, SpatialReference>(null,
              (current, spatialReference) => spatialReference.KnownInArcMap ? spatialReference : current);
          }

          if (spatialRef != null)
          {
            epsgCode = spatialRef.SRSName;
            _config.SpatialReference = spatialRef;
            _config.Save();
          }
        }

        _api.SetAPIKey(_apiKey.Value);
        _api.SetUserNamePassword(_login.Username, _login.Password);
        _api.SetSrsNameViewer(epsgCode);
        _api.SetSrsNameAddress(epsgCode);
        _api.SetAdressLanguageCode("nl");

        if (!_config.RecordingsServiceDefault)
        {
          _api.SetServiceURL(_config.RecordingsService, ServiceUrlType.URL_RECORDING_LOCATION_SERVICE);
        }

        if (!_config.BaseUrlDefault)
        {
          _api.SetServiceURL(_config.BaseUrl, ServiceUrlType.URL_BASE);
        }
      }
    }

    public void OnAPIReady()
    {
      if (_api != null)
      {
        GlobeSpotterConfiguration.Load();
        _api.SetMaxViewers(_config.MaxViewers);
        _api.SetCloseViewerEnabled(true);
        _api.SetViewerToolBarVisible(false);
        _api.SetViewerToolBarButtonsVisible(true);
        _api.SetViewerTitleBarVisible(false);
        _api.SetViewerWindowBorderVisible(false);
        _api.SetHideOverlaysWhenMeasuring(false);
        _api.SetImageInformationEnabled(true);
        _api.SetViewerBrightnessEnabled(true);
        _api.SetViewerSaveImageEnabled(true);
        _api.SetViewerOverlayAlphaEnabled(true);
        _api.SetViewerShowLocationEnabled(true);
        _api.SetViewerDetailImagesVisible(_config.DetailImagesEnabled);
        _api.SetContextMenuEnabled(true);
        _api.SetKeyboardEnabled(true);
        _api.SetViewerRotationEnabled(true);

        if (GlobeSpotterConfiguration.MeasurePermissions)
        {
          _api.SetMeasurementSeriesModeEnabled(true);
        }

        if (GlobeSpotterConfiguration.MeasureSmartClick)
        {
          _api.SetMeasurementSmartClickModeEnabled(_config.SmartClickEnabled);
        }

        UpdateCol();

        VectorLayer.LayerAddEvent += OnAddVectorLayer;
        VectorLayer.LayerRemoveEvent += OnRemoveVectorLayer;
        VectorLayer.LayerChangedEvent += OnRefreshVectorLayer;

        VectorLayer.FeatureStartEditEvent += OnStartEditFeature;
        VectorLayer.FeatureUpdateEditEvent += OnUpdateEditFeature;
        VectorLayer.FeatureDeleteEvent += OnDeleteFeature;

        VectorLayer.StopEditEvent += OnStopEdit;
        VectorLayer.StartMeasurementEvent += OnStartMeasurement;

        VectorLayer.SketchCreateEvent += OnCreateSketch;
        VectorLayer.SketchModifiedEvent += OnModifiedSketch;
        VectorLayer.SketchFinishedEvent += OnSketchFinished;

        CycloMediaLayer.LayerAddedEvent += OnAddCycloMediaLayer;
        CycloMediaLayer.LayerChangedEvent += OnRefreshCycloMediaLayer;
        CycloMediaLayer.LayerRemoveEvent += OnRemoveCycloMediaLayer;

        IActiveViewEvents_Event events = ArcUtils.ActiveViewEvents;

        if (events != null)
        {
          events.ViewRefreshed += OnViewRefreshed;
        }

        var extension = GsExtension.GetExtension();

        if (extension != null)
        {
          CycloMediaGroupLayer cycloMediaGroupLayer = extension.CycloMediaGroupLayer;
          IList<CycloMediaLayer> layers = cycloMediaGroupLayer.Layers;

          foreach (var layer in layers)
          {
            if (!_visibleLayers.Contains(layer))
            {
              _visibleLayers.Add(layer);
            }
          }
        }

        if (!string.IsNullOrEmpty(_imageId))
        {
          OpenImage(false);
        }

        OnHistoricalDateChanged(_yearMonth);
        FrmCycloMediaOptions.ReloadData();
      }
    }

    public void OnAPIFailed()
    {
      MessageBox.Show(Resources.FrmGlobespotter_OnAPIFailed_Initialize_API_failed);
    }

    public void OnImageChanged(uint viewerId)
    {
      Viewer viewer = Viewer.Get(viewerId);

      if ((viewer != null) && (_api != null))
      {
        FrmMeasurement.RemoveImageIdColor(viewer.ImageId);
        MeasurementPoint.RemoveObsColor(viewer.ImageId);
        string imageId = _api.GetImageID(viewerId);
        viewer.Update(imageId);
      }
    }

    public void OnImagePreviewCompleted(uint viewerId)
    {
      Viewer viewer = Viewer.Get(viewerId);

      if ((viewer != null) && (_api != null))
      {
        CurrentCult cult = CurrentCult.Get();
        string dateFormat = cult.DateFormat;
        _api.SetDateFormat(dateFormat);
        string timeFormat = cult.TimeFormat;
        _api.SetTimeFormat(timeFormat);

        RecordingLocation location = _api.GetRecordingLocation(viewerId);
        double angle = _api.GetYaw(viewerId);
        double hFov = _api.GetHFov(viewerId);
        Color color = _api.GetViewerBorderColor(viewerId);
        viewer.Set(location, angle, hFov, color);
        OnRefreshVectorLayer(null);
        FrmMeasurement.AddImageIdColor(viewer.ImageId, color);
        MeasurementPoint.UpdateObsColor(viewer.ImageId, color);

        foreach (var wfsLayer in _wfsLayers)
        {
          if (wfsLayer.Value == null)
          {
            AddWfsLay(wfsLayer.Key);
          }
        }

        if (_lookAtCoord != null)
        {
          _api.LookAtCoordinate(viewerId, _lookAtCoord.x, _lookAtCoord.y, _lookAtCoord.z);
          OnShowLocationRequested(viewerId, _lookAtCoord);
          _lookAtCoord = null;
        }

        if (_restartImages.Contains(_imageId))
        {
          _restartImages.Remove(_imageId);
        }

        if (_restartImages.Count >= 1)
        {
          _imageId = _restartImages[0];
          OpenImage(false);
        }

        MoveToLocation(viewerId);
      }
    }

    public void OnImageSegmentLoaded(uint viewerId)
    {
      // empty
    }

    public void OnImageCompleted(uint viewerId)
    {
      // empty
    }

    public void OnImageFailed(uint viewerId)
    {
      // empty
    }

    public void OnViewLoaded(uint viewerId)
    {
      // empty
    }

    public void OnViewChanged(uint viewerId, double yaw, double pitch, double hFov)
    {
      Viewer viewer = Viewer.Get(viewerId);

      if (viewer != null)
      {
        viewer.Update(yaw, hFov);
      }
    }

    public void OnEntityFocusChanged(int entityId)
    {
      // empty
    }

    public void OnFocusPointChanged(double x, double y, double z)
    {
      // empty
    }

    public void OnViewClicked(uint viewerId, double[] mouseCoords)
    {
      // empty
    }

    public void OnMarkerClicked(uint viewerId, uint drawingId, double[] markerCoords)
    {
      // empty
    }

    public void OnEntityDataChanged(int entityId, EntityData data)
    {
      // empty
    }

    public void OnViewerActive(uint viewerId)
    {
      MoveToLocation(viewerId);
      Viewer viewer = Viewer.Get(viewerId);
      viewer.SetActive();
    }

    public void OnViewerInactive(uint viewerId)
    {
      // empty
    }

    public void OnViewerAdded(uint viewerId)
    {
      if (_api != null)
      {
        string imageId = _api.GetImageID(viewerId);
        Viewer.Add(viewerId, imageId);
        _api.SetActiveViewerReplaceMode(true);
        int nrImages = Viewer.ImageIds.Count;
        _api.SetViewerWindowBorderVisible(nrImages >= 2);
      }
    }

    public void OnViewerRemoved(uint viewerId)
    {
      Viewer viewer = Viewer.Get(viewerId);
      FrmMeasurement.RemoveImageIdColor(viewer.ImageId);
      MeasurementPoint.RemoveObsColor(viewer.ImageId);
      Viewer.Delete(viewerId);
      int nrImages = Viewer.ImageIds.Count;
      _api.SetViewerWindowBorderVisible(nrImages >= 2);

      if (_api != null)
      {
        uint nrviewers = _api.GetViewerCount();

        if (nrviewers == 0)
        {
          CloseGlobespotter();
        }
        else
        {
          OnRefreshVectorLayer(null);
        }
      }
    }

    public void OnMaxViewers(uint maxViewers)
    {
      // empty
    }

    public void OnMeasurementCreated(int entityId, string entityType)
    {
      Measurement.Add(entityId, entityType, this, _drawPoint);
    }

    public void OnMeasurementClosed(int entityId, EntityData data)
    {
      Measurement measurement = Measurement.Get(entityId);

      if (measurement != null)
      {
        measurement.Close();
      }
    }

    public void OnMeasurementOpened(int entityId, EntityData data)
    {
      Measurement measurement = Measurement.Get(entityId);

      if (measurement != null)
      {
        measurement.Open();
      }
    }

    public void OnMeasurementCanceled(int entityId)
    {
      FrmMeasurement.RemoveMeasurement(entityId);
    }

    public void OnMeasurementModeChanged(bool mode)
    {
      // empty
    }

    public void OnMeasurementPointAdded(int entityId, int pointId)
    {
      Measurement measurement = Measurement.Get(entityId);

      if (measurement != null)
      {
        measurement.AddPoint(pointId);

        if (_api != null)
        {
          if ((!_api.GetMeasurementSeriesModeEnabled()) || measurement.IsPointMeasurement)
          {
            OnMeasurementPointUpdated(entityId, pointId);
          }
        }
      }
    }

    public void OnMeasurementPointUpdated(int entityId, int pointId)
    {
      if (_api != null)
      {
        Measurement measurement = Measurement.Get(entityId);

        if ((!measurement.IsPointMeasurement) || (!_observationAdded))
        {
          MeasurementPointUpdated(entityId, pointId);
        }

        PointMeasurementData entityData = _api.GetMeasurementPointData(entityId, pointId);
        var measurementPoint = entityData.measurementPoint;
        FrmMeasurement.UpdateMeasurementPoint(this, measurementPoint, entityId, pointId);
        _observationAdded = false;
      }
    }

    public void OnMeasurementPointRemoved(int entityId, int pointId)
    {
      Measurement measurement = Measurement.Get(entityId);

      if (measurement != null)
      {
        measurement.RemovePoint(pointId);
        FrmMeasurement.RemoveMeasurementPoint(entityId, pointId);
      }
    }

    public void OnMeasurementPointOpened(int entityId, int pointId)
    {
      Measurement measurement = Measurement.Get(entityId);

      if (measurement != null)
      {
        if (measurement.ContainsKey(pointId))
        {
          MeasurementPoint point = measurement[pointId];
          point.Opened();
        }
      }
    }

    public void OnMeasurementPointClosed(int entityId, int pointId)
    {
      Measurement measurement = Measurement.Get(entityId);

      if (measurement != null)
      {
        if (measurement.ContainsKey(pointId))
        {
          MeasurementPoint point = measurement[pointId];
          point.Closed();
        }
      }
    }

    public void OnMeasurementPointObservationAdded(int entityId, int pointId, string imageId, Bitmap match)
    {
      _observationAdded = true;
      ObservationMeasurementData observation = _api.GetMeasurementPointObservationData(entityId, pointId, imageId);
      FrmMeasurement.AddObservation(match, this, entityId, pointId, observation.measurementObservation);
    }

    public void OnMeasurementPointObservationUpdated(int entityId, int pointId, string imageId)
    {
      _observationAdded = true;
      ObservationMeasurementData observation = _api.GetMeasurementPointObservationData(entityId, pointId, imageId);
      FrmMeasurement.UpdateObservation(entityId, pointId, observation.measurementObservation);
    }

    public void OnMeasurementPointObservationRemoved(int entityId, int pointId, string imageId)
    {
      FrmMeasurement.RemoveObservation(entityId, pointId, imageId);
    }

    public void OnDividerPositionChanged(double position)
    {
      // empty
    }

    public void OnMapClicked(Point2D point)
    {
      // empty
    }

    public void OnMapExtentChanged(MapExtent extent, Point2D mapCenter, uint zoomLevel)
    {
      // empty
    }

    public void OnOpenImageFailed(string input)
    {
      // empty
    }

    public void OnOpenImageResult(string input, bool opened, string imageId)
    {
      try
      {
        if (!String.IsNullOrEmpty(input))
        {
          if (input.Contains("Vector3D"))
          {
            input = input.Remove(0, 9);
            input = input.Remove(input.Length - 1, 1);
            var seperator = new[] {", "};
            string[] split = input.Split(seperator, StringSplitOptions.None);

            if (split.Length == 3)
            {
              CultureInfo ci = CultureInfo.InvariantCulture;
              var point3D = new Point3D(Double.Parse(split[0], ci), Double.Parse(split[1], ci),
                                        Double.Parse(split[2], ci));
              uint viewerId = _api.GetActiveViewer();
              OnShowLocationRequested(viewerId, point3D);
            }
          }
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(String.Format("Exception occured, message: {0}", ex.Message));
      }
    }

    public void OnOpenNearestImageResult(string input, bool opened, string imageId)
    {
      // empty
    }

    public void OnFeatureClicked(Dictionary<string, string> featureData)
    {
      FrmIdentify.Show(featureData);
    }

    public void OnAutoCompleteResult(string result, string[] matches)
    {
      // empty
    }

    public void OnMapInitialized()
    {
      // empty
    }

    public void OnShowLocationRequested(uint viewerId, Point3D point3D)
    {
      IActiveView activeView = ArcUtils.ActiveView;

      if ((activeView != null) && (point3D != null))
      {
        IPoint point = ArcUtils.GsToMapPoint(point3D.x, point3D.y, point3D.z);
        IEnvelope envelope = activeView.Extent;

        if ((point != null) && (envelope != null))
        {
          envelope.CenterAt(point);
          activeView.Extent = envelope;
          activeView.Refresh();

          Viewer viewer = Viewer.Get(viewerId);
          viewer.SetActive();
        }
      }
    }

    public void OnDetailImagesVisibilityChanged(bool value)
    {
      _config.DetailImagesEnabled = value;
      _config.Save();
    }

    public void OnMeasurementHeightLevelChanged(int entityId, double level)
    {
      // empty
    }

    public void OnMeasurementPointHeightLevelChanged(int entityId, int pointId, double level)
    {
      // empty
    }

    public void OnMapBrightnessChanged(double value)
    {
      // empty
    }

    public void OnMapContrastChanged(double value)
    {
      // empty
    }

    public void OnObliqueImageChanged()
    {
      // empty
    }

    #endregion

    #region Add / Remove layer events

    private void OnAddVectorLayer(VectorLayer vectorLayer)
    {
      if (GlobeSpotterConfiguration.AddLayerWfs && (vectorLayer != null) && vectorLayer.IsVisibleInGlobespotter && (_api != null))
      {
        OnRemoveVectorLayer(vectorLayer);
        Color color;
        string gml = GetGml(vectorLayer, out color);
        AddVectorLayer(vectorLayer, gml, color);
      }
    }

    private void OnRemoveVectorLayer(VectorLayer vectorLayer)
    {
      if (GlobeSpotterConfiguration.AddLayerWfs && (vectorLayer != null) && (_api != null))
      {
        if (_vectorLayers.ContainsKey(vectorLayer))
        {
          uint layerId = _vectorLayers[vectorLayer];
          _api.RemoveLayer(layerId);
          _vectorLayers.Remove(vectorLayer);
        }
      }
    }

    private void OnRefreshVectorLayer(VectorLayer layer)
    {
      if (GlobeSpotterConfiguration.AddLayerWfs && (_api != null))
      {
        if (layer != null)
        {
          if (_vectorLayers.ContainsKey(layer))
          {
            uint layerId = _vectorLayers[layer];
            _api.RemoveLayer(layerId);
            _vectorLayers.Remove(layer);
          }

          OnAddVectorLayer(layer);
        }
        else
        {
          IList<VectorLayer> vectorLayers = VectorLayer.Layers;
          int i = 0;

          while (i < _vectorLayers.Count)
          {
            var vectorLayer = _vectorLayers.ElementAt(i);
            VectorLayer lay = vectorLayer.Key;

            if (!vectorLayers.Contains(lay))
            {
              uint layerId = vectorLayer.Value;
              _api.RemoveLayer(layerId);
              _vectorLayers.Remove(lay);
            }
            else
            {
              i++;
            }
          }

          foreach (var vectorLayer in vectorLayers)
          {
            if (!_vectorLayers.ContainsKey(vectorLayer))
            {
              OnAddVectorLayer(vectorLayer);
            }
            else
            {
              if (vectorLayer.IsVisibleInGlobespotter)
              {
                Color color;
                string gml = GetGml(vectorLayer, out color);

                if (vectorLayer.GmlChanged)
                {
                  uint layerId = _vectorLayers[vectorLayer];
                  _api.RemoveLayer(layerId);
                  _vectorLayers.Remove(vectorLayer);
                  AddVectorLayer(vectorLayer, gml, color);
                }
              }
              else
              {
                if (_vectorLayers.ContainsKey(vectorLayer))
                {
                  uint layerId = _vectorLayers[vectorLayer];
                  _api.RemoveLayer(layerId);
                  _vectorLayers.Remove(vectorLayer);
                }
              }
            }
          }
        }
      }
    }

    private string GetGml(VectorLayer vectorLayer, out Color color)
    {
      List<RecordingLocation> locations = Viewer.Locations;
      double distanceVectorLayer = _config.DistanceCycloramaVectorLayer;
      return vectorLayer.GetGmlFromLocation(locations, distanceVectorLayer, out color, _config.SpatialReference);
    }

    private void AddVectorLayer(VectorLayer vectorLayer, string gml, Color color)
    {
      SpatialReference spatRel = _config.SpatialReference;
      string srsName = (spatRel == null) ? ArcUtils.EpsgCode : spatRel.SRSName;
      string layerName = vectorLayer.Name;
      const int minZoomLevel = 7;
      uint layerId = _api.AddGMLLayer(layerName, gml, srsName, color, true, false, minZoomLevel);
      _vectorLayers.Add(vectorLayer, layerId);
    }

    private void OnAddCycloMediaLayer(CycloMediaLayer cycloMediaLayer)
    {
      if (!(cycloMediaLayer is WfsLayer))
      {
        if (!_visibleLayers.Contains(cycloMediaLayer))
        {
          _visibleLayers.Add(cycloMediaLayer);
          RefreshVisibleLayers();
        }
      }
    }

    private void OnRefreshCycloMediaLayer(CycloMediaLayer cycloMediaLayer)
    {
      if (_api != null)
      {
        if (cycloMediaLayer is WfsLayer)
        {
          var wfsLayer = cycloMediaLayer as WfsLayer;

          if (_wfsLayers.ContainsKey(wfsLayer))
          {
            uint? layerId = _wfsLayers[wfsLayer];

            if (wfsLayer.IsVisibleInGlobespotter)
            {
              if (layerId != null)
              {
                _api.RemoveLayer((uint) layerId);
                _wfsLayers[wfsLayer] = null;
              }

              AddWfsLay(wfsLayer);
            }
            else
            {
              if (layerId != null)
              {
                _api.RemoveLayer((uint) layerId);
                _wfsLayers[wfsLayer] = null;
              }
            }
          }
        }
        else
        {
          if (cycloMediaLayer != null)
          {
            if (!_visibleLayers.Contains(cycloMediaLayer))
            {
              _visibleLayers.Add(cycloMediaLayer);
            }

            RefreshVisibleLayers();
          }
        }
      }
    }

    private void OnRemoveCycloMediaLayer(CycloMediaLayer cycloMediaLayer)
    {
      if (cycloMediaLayer is WfsLayer)
      {
        if (_api != null)
        {
          var wfsLayer = cycloMediaLayer as WfsLayer;

          if (_wfsLayers.ContainsKey(wfsLayer))
          {
            uint? layerId = _wfsLayers[wfsLayer];

            if (layerId != null)
            {
              _api.RemoveLayer((uint) layerId);
            }

            _wfsLayers.Remove(wfsLayer);
          }
        }
      }
      else
      {
        if (cycloMediaLayer != null)
        {
          if (_visibleLayers.Contains(cycloMediaLayer))
          {
            _visibleLayers.Remove(cycloMediaLayer);
          }

          RefreshVisibleLayers();
        }
      }
    }

    private void OnDateRangeChanged()
    {
      if (_layer != null)
      {
        _layer.Refresh();
      }
    }

    private void OnHistoricalDateChanged(SortedDictionary<int, int> yearMonth)
    {
      _yearMonth = yearMonth;
      FrmRecordingHistory.OnChangeDateRange(yearMonth);
    }

    private void RefreshVisibleLayers()
    {
      bool visible = _visibleLayers.Aggregate
        (false, (current, visibleLayer) => visibleLayer.IsVisibleInGlobespotter || current);

      if (_api != null)
      {
        _api.SetRecordingLocationsVisible(visible);

        if (visible)
        {
          UpdateDateRange();
        }
      }
    }

    private void OnStartMeasurement(IGeometry geometry)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        Measurement measurement = Measurement.Sketch;
        StartMeasurement(geometry, measurement, true);
      }
    }

    private void OnDeleteFeature(IFeature feature)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        IGeometry geometry = feature.Shape;

        if (geometry != null)
        {
          Measurement measurement = Measurement.Get(geometry);

          if (measurement != null)
          {
            measurement.RemoveMeasurement();
          }
        }
      }
    }

    private void OnStartEditFeature(IList<IGeometry> geometries)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (geometries != null))
      {
        var usedMeasurements = new List<Measurement>();

        foreach(IGeometry geometry in geometries)
        {
          if (geometry != null)
          {
            Measurement measurement = Measurement.Get(geometry);
            _drawPoint = false;
            measurement = StartMeasurement(geometry, measurement, false);
            _drawPoint = true;

            if (measurement != null)
            {
              measurement.UpdateMeasurementPoints(geometry);
              measurement.CloseMeasurement();
              usedMeasurements.Add(measurement);
            }
          }
        }

        Measurement.RemoveUnusedMeasurements(usedMeasurements);
      }
    }

    private void OnUpdateEditFeature(IFeature feature)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (feature != null))
      {
        IGeometry geometry = feature.Shape;

        if (geometry != null)
        {
          Measurement measurement = Measurement.Get(geometry);

          if (measurement != null)
          {
            measurement.UpdateMeasurementPoints(geometry);
          }
        }
      }
    }

    private void OnStopEdit()
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        _drawingSketch = false;
        Measurement.RemoveAll();
        FrmMeasurement.Close();
      }
    }

    private void OnCreateSketch(IEditSketch3 sketch)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (!_sketchModified) && (!_screenPointAdded) && (_layer != null))
      {
        _sketchModified = true;
        _layer.AddZToSketch(sketch);
        _sketchModified = false;
      }
    }

    private void OnModifiedSketch(IGeometry geometry)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        _mapPointAdded = !_screenPointAdded;
        Measurement measurement = Measurement.Sketch;

        if (geometry != null)
        {
          if (((!_drawingSketch) && (!geometry.IsEmpty)) || (measurement == null))
          {
            _drawingSketch = true;
            measurement = StartMeasurement(geometry, measurement, true);
          }

          if (measurement != null)
          {
            measurement.UpdateMeasurementPoints(geometry);
          }
        }

        _mapPointAdded = false;
      }
    }

    private void OnSketchFinished()
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        _screenPointAdded = false;
        _mapPointAdded = false;
        _drawingSketch = false;
        Measurement.RemoveSketch();
      }
    }

    #endregion

    #region Measurement functions

    private Measurement StartMeasurement(IGeometry geometry, Measurement measurement, bool sketch)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        bool measurementExists = false;
        const string name = "my measurement";
        var typeOfLayer = Measurement.GetTypeOfLayer(geometry);

        if ((_api != null) && (typeOfLayer != TypeOfLayer.None))
        {
          if (measurement != null)
          {
            if (measurement.IsTypeOfLayer(typeOfLayer))
            {
              measurementExists = true;
              measurement.OpenMeasurement();
            }
            else
            {
              measurement.RemoveMeasurement();
            }
          }

          if (!measurementExists)
          {
            _measurementName = name;
            Measurement.CloseOpenMeasurement();
            int entityId = CreateMeasurement(typeOfLayer);
            measurement = (entityId == -1) ? null : Measurement.Get(entityId);

            if (measurement != null)
            {
              measurement.Open();

              if (sketch)
              {
                measurement.SetSketch();
              }
            }
          }
        }
      }

      return measurement;
    }

    public void CloseMeasurement(int entityId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.CloseMeasurement(entityId);
      }
    }

    public void RemoveMeasurement(int entityId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.RemoveEntity(entityId);
      }
    }

    public void OpenMeasurement(int entityId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.OpenMeasurement(entityId);
        _api.SetFocusEntity(entityId);
      }
    }

    public void DisableMeasurementSeries()
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.SetMeasurementSeriesModeEnabled(false);
      }
    }

    public bool GetMeasurementSeriesEnabled()
    {
      bool result = false;

      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        result = _api.GetMeasurementSeriesModeEnabled();
      }

      return result;
    }

    public void EnableMeasurementSeries(int entityId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.NextMeasurementSeries(entityId);
        _api.SetMeasurementSeriesModeEnabled(true);
      }
    }

    public void EnableMeasurementSeries()
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.SetMeasurementSeriesModeEnabled(true);
      }
    }

    public void RemoveMeasurementPoint(int entityId, int pointId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.RemoveMeasurementPoint(entityId, pointId);
      }
    }

    public int CreateMeasurementPoint(int entityId, IPoint point)
    {
      int result = -1;

      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        var point3D = new Point3D(point.X, point.Y, point.Z);
        result = _api.CreateMeasurementPoint(entityId, point3D);
      }

      return result;
    }

    public void OpenMeasurementPoint(int entityId, int pointId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.OpenMeasurementPoint(entityId, pointId);
        PointMeasurementData pointMeasurementData = _api.GetMeasurementPointData(entityId, pointId);
        FrmMeasurement.OpenMeasurementPoint(entityId, pointId, this, pointMeasurementData.measurementPoint);
      }
    }

    public void AddMeasurementPoint(int entityId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.AddMeasurementPoint(entityId);
      }
    }

    public void SetFocusEntity(int entityId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.SetFocusEntity(entityId);
      }
    }

    public void CloseMeasurementPoint(int entityId, int pointId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.CloseMeasurementPoint(entityId, pointId);
        FrmMeasurement.CloseMeasurementPoint(entityId, pointId);
      }
    }

    public int CreateMeasurement(TypeOfLayer typeOfLayer)
    {
      int entityId = -1;

      switch (typeOfLayer)
      {
        case TypeOfLayer.Point:
          if (GlobeSpotterConfiguration.MeasurePoint)
          {
            entityId = _api.AddPointMeasurement(_measurementName);
            OpenMeasurement(entityId);
            DisableMeasurementSeries();
            AddMeasurementPoint(entityId);
          }

          break;
        case TypeOfLayer.Line:
          if (GlobeSpotterConfiguration.MeasureLine)
          {
            entityId = _api.AddLineMeasurement(_measurementName);
            OpenMeasurement(entityId);
            EnableMeasurementSeries(entityId);
          }

          break;
        case TypeOfLayer.Polygon:
          if (GlobeSpotterConfiguration.MeasurePolygon)
          {
            entityId = _api.AddSurfaceMeasurement(_measurementName);
            _api.SetMeasurementExtrusionEnabled(entityId, false);
            OpenMeasurement(entityId);
            EnableMeasurementSeries(entityId);
          }

          break;
      }

      return entityId;
    }

    public void MeasurementPointUpdated(int entityId, int pointId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions)
      {
        _screenPointAdded = !_mapPointAdded;

        if (_screenPointAdded)
        {
          ArcUtils.SetZOffset();
        }

        PointMeasurementData measurementData = _api.GetMeasurementPointData(entityId, pointId);
        Measurement measurement = Measurement.Get(entityId);

        if ((measurementData != null) && (measurement != null))
        {
          int index = _api.GetMeasurementPointIndex(entityId, pointId);
          measurement.UpdatePoint(pointId, measurementData, index);
        }

        _screenPointAdded = false;
      }
    }

    public void SelectImageMeasurement(string imageId)
    {
      ShowLoc(imageId, null, true);
    }

    public void RemoveMeasurementObservation(int entityId, int pointId, string imageId)
    {
      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        _api.RemoveMeasurementPointObservation(entityId, pointId, imageId);
      }
    }

    private List<Bitmap> GetViewerScreen()
    {
      var result = new List<Bitmap>();

      if (ApiReady)
      {
        int[] viewerIds = _api.GetViewerIDs();
        result.AddRange(viewerIds.Select(id => _api.GetViewerScreenshot((uint) id)));
      }

      return result;
    }

    public void LookAtMeasurement(string imageId, double x, double y, double z)
    {
      if (_api != null)
      {
        _lookAtCoord = new Point3D(x, y, z);
        _frmGlobespotter.SelectImageMeasurement(imageId);
      }
    }

    public void LookAtMeasurement(double x, double y, double z)
    {
      if (_api != null)
      {
        int[] viewerIds = _api.GetViewerIDs();

        foreach (var viewerId in viewerIds)
        {
          _api.LookAtCoordinate((uint) viewerId, x, y, z);
        }
      }
    }

    public void OpenNearestImage(double x, double y, double z)
    {
      if (_api != null)
      {
        CultureInfo ci = CultureInfo.InvariantCulture;
        string coordinate = String.Format(ci, "{0:#0.#},{1:#0.#},{2:#0.#}", x, y, z);
        _api.OpenNearestImage(coordinate, 1);
      }
    }

    public List<MeasurementObservation> GetObservationPoints(int entityId, int pointId)
    {
      var observations = new List<MeasurementObservation>();

      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        string[] imageIds = _api.GetMeasurementPointObservationImageIDs(entityId, pointId);

        if (imageIds != null)
        {
          observations.AddRange(from imageId in imageIds
                                select _api.GetMeasurementPointObservationData(entityId, pointId, imageId)
                                into obsData
                                where obsData != null
                                select obsData.measurementObservation);
        }
      }

      return observations;
    }

    public GsMeasurementPoint GetMeasurementData(int entityId, int pointId)
    {
      GsMeasurementPoint measurementPoint = null;

      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        PointMeasurementData data = _api.GetMeasurementPointData(entityId, pointId);

        if (data != null)
        {
          measurementPoint = data.measurementPoint;
        }
      }

      return measurementPoint;
    }

    public int GetMeasurementPointIndex(int entityId, int pointId)
    {
      int result = 0;

      if (GlobeSpotterConfiguration.MeasurePermissions && (_api != null))
      {
        result = _api.GetMeasurementPointIndex(entityId, pointId);
      }

      return result;
    }

    #endregion

    #region other event handlers

    private void OnViewRefreshed(IActiveView view, esriViewDrawPhase phase, object data, IEnvelope envelope)
    {
      if (_api != null)
      {
        IMap map = ArcUtils.Map;
        esriUnits units = map.DistanceUnits;
        string stunit = units.ToString();
        stunit = stunit.Replace("esri", " ");
        string label = _api.GetLengthUnitLabel();

        if (label != stunit)
        {
          IUnitConverter converter = new UnitConverterClass();
          double factor = converter.ConvertUnits(1, units, esriUnits.esriMeters);

          SpatialReference spatialRef = _config.SpatialReference;
          ISpatialReference gsSpatialReference = ((spatialRef != null) && spatialRef.KnownInArcMap)
            ? spatialRef.SpatialRef
            : ArcUtils.SpatialReference;
          var projCoord = gsSpatialReference as IProjectedCoordinateSystem;
          double conversion = 1.0;

          if (projCoord == null)
          {
            var geoCoord = gsSpatialReference as IGeographicCoordinateSystem;

            if (geoCoord != null)
            {
              IAngularUnit unit = geoCoord.CoordinateUnit;
              conversion = factor * unit.ConversionFactor;
            }
          }
          else
          {
            ILinearUnit unit = projCoord.CoordinateUnit;
            conversion = factor / unit.ConversionFactor;
          }

          _api.SetLengthUnitLabel(stunit);
          _api.SetLengthUnitFactor(conversion);
        }
      }
    }

    #endregion
  }
}
