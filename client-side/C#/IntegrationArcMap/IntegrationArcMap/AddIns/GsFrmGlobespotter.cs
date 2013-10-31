using System;
using IntegrationArcMap.Forms;
using ESRI.ArcGIS.Desktop.AddIns;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The globespotter docked window
  /// </summary>
  class GsFrmGlobespotter : DockableWindow
  {
    protected override IntPtr OnCreateChild()
    {
      return FrmGlobespotter.FrmHandle;
    }

    protected override void Dispose(bool disposing)
    {
      FrmGlobespotter.DisposeFrm(disposing);
      base.Dispose(disposing);
    }
  }
}
