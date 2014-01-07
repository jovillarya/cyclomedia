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
using System.Drawing;
using System.Linq;
using IntegrationArcMap.Forms;
using GlobeSpotterAPI;

namespace IntegrationArcMap.Objects
{
  internal class Viewer : IDisposable
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static readonly Dictionary<uint, Viewer> Viewers;

    private Arrow _arrow;

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public RecordingLocation Location { get; private set; }
    public string ImageId { get; private set; }

    public static List<string> ImageIds
    {
      get { return Viewers.Select(viewer => viewer.Value.ImageId).ToList(); }
    }

    public static List<RecordingLocation> Locations
    {
      get { return Viewers.Select(viewer => viewer.Value.Location).ToList(); }
    }

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    static Viewer()
    {
      Viewers = new Dictionary<uint, Viewer>();
    }

    private Viewer(string imageId)
    {
      _arrow = null;
      Location = null;
      ImageId = imageId;
    }

    #endregion

    #region functions (public)

    // =========================================================================
    // Functions (Public)
    // =========================================================================
    public void Dispose()
    {
      if (_arrow != null)
      {
        _arrow.Dispose();
        _arrow = null;
      }
    }

    public void Set(RecordingLocation location, double angle, double hFov, Color color)
    {
      if (_arrow != null)
      {
        _arrow.Dispose();
      }

      Location = location;
      _arrow = Arrow.Create(location, angle, hFov, color);

      foreach (var viewer in Viewers.Values)
      {
        if (viewer != this)
        {
          viewer._arrow.SetActive(false);
        }
      }
    }

    public void Update(double angle, double hFov)
    {
      if (_arrow != null)
      {
        _arrow.Update(angle, hFov);
      }
    }

    public void Update(string imageId)
    {
      ImageId = imageId;
    }

    public void SetActive()
    {
      foreach (var viewer in Viewers.Values)
      {
        viewer._arrow.SetActive(viewer == this);
      }
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Functions (Static)
    // =========================================================================
    public static Viewer Get(uint viewerId)
    {
      return Viewers[viewerId];
    }

    public static void Clear(FrmGlobespotter globespotter)
    {
      foreach (var viewer in Viewers)
      {
        uint viewerId = viewer.Key;
        globespotter.CloseViewer(viewerId);
        Viewer myViewer = viewer.Value;
        myViewer.Dispose();
      }

      Viewers.Clear();
    }

    public static void Redraw()
    {
      foreach (var viewer in Viewers.Values)
      {
        viewer._arrow.Redraw();
      }
    }

    public static void Add(uint viewerId, string imageId)
    {
      Viewers.Add(viewerId, new Viewer(imageId));
    }

    public static void Delete(uint viewerId)
    {
      Viewers[viewerId].Dispose();
      Viewers.Remove(viewerId);
    }

    #endregion
  }
}
