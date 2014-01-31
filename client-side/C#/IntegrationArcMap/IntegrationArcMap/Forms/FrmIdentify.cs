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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ESRI.ArcGIS.Framework;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Forms
{
  public partial class FrmIdentify : UserControl
  {
    private static FrmIdentify _frmIdentify;
    private static IDockableWindow _window;

    public FrmIdentify()
    {
      InitializeComponent();

      Font font = SystemFonts.DefaultFont;
      lvFields.Font = (Font) font.Clone();
      txtFeatureClassName.Font = (Font) font.Clone();
    }

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    private static FrmIdentify Instance
    {
      get { return _frmIdentify ?? (_frmIdentify = new FrmIdentify()); }
    }

    private static IDockableWindow Window
    {
      get { return _window ?? (_window = GetDocWindow()); }
    }

    public static IntPtr FrmHandle
    {
      get { return Instance.Handle; }
    }

    #endregion

    #region functions (static)

    // =========================================================================
    // Static Functions
    // =========================================================================
    private static IDockableWindow GetDocWindow()
    {
      IApplication application = ArcMap.Application;
      ICommandItem tool = application.CurrentTool;
      const string windowName = "IntegrationArcMap_GsFrmIdentify";
      IDockableWindow result = ArcUtils.GetDocWindow(windowName);
      application.CurrentTool = tool;
      return result;
    }

    public static void DisposeFrm(bool disposing)
    {
      if (_frmIdentify != null)
      {
        _frmIdentify.Dispose(disposing);
      }
    }

    public static void Show(Dictionary<string, string> featureData)
    {
      if (!Window.IsVisible())
      {
        Window.Show(true);
      }

      Instance.Update(featureData);
    }

    public static void Close()
    {
      if (Window.IsVisible())
      {
        Window.Show(false);
      }
    }

    #endregion

    #region functions (private)

    private void Update(Dictionary<string, string> featureData)
    {
      lvFields.Items.Clear();

      foreach (var feature in featureData)
      {
        if ((feature.Key != "coordinates") && (!feature.Key.Contains("SrsDimension")))
        {
          if (feature.Key == "FEATURECLASSNAME")
          {
            txtFeatureClassName.Text = feature.Value;
          }
          else
          {
            var items = new[] {feature.Key, feature.Value};
            var listViewItem = new ListViewItem(items);
            lvFields.Items.Add(listViewItem);
          }
        }
      }
    }

    #endregion
  }
}
