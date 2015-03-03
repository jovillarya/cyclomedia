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
using System.Globalization;
using System.Windows.Forms;
using System.Xml.Linq;
using System.ComponentModel;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Client;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Model;
using IntegrationArcMap.Model.Atlas;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Forms
{
  public partial class FrmCycloramaSearch : Form
  {
    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static FrmCycloramaSearch _frmCycloramaSearch;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public FrmCycloramaSearch()
    {
      // Initialize
      InitializeComponent();

      // Fonts
      Font font = SystemFonts.DefaultFont;
      txtImageId.Font = (Font) font.Clone();
      lblImageId.Font = (Font) font.Clone();
      tbCycloramaSearch.Font = (Font) font.Clone();
      btnFind.Font = (Font) font.Clone();
      btnCancel.Font = (Font) font.Clone();
      lvResults.Font = (Font) font.Clone();
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public static bool IsVisible
    {
      get { return _frmCycloramaSearch != null; }
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    public static void OpenCloseSwitch()
    {
      if (_frmCycloramaSearch == null)
      {
        OpenForm();
      }
      else
      {
        CloseForm();
      }
    }

    public static void CloseForm()
    {
      if (_frmCycloramaSearch != null)
      {
        _frmCycloramaSearch.Close();
      }
    }

    private static void OpenForm()
    {
      if (_frmCycloramaSearch == null)
      {
        _frmCycloramaSearch = new FrmCycloramaSearch();
        var application = ArcMap.Application;
        int hWnd = application.hWnd;
        IWin32Window parent = new WindowWrapper(hWnd);
        _frmCycloramaSearch.Show(parent);
      }
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Eventhandlers
    // =========================================================================
    private void BtnFind_Click(object sender, EventArgs e)
    {
      backgroundImageSearch.RunWorkerAsync();
      lvResults.Items.Clear();
      btnFind.Enabled = false;
      btnCancel.Enabled = false;
      prSearching.Visible = true;
      txtImageId.Enabled = false;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void FrmCycloramaSearch_FormClosed(object sender, FormClosedEventArgs e)
    {
      _frmCycloramaSearch = null;
    }

    private void FrmCycloramaSearch_FormClosing(object sender, FormClosingEventArgs e)
    {
      if (backgroundImageSearch.IsBusy)
      {
        e.Cancel = true;
      }
    }

    private void lvResults_DoubleClick(object sender, EventArgs e)
    {
      foreach (ListViewItem selectedItem in lvResults.SelectedItems)
      {
        var tag = selectedItem.Tag as object[];

        if ((tag != null) && (tag.Length >= 2) && (selectedItem.SubItems.Count >= 1))
        {
          var feature = tag[0] as IFeature;
          var cycloMediaLayer = tag[1] as CycloMediaLayer;
          IActiveView activeView = ArcUtils.ActiveView;
          ListViewItem.ListViewSubItem item = selectedItem.SubItems[0];

          if ((feature != null) && (cycloMediaLayer != null) && (activeView != null) && (item != null))
          {
            var point = feature.Shape as IPoint;

            if (point != null)
            {
              string imageId = item.Text;
              FrmGlobespotter.ShowLocation(imageId, cycloMediaLayer);
              IEnvelope envelope = activeView.Extent;
              envelope.CenterAt(point);
              activeView.Extent = envelope;
              activeView.Refresh();
            }
          }
        }
      }
    }

    private void backgroundImageSearch_DoWork(object sender, DoWorkEventArgs e)
    {
      Web web = Web.Instance;
      string imageId = txtImageId.Text;
      GsExtension extension = GsExtension.GetExtension();
      CycloMediaGroupLayer groupLayer = extension.CycloMediaGroupLayer;
      IList<CycloMediaLayer> layers = groupLayer.Layers;

      foreach (var layer in layers)
      {
        try
        {
          List<XElement> featureMemberElements = web.GetByImageId(imageId, layer);
          e.Result = featureMemberElements;
        }
        catch (Exception)
        {
          e.Cancel = true;
        }
      }
    }

    private void backgroundImageSearch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      if (!e.Cancelled)
      {
        var featureMemberElements = e.Result as List<XElement>;

        if (featureMemberElements != null)
        {
          string imageId = txtImageId.Text;
          GsExtension extension = GsExtension.GetExtension();
          CycloMediaGroupLayer groupLayer = extension.CycloMediaGroupLayer;
          IList<CycloMediaLayer> layers = groupLayer.Layers;

          foreach (var layer in layers)
          {
            if (layer.IsVisible)
            {
              layer.SaveFeatureMembers(featureMemberElements, null);
              IMappedFeature mappedFeature = layer.GetLocationInfo(imageId);
              var recording = mappedFeature as Recording;

              if (recording != null)
              {
                DateTime? recordedAt = recording.RecordedAt;
                CultureInfo ci = CultureInfo.InvariantCulture;
                string recordedAtString = (recordedAt == null) ? string.Empty : ((DateTime) recordedAt).ToString(ci);
                string imageIdString = recording.ImageId;
                IFeature feature = layer.GetFeature(imageId);
                var items = new[] {imageIdString, recordedAtString};
                var listViewItem = new ListViewItem(items) {Tag = new object[] {feature, layer}};
                lvResults.Items.Add(listViewItem);
              }
            }
          }
        }
      }

      btnFind.Enabled = true;
      btnCancel.Enabled = true;
      prSearching.Visible = false;
      txtImageId.Enabled = true;
    }

    #endregion
  }
}
