using System;
using System.Diagnostics;
using System.Windows.Forms;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Utilities;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The settings button. Click on this button for show the settings
  /// </summary>
  public class GsCycloMediaOptions : Button
  {
    private const string MenuItem = "esriArcMapUI.MxCustomizeMenu";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsCycloMediaOptions";

    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        FrmCycloMediaOptions.OpenCloseSwitch();
      }
      catch (Exception e)
      {
        MessageBox.Show(e.Message, "Globespotter integration Addin Error.");
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
        Trace.WriteLine(ex.Message, "GsCycloMediaOptions.OnUpdate");
      }
    }

    public static void AddToMenu()
    {
      ArcUtils.AddCommandItem(MenuItem, CommandItem);
    }

    public static void RemoveFromMenu()
    {
      ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
    }
  }
}
