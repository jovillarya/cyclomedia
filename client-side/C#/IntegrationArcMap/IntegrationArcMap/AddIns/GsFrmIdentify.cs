/*
 * Integration in ArcMap for Cycloramas
 * Copyright (c) 2014, CycloMedia, All rights reserved.
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3.0 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library.
 */

using System;
using ESRI.ArcGIS.Desktop.AddIns;
using IntegrationArcMap.Forms;

namespace IntegrationArcMap.AddIns
{
  /// <summary>
  /// The identify docked window
  /// </summary>
  class GsFrmIdentify : DockableWindow
  {
    protected override IntPtr OnCreateChild()
    {
      return FrmIdentify.FrmHandle;
    }

    protected override void Dispose(bool disposing)
    {
      FrmIdentify.DisposeFrm(disposing);
      base.Dispose(disposing);
    }
  }
}
