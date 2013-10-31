using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using IntegrationArcMap.Forms;
using IntegrationArcMap.Layers;
using ESRI.ArcGIS.Desktop.AddIns;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This is the tool for select a point on the screen and open a cyclorama
  /// </summary>
  public class GsOpenLocation : Tool
  {
    private Cursor _thisCursor;

    protected override void OnActivate()
    {
      if (_thisCursor == null)
      {
        Type thisType = GetType();
        string cursorPath = string.Format(@"IntegrationArcMap.Cursors.{0}.cur", thisType.Name);
        Assembly thisAssembly = Assembly.GetAssembly(thisType);
        Stream cursorStream = thisAssembly.GetManifestResourceStream(cursorPath);

        if (cursorStream != null)
        {
          _thisCursor = new Cursor(cursorStream);
        }
      }

      base.OnActivate();
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
        Trace.WriteLine(ex.Message, "GsOpenLocation.OnUpdate");
      }

      base.OnUpdate();
    }

    protected override void OnMouseMove(MouseEventArgs arg)
    {
      try
      {
        CycloMediaLayer layer;
        string imageId = GetImageIdFromPoint(arg, out layer);
        Cursor = (string.IsNullOrEmpty(imageId) || (layer == null)) ? _thisCursor : Cursors.Arrow;
      }
      catch (Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsOpenLocation.OnMouseMove");
      }

      base.OnMouseMove(arg);
    }

    protected override void OnMouseUp(MouseEventArgs arg)
    {
      try
      {
        CycloMediaLayer layer;
        string imageId = GetImageIdFromPoint(arg, out layer);

        if ((!string.IsNullOrEmpty(imageId)) && (layer != null))
        {
          FrmGlobespotter.ShowLocation(imageId, layer);
        }
      }
      catch(Exception ex)
      {
        Trace.WriteLine(ex.Message, "GsOpenLocation.OnMouseUp");
      }

      base.OnMouseUp(arg);
    }

    private static string GetImageIdFromPoint(MouseEventArgs arg, out CycloMediaLayer layer)
    {
      layer = null;
      string result = string.Empty;
      GsExtension extension = GsExtension.GetExtension();

      if (extension.InsideScale())
      {
        int x = arg.X;
        int y = arg.Y;
        CycloMediaGroupLayer cycloMediaGroupLayer = extension.CycloMediaGroupLayer;
        result = cycloMediaGroupLayer.GetFeatureFromPoint(x, y, out layer);
      }

      return result;
    }
  }
}
