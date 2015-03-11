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
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using IntegrationArcMap.Client;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Layers;
using ESRI.ArcGIS.Desktop.AddIns;
using IntegrationArcMap.Model;
using IntegrationArcMap.Model.Atlas;
using IntegrationArcMap.Properties;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This is the tool for select a point on the screen and open a cyclorama
  /// </summary>
  public class GsOpenLocation : Tool
  {
    #region members

    private Cursor _thisCursor;
    private readonly LogClient _logClient;

    #endregion

    public GsOpenLocation()
    {
      _logClient = new LogClient(typeof(GsOpenLocation));
    }

    #region event handlers

    protected override void OnActivate()
    {
      if (_thisCursor == null)
      {
        Type thisType = GetType();
        string cursorPath = string.Format(@"IntegrationArcMap.Images.{0}.cur", thisType.Name);
        Assembly thisAssembly = Assembly.GetAssembly(thisType);
        Stream cursorStream = thisAssembly.GetManifestResourceStream(cursorPath);

        if (cursorStream != null)
        {
          _thisCursor = new Cursor(cursorStream);
        }
      }

      base.OnActivate();
    }

    protected override void OnUpdate()
    {
      try
      {
        GsExtension extension = GsExtension.GetExtension();
        Enabled = ((ArcMap.Application != null) && extension.Enabled);
      }
      catch (Exception ex)
      {
        _logClient.Error("GsOpenLocation.OnUpdate", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsOpenLocation.OnUpdate");
      }

      base.OnUpdate();
    }

    protected override void OnMouseMove(MouseEventArgs arg)
    {
      try
      {
        CycloMediaLayer layer;
        string imageId = GetImageIdFromPoint(arg, out layer);
        Cursor = (string.IsNullOrEmpty(imageId) || (layer == null)) ? _thisCursor : Cursors.Arrow;
      }
      catch (Exception ex)
      {
        _logClient.Error("GsOpenLocation.OnMouseMove", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsOpenLocation.OnMouseMove");
      }

      base.OnMouseMove(arg);
    }

    protected override void OnMouseUp(MouseEventArgs arg)
    {
      try
      {
        CycloMediaLayer layer;
        string imageId = GetImageIdFromPoint(arg, out layer);

        if ((!string.IsNullOrEmpty(imageId)) && (layer != null))
        {
          IMappedFeature mappedFeature = layer.GetLocationInfo(imageId);
          var recording = mappedFeature as Recording;

          if (recording != null)
          {
            if ((recording.IsAuthorized == null) || ((bool) recording.IsAuthorized))
            {
              FrmGlobespotter.ShowLocation(imageId, layer);
            }
            else
            {
              MessageBox.Show(Resources.GsOpenLocation_OnMouseUp_You_are_not_authorized_to_view_the_image_);
            }
          }
        }
      }
      catch(Exception ex)
      {
        _logClient.Error("GsOpenLocation.OnMouseUp", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsOpenLocation.OnMouseUp");
      }

      base.OnMouseUp(arg);
    }

    #endregion

    #region Functions

    /// <summary>
    /// This function calculates the imageId for location on the screen
    /// </summary>
    /// <param name="arg">The mouse arguments</param>
    /// <param name="layer">The layer where the point has been found</param>
    /// <returns>The imageId of the point</returns>
    private static string GetImageIdFromPoint(MouseEventArgs arg, out CycloMediaLayer layer)
    {
      layer = null;
      string result = string.Empty;
      GsExtension extension = GsExtension.GetExtension();

      if (extension.InsideScale())
      {
        int x = arg.X;
        int y = arg.Y;
        CycloMediaGroupLayer cycloMediaGroupLayer = extension.CycloMediaGroupLayer;
        result = cycloMediaGroupLayer.GetFeatureFromPoint(x, y, out layer);
      }

      return result;
    }

    #endregion
  }
}
