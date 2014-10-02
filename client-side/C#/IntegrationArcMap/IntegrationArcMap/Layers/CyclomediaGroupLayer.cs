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
using System.IO;
using System.Linq;
using ESRI.ArcGIS.ArcMapUI;
using IntegrationArcMap.Client;
using IntegrationArcMap.Model;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using Path = System.IO.Path;

namespace IntegrationArcMap.Layers
{
  public class CycloMediaGroupLayer: IDisposable
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private IList<CycloMediaLayer> _allLayers; 
    private IList<CycloMediaLayer> _currentLayers;
    private IFeatureWorkspace _featureWorkspace;
    private IGroupLayer _groupLayer;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public string Name { get { return "CycloMedia"; } }

    public IGroupLayer GroupLayer
    {
      get { return _groupLayer ?? (_groupLayer = GetGroupLayer()); }
    }

    public IList<CycloMediaLayer> AllLayers
    {
      get
      {
        return _allLayers ?? (_allLayers = new List<CycloMediaLayer>
               {new RecordingLayer(this), new HistoricalLayer(this)});
      }
    }

    public IList<CycloMediaLayer> Layers
    {
      get { return _currentLayers ?? (_currentLayers = GetCurrentLayers()); }
    }

    public bool ContainsLayers
    {
      get { return (Layers.Count != 0); }
    }

    public bool HistoricalLayerEnabled
    {
      get { return Layers.Aggregate(false, (current, layer) => current || layer.UseDateRange); }
    }

    public IFeatureWorkspace FeatureWorkspace
    {
      get { return _featureWorkspace ?? (_featureWorkspace = GetDefaultFeatureWorkSpace()); }
    }

    public bool InsideScale
    {
      get { return Layers.Aggregate(false, (current, layer) => layer.InsideScale || current); }
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public CycloMediaGroupLayer()
    {
      CreateWorkspace();
      CreateLayers();
      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.ContentsChanged += OnContentChanged;
      }
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (public)
    // =========================================================================
    public CycloMediaLayer GetLayer(ILayer layer)
    {
      return Layers.Aggregate<CycloMediaLayer, CycloMediaLayer>(null,
                                                                (current, layerCheck) =>
                                                                (layerCheck.Layer == layer) ? layerCheck : current);
    }

    public CycloMediaLayer AddLayer(string name)
    {
      CycloMediaLayer thisLayer = null;

      if (!Layers.Aggregate(false, (current, cycloLayer) => (cycloLayer.Name == name) || current))
      {
        thisLayer = AllLayers.Aggregate<CycloMediaLayer, CycloMediaLayer>
          (null, (current, checkLayer) => (checkLayer.Name == name) ? checkLayer : current);

        if (thisLayer != null)
        {
          thisLayer.AddToLayers();
        }
      }

      return thisLayer;
    }

    public void RemoveLayer(string name)
    {
      CycloMediaLayer layer = Layers.Aggregate<CycloMediaLayer, CycloMediaLayer>
        (null, (current, checkLayer) => (checkLayer.Name == name) ? checkLayer : current);

      if (layer != null)
      {
        Layers.Remove(layer);
        layer.Dispose();
        IActiveView activeView = ArcUtils.ActiveView;

        if (activeView != null)
        {
          activeView.ContentsChanged();
          activeView.Refresh();
        }
      }
    }

    public bool IsKnownName(string name)
    {
      return Layers.Aggregate((name == Name), (current, layer) => (layer.Name == name) || current);
    }

    public void Dispose()
    {
      int i = 0;

      while (Layers.Count > i)
      {
        CycloMediaLayer layer = Layers[i];
        layer.Dispose();

        if (layer is WfsLayer)
        {
          Layers.Remove(layer);
        }
        else
        {
          i++;
        }
      }

      IActiveView activeView = ArcUtils.ActiveView;
      IMap map = ArcUtils.Map;

      if ((activeView != null) && (map != null))
      {
        var layer = GroupLayer as ILayer;

        if (layer != null)
        {
          map.DeleteLayer(layer);
        }

        activeView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
      }

      var avEvents = ArcUtils.ActiveViewEvents;

      if (avEvents != null)
      {
        avEvents.ContentsChanged -= OnContentChanged;
      }
    }

    public string GetFeatureFromPoint(int x, int y, out CycloMediaLayer layer)
    {
      string result = string.Empty;
      layer = null;

      foreach (var layert in Layers)
      {
        if (string.IsNullOrEmpty(result))
        {
          if ((layert.IsVisible) && (!(layert is WfsLayer)))
          {
            layer = layert;
            result = layer.GetFeatureFromPoint(x, y);
          }
        }
      }

      return result;
    }

    public IMappedFeature GetLocationInfo(string imageId)
    {
      return Layers.Select(layer => layer.GetLocationInfo(imageId)).Aggregate<IMappedFeature, IMappedFeature>
        (null, (current, mappedFeature) => mappedFeature ?? current);
    }

    public void MakeEmpty()
    {
      foreach (var layer in AllLayers)
      {
        layer.MakeEmpty();
      }

      IActiveView activeView = ArcUtils.ActiveView;

      if (activeView != null)
      {
        activeView.Refresh();
      }
    }

    #endregion

    #region functions (private)

    // =========================================================================
    // Functions (private)
    // =========================================================================
    private void CreateLayers()
    {
      if (GroupLayer == null)
      {
        IMap map = ArcUtils.Map;

        if (map != null)
        {
          _groupLayer = new GroupLayerClass();

          if (_groupLayer != null)
          {
            _groupLayer.Name = Name;
          }

          CreateFeatureLayers();
          var layer = _groupLayer as ILayer;
          map.AddLayer(layer);
        }
      }
      else
      {
        CreateFeatureLayers();
      }
    }

    private void CreateFeatureLayers()
    {
      foreach (var layer in Layers)
      {
        layer.AddToLayers();
      }
    }

    private void CreateWorkspace()
    {
      if (FeatureWorkspace == null)
      {
        Config config = Config.Instance;
        string location = config.CycloramaVectorLayerLocationDefault ? ArcUtils.FileDir : config.CycloramaVectorLayerLocation;

        if (!Directory.Exists(location))
        {
          Directory.CreateDirectory(location);
        }

        ISpatialReference spatialReference = ArcUtils.SpatialReference;
        int factoryCode = spatialReference.FactoryCode;
        IWorkspaceFactory workspaceFactory = new FileGDBWorkspaceFactoryClass();
        string name = Path.Combine(location, string.Concat(Name, factoryCode));
        string workSpaceFileName = string.Format("{0}.gdb", name);

        if (workspaceFactory.IsWorkspace(workSpaceFileName))
        {
          IWorkspace workspace = workspaceFactory.OpenFromFile(workSpaceFileName, 0);
          _featureWorkspace = workspace as IFeatureWorkspace;
        }
        else
        {
          IWorkspaceName workspaceName = workspaceFactory.Create(string.Empty, name, null, 0);
          var nameObject = workspaceName as IName;

          if (nameObject != null)
          {
            var workspace = nameObject.Open() as IWorkspace;

            if (workspace != null)
            {
              _featureWorkspace = workspace as IFeatureWorkspace;
            }
          }
        }
      }
    }

    private IList<CycloMediaLayer> GetCurrentLayers()
    {
      IList<CycloMediaLayer> result = new List<CycloMediaLayer>();
      IMap map = ArcUtils.Map;

      if (map != null)
      {
        // ReSharper disable UseIndexedProperty
        var layers = map.get_Layers();
        ILayer layer;

        while ((layer = layers.Next()) != null)
        {
          CycloMediaLayer thisLayer = AllLayers.Aggregate<CycloMediaLayer, CycloMediaLayer>
            (null, (current, checkLayer) => (checkLayer.Name == layer.Name) ? checkLayer : current);

          if (thisLayer != null)
          {
            result.Add(thisLayer);
          }
        }

        // ReSharper restore UseIndexedProperty
      }

      return result;
    }

    private IGroupLayer GetGroupLayer()
    {
      IGroupLayer result = null;
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
            if (layer is GroupLayer)
            {
              result = layer as IGroupLayer;
              leave = true;
            }
            else
            {
              map.DeleteLayer(layer);
            }
          }
        }

        // ReSharper restore UseIndexedProperty
      }

      return result;
    }

    private IFeatureWorkspace GetDefaultFeatureWorkSpace()
    {
      IFeatureWorkspace result = null;
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
            leave = true;
            var partLayers = layer as ICompositeLayer;

            if (partLayers != null)
            {
              int n = partLayers.Count;
              int i = 0;
              bool leavePart = false;

              while ((i++ < n) && (!leavePart))
              {
                ILayer layerPart = partLayers.get_Layer(i - 1);

                if (layerPart is IFeatureLayer)
                {
                  leavePart = true;
                  var dataset = layerPart as IDataset;

                  if (dataset != null)
                  {
                    try
                    {
                      result = dataset.Workspace as IFeatureWorkspace;
                    }
                    catch
                    {
                      result = null;
                    }
                  }
                }
              }
            }
          }
        }

        // ReSharper restore UseIndexedProperty
      }

      return result;
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Event handlers
    // =========================================================================
    private void OnContentChanged()
    {
      foreach (var layer in AllLayers)
      {
        if (!Layers.Aggregate(false, (current, visLayer) => current || (visLayer == layer)))
        {
          layer.IsVisible = true;
          layer.Visible = false;
        }
      }

      CycloMediaLayer changedLayer = Layers.Aggregate<CycloMediaLayer, CycloMediaLayer>
        (null, (current, layer) => (layer.IsVisible && (!layer.Visible)) ? layer : current);
      CycloMediaLayer refreshLayer = null;

      foreach (var layer in Layers)
      {
        bool visible = ((changedLayer == null) || (layer == changedLayer)) && layer.IsVisible;
        refreshLayer = (layer.IsVisible != visible) ? layer : refreshLayer;
        layer.IsVisible = visible;
        layer.Visible = layer.IsVisible;
      }

      if (refreshLayer != null)
      {
        IMxDocument document = ArcUtils.MxDocument;
        IActiveView activeView = ArcUtils.ActiveView;

        if (document != null)
        {
          // ReSharper disable UseIndexedProperty
          IContentsView contentView = document.get_ContentsView(0);
          // ReSharper restore UseIndexedProperty

          if (contentView != null)
          {
            contentView.Refresh(_groupLayer);
          }
        }

        if (activeView != null)
        {
          activeView.Refresh();
        }
      }
    }

    #endregion
  }
}
