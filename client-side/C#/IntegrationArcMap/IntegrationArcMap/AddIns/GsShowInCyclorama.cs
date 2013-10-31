using System;
using System.Diagnostics;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using Button = ESRI.ArcGIS.Desktop.AddIns.Button;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This button adds the historical data layer
  /// </summary>
  public class GsShowInCyclorama : Button
  {
    private const string MenuItem = "esriArcMapUI.FeatureLayerContextMenu";
    private const string CommandItem = "CycloMedia_IntegrationArcMap_GsShowInCyclorama";

    private CycloMediaLayer _cycloMediaLayer;
    private VectorLayer _vectorLayer;

    public GsShowInCyclorama()
    {
      Checked = false;
    }

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
        Trace.WriteLine(ex.Message, "GsShowInCyclorama.OnUpdate");
      }
    }

    public static void AddToMenu()
    {
      ArcUtils.AddCommandItem(MenuItem, CommandItem, 0);
    }

    public static void RemoveFromMenu()
    {
      ArcUtils.RemoveCommandItem(MenuItem, CommandItem);
    }
  }
}
