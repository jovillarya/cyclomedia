using System;
using IntegrationArcMap.Forms;
using ESRI.ArcGIS.Desktop.AddIns;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// This is the form for can select additional layers to the map
  /// </summary>
  class GsFrmAddLayer : DockableWindow
  {
    protected override IntPtr OnCreateChild()
    {
      return FrmAddLayer.FrmHandle;
    }

    protected override void Dispose(bool disposing)
    {
      FrmAddLayer.DisposeFrm(disposing);
      base.Dispose(disposing);
    }
  }
}
