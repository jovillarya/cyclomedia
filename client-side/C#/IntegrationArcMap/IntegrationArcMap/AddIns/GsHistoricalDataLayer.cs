using System;
using System.Diagnostics;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Desktop.AddIns;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button adds the historical data layer
  /// </summary>
  public class GsHistoricalDataLayer : Button
  {
    private const string LayerName = "Historical Recordings";
    private const string MenuItem = "esriArcMapUI.MxAddDataMenu";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsHistoricalDataLayer";

    public GsHistoricalDataLayer()
    {
      Checked = false;
      CycloMediaLayer.LayerAddedEvent += CycloMediaLayerAdded;
      CycloMediaLayer.LayerRemoveEvent += CycloMediaLayerRemoved;
    }

    protected override void OnClick()
    {
      try
      {
        OnUpdate();
        GsExtension extension = GsExtension.GetExtension();

        if (Checked)
        {
          extension.RemoveLayer(LayerName);
        }
        else
        {
          extension.AddLayers(LayerName);
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsHistoricalDataLayers.OnClick");
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
        Trace.WriteLine(ex.Message, "GsHistoricalDataLayers.OnUpdate");
      }
    }

    private void CycloMediaLayerAdded(CycloMediaLayer layer)
    {
      try
      {
        if (layer != null)
        {
          Checked = (layer.Name == LayerName) || Checked;
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsHistoricalDataLayers.CycloMediaLayerAdded");
      }
    }

    private void CycloMediaLayerRemoved(CycloMediaLayer layer)
    {
      try
      {
        if (layer != null)
        {
          Checked = (layer.Name != LayerName) && Checked;
        }
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsHistoricalDataLayers.CycloMediaRemoved");
      }
    }

    public static void AddToMenu()
    {
      ArcUtils.AddCommandItem(MenuItem, CommandItem, 1);
    }

    public static void RemoveFromMenu()
    {
      ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
    }
  }
}
