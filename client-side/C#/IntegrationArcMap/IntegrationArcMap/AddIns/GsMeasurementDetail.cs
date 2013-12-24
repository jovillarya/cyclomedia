using System;
using System.Diagnostics;
using ESRI.ArcGIS.Editor;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Utilities;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button adds the historical data layer
  /// </summary>
  public class GsMeasurementDetail : Button
  {
    private const string MenuItem = "esriEditor.EditingToolbarNew";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsMeasurementDetail";

    public GsMeasurementDetail()
    {
      Checked = false;
    }

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
        Trace.WriteLine(ex.Message, "GsMeasurementDetail.OnClick");
      }
    }

    protected override void OnUpdate()
    {
      try
      {
        IEditor3 editor = ArcUtils.Editor;
        Enabled = (editor.EditState == esriEditState.esriStateEditing);
        Checked = FrmMeasurement.IsVisible();
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsMeasurementDetail.OnUpdate");
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
