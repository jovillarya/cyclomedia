using System;
using System.Diagnostics;
using IntegrationArcMap.Forms;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button adds the historical data layer
  /// </summary>
  public class GsHelp : Button
  {
    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        FrmHelp.OpenCloseSwitch();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsHelp.OnClick");
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        GsExtension extension = GsExtension.GetExtension();
        Enabled = ((ArcMap.Application != null) && extension.Enabled);
        Checked = FrmHelp.IsVisible;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsHelp.OnUpdate");
      }
    }
  }
}
