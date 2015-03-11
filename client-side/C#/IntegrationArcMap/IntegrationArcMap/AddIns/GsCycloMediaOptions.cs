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
using System.Windows.Forms;
using IntegrationArcMap.Client;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Properties;
using IntegrationArcMap.Utilities;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The CycloMedia options button. Click on this button for show the CycloMedia options
  /// </summary>
  public class GsCycloMediaOptions : Button
  {
    /// <summary>
    /// The name of the menu and the command item of this button
    /// </summary>
    private const string MenuItem = "esriArcMapUI.MxCustomizeMenu";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsCycloMediaOptions";

    private readonly LogClient _logClient;

    public GsCycloMediaOptions()
    {
      _logClient = new LogClient(typeof(GsCycloMediaOptions));
    }

    #region event handlers

    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        FrmCycloMediaOptions.OpenCloseSwitch();
      }
      catch (Exception ex)
      {
        _logClient.Error("GsCycloMediaOptions.OnClick", ex.Message, ex);
        MessageBox.Show(ex.Message, Resources.GsCycloMediaOptions_OnClick_Globespotter_integration_Addin_Error_);
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        GsExtension extension = GsExtension.GetExtension();
        Enabled = ((ArcMap.Application != null) && extension.Enabled);
        Checked = FrmCycloMediaOptions.IsVisible;
      }
      catch (Exception ex)
      {
        _logClient.Error("GsCycloMediaOptions.OnUpdate", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsCycloMediaOptions.OnUpdate");
      }
    }

    #endregion

    #region add or remove button from the menu

    public static void AddToMenu()
    {
      ArcUtils.AddCommandItem(MenuItem, CommandItem);
    }

    public static void RemoveFromMenu()
    {
      ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
    }

    #endregion
  }
}
