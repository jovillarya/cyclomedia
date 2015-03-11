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
using IntegrationArcMap.Layers;
using IntegrationArcMap.Properties;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The recording history button. Click on this button for show the recording history range control.
  /// </summary>
  public class GsRecordingHistory : Button
  {
    private readonly LogClient _logClient;

    #region event handlers

    public GsRecordingHistory()
    {
      _logClient = new LogClient(typeof(GsRecordingHistory));
    }

    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        FrmRecordingHistory.OpenCloseSwitch();
      }
      catch (Exception ex)
      {
        _logClient.Error("GsRecordingHistory.OnClick", ex.Message, ex);
        MessageBox.Show(ex.Message, Resources.GsCycloMediaOptions_OnClick_Globespotter_integration_Addin_Error_);
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        GsExtension extension = GsExtension.GetExtension();

        if (extension != null)
        {
          CycloMediaGroupLayer cyclGroupLayer = extension.CycloMediaGroupLayer;
          bool historicalEnabled = (cyclGroupLayer != null) && cyclGroupLayer.HistoricalLayerEnabled;

          if (!historicalEnabled && FrmRecordingHistory.IsVisible)
          {
            FrmRecordingHistory.CloseForm();
          }

          Enabled = ((ArcMap.Application != null) && extension.Enabled && historicalEnabled);
          Checked = FrmRecordingHistory.IsVisible;
        }
      }
      catch (Exception ex)
      {
        _logClient.Error("GsRecordingHistory.OnUpdate", ex.Message, ex);
        Trace.WriteLine(ex.Message, "GsRecordingHistory.OnUpdate");
      }
    }

    #endregion
  }
}
