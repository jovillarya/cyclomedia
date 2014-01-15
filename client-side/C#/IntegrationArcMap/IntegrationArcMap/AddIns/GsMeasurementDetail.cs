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
using ESRI.ArcGIS.Editor;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Properties;
using IntegrationArcMap.Utilities;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The Measurement detail button. Click on this button for show the Measurement detail docked window
  /// </summary>
  public class GsMeasurementDetail : Button
  {
    /// <summary>
    /// The name of the menu and the command item of this button
    /// </summary>
    private const string MenuItem = "esriEditor.EditingToolbarNew";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsMeasurementDetail";

    public GsMeasurementDetail()
    {
      Checked = false;
    }

    #region event handlers

    protected override void OnClick()
    {
      try
      {
        OnUpdate();

        if (Checked)
        {
          FrmMeasurement.Close();
        }
        else
        {
          FrmMeasurement.Open();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message, Resources.GsCycloMediaOptions_OnClick_Globespotter_integration_Addin_Error_);
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        Enabled = (editor.EditState == esriEditState.esriStateEditing);
        bool visible = FrmMeasurement.IsVisible();

        if (!Enabled && visible)
        {
          FrmMeasurement.Close();
          visible = false;
        }

        Checked = visible;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsMeasurementDetail.OnUpdate");
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
