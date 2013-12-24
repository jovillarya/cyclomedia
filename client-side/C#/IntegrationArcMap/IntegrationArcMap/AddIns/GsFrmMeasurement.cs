using System;
using ESRI.ArcGIS.Desktop.AddIns;
using IntegrationArcMap.Forms;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The settings docked window
  /// </summary>
  class GsFrmMeasurement : DockableWindow
  {
    protected override IntPtr OnCreateChild()
    {
      return FrmMeasurement.FrmHandle;
    }

    protected override void Dispose(bool disposing)
    {
      FrmMeasurement.DisposeFrm(disposing);
      base.Dispose(disposing);
    }
  }
}
