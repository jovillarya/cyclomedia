using System;
using System.Diagnostics;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Utilities;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button adds the historical data layer
  /// </summary>
  public class GsAddLayer : Button
  {
    private const string MenuItem = "esriArcMapUI.MapViewContextMenu";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsAddLayer";

    protected override void OnClick()
    {
      try
      {
        FrmAddLayer.Open();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsAddLayer.OnClick");
      }
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
        Trace.WriteLine(ex.Message, "GsAddLayer.OnUpdate");
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
