using System;
using IntegrationArcMap.Forms;
using ESRI.ArcGIS.Desktop.AddIns;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The settings docked window
  /// </summary>
  class GsFrmSmartClick : DockableWindow
  {
    protected override IntPtr OnCreateChild()
    {
      return FrmSmartClick.FrmHandle;
    }

    protected override void Dispose(bool disposing)
    {
      FrmSmartClick.DisposeFrm(disposing);
      base.Dispose(disposing);
    }
  }
}
