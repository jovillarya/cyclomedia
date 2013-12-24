using System;
using System.Diagnostics;
using System.Windows.Forms;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Layers;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The settings button. Click on this button for show the settings
  /// </summary>
  public class GsRecordingHistory : Button
  {
    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        FrmRecordingHistory.OpenCloseSwitch();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, "GsRecordingHistory.OnClick");
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
        Trace.WriteLine(ex.Message, "GsRecordingHistory.OnUpdate");
      }
    }
  }
}
