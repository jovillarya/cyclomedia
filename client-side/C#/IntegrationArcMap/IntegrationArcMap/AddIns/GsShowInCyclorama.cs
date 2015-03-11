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
using System.Diagnostics;
using IntegrationArcMap.Client;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button can be used for show the vector layer in the cyclorama.
  /// </summary>
  public class GsShowInCyclorama : Button
  {
    /// <summary>
    /// The name of the menu and the command item of this button
    /// </summary>
    private const string MenuItem = "esriArcMapUI.FeatureLayerContextMenu";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsShowInCyclorama";

    #region members

    private CycloMediaLayer _cycloMediaLayer;
    private VectorLayer _vectorLayer;

    private readonly LogClient _logClient;

    #endregion

    public GsShowInCyclorama()
    {
      _logClient = new LogClient(typeof (GsShowInCyclorama));
    }

    #region event handlers

    protected override void OnClick()
    {
      try
      {
        Checked = !Checked;

        if (_cycloMediaLayer != null)
        {
          _cycloMediaLayer.IsVisibleInGlobespotter = Checked;
        }

        if (_vectorLayer != null)
        {
          _vectorLayer.IsVisibleInGlobespotter = Checked;
        }
      }
      catch (Exception ex)
      {
        _logClient.Error("GsShowInCyclorama.OnClick", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsShowInCyclorama.OnClick");
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        GsExtension extension = GsExtension.GetExtension();
        IApplication application = ArcMap.Application;
        Enabled = ((application != null) && extension.Enabled);

        if (application != null)
        {
          var document = application.Document as IMxDocument;

          if (document != null)
          {
            var tocDisplayView = document.CurrentContentsView as TOCDisplayView;

            if (tocDisplayView != null)
            {
              var selectedItem = tocDisplayView.SelectedItem as ILayer;

              if (selectedItem != null)
              {
                _vectorLayer = VectorLayer.GetLayer(selectedItem);
                CycloMediaGroupLayer cycloMediaGroupLayer = extension.CycloMediaGroupLayer;
                _cycloMediaLayer = (cycloMediaGroupLayer == null) ? null : cycloMediaGroupLayer.GetLayer(selectedItem);

                if (_cycloMediaLayer != null)
                {
                  Checked = _cycloMediaLayer.IsVisibleInGlobespotter;
                  Enabled = _cycloMediaLayer.IsVisible;
                }

                if (_vectorLayer != null)
                {
                  Checked = _vectorLayer.IsVisibleInGlobespotter;
                  Enabled = _vectorLayer.IsVisible;
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        _logClient.Error("GsShowInCyclorama.OnUpdate", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsShowInCyclorama.OnUpdate");
      }
    }

    #endregion

    #region add or remove button from the menu

    public static void AddToMenu()
    {
      ArcUtils.AddCommandItem(MenuItem, CommandItem, 0);
    }

    public static void RemoveFromMenu()
    {
      ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
    }

    #endregion
  }
}
