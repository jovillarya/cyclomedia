using System;
using System.Collections.Generic;
using System.Windows.Forms;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Model.Capabilities;
using IntegrationArcMap.Utilities;
using ESRI.ArcGIS.Framework;

namespace IntegrationArcMap.Forms
{
  public partial class FrmAddLayer : UserControl
  {
    // =========================================================================
    // Members
    // =========================================================================
    private enum CustomStep
    {
      SelectFormat = 1,
      SelectWfsLayer = 2,
      SelectCompatibleLayer = 3,
      SetViewingParameters = 4
    }

    private static FrmAddLayer _frmAddLayer;
    private CustomStep _customStep;
    private IList<WfsLayer> _wfsLayers;
    private WfsLayer _selectedLayer;
    private readonly List<string> _wfsStatus; 
    private readonly List<string> _wfsVersions;

    // =========================================================================
    // Constructor
    // =========================================================================
    public FrmAddLayer()
    {
      _wfsStatus = new List<string> {"WFS layers found at URL", "No WFS layers found at URL"};
      _wfsVersions = new List<string> {"1.0.0", "1.1.0"};
      InitializeComponent();

      foreach (var wfsVersion in _wfsVersions)
      {
        cbVersion.Items.Add(wfsVersion);
      }

      cbVersion.SelectedItem = _wfsVersions[1];
      _wfsLayers = null;
      _selectedLayer = null;
    }

    // =========================================================================
    // Properties
    // =========================================================================
    private static FrmAddLayer Instance
    {
      get { return _frmAddLayer ?? (_frmAddLayer = new FrmAddLayer()); }
    }

    public static IntPtr FrmHandle
    {
      get { return Instance.Handle; }
    }

    // =========================================================================
    // Functions
    // =========================================================================
    private static IDockableWindow GetDocWindow()
    {
      IApplication application = ArcMap.Application;
      ICommandItem tool = application.CurrentTool;
      const string windowName = "IntegrationArcMap_GsFrmAddLayer";
      IDockableWindow result = ArcUtils.GetDocWindow(windowName);
      application.CurrentTool = tool;
      return result;
    }

    public static void DisposeFrm(bool disposing)
    {
      if (_frmAddLayer != null)
      {
        _frmAddLayer.Dispose(disposing);
      }
    }

    public static void Open()
    {
      IDockableWindow window = GetDocWindow();

      if (window != null)
      {
        if (!window.IsVisible())
        {
          window.Show(true);
          Instance.SetNextStep(CustomStep.SelectFormat);
        }
      }
    }

    private static void Close()
    {
      IDockableWindow window = GetDocWindow();

      if (window != null)
      {
        if (window.IsVisible())
        {
          window.Show(false);
        }
      }
    }

    private void SetNextStep(CustomStep customStep)
    {
      _customStep = customStep;

      switch (_customStep)
      {
        case CustomStep.SelectFormat:
          btnBack.Enabled = false;
          btnFinish.Enabled = false;
          btnNext.Enabled = true;
          plSelectFormat.Visible = true;
          plSelectWfsLayer.Visible = false;
          plCompatibleLayer.Visible = false;
          plSetViewingParameters.Visible = false;
          rbWFS.Checked = true;
          break;
        case CustomStep.SelectWfsLayer:
          btnBack.Enabled = true;
          btnFinish.Enabled = false;
          btnNext.Enabled = (txtStatus.Text == _wfsStatus[0]);
          plSelectWfsLayer.Visible = true;
          plSelectFormat.Visible = false;
          plCompatibleLayer.Visible = false;
          plSetViewingParameters.Visible = false;
          break;
        case CustomStep.SelectCompatibleLayer:
          btnBack.Enabled = true;
          btnFinish.Enabled = false;
          btnNext.Enabled = false;
          plSelectWfsLayer.Visible = false;
          plSelectFormat.Visible = false;
          plCompatibleLayer.Visible = true;
          plSetViewingParameters.Visible = false;
          lvLayers.Items.Clear();

          if (_wfsLayers != null)
          {
            foreach (var wfsLayer in _wfsLayers)
            {
              if (wfsLayer != null)
              {
                string epsgCode = ArcUtils.EpsgCode;
                string onMap = wfsLayer.ContainsSrsName(epsgCode) ? "Yes" : "No";
                string onCycl = wfsLayer.ContainsSrsName(epsgCode) ? "Yes" : "No";
                var items = new[] {wfsLayer.ToString(), onMap, onCycl};
                lvLayers.Items.Add(new ListViewItem(items) {Tag = wfsLayer});
              }
            }
          }

          break;
        case CustomStep.SetViewingParameters:
          btnBack.Enabled = true;
          btnFinish.Enabled = true;
          btnNext.Enabled = false;
          plSelectWfsLayer.Visible = false;
          plSelectFormat.Visible = false;
          plCompatibleLayer.Visible = false;
          plSetViewingParameters.Visible = true;
          FeatureType featureType = _selectedLayer.FeatureType;

          if (featureType != null)
          {
            var name = featureType.Name;

            if (name != null)
            {
              txtLayerName.Text = name.Value;
            }
          }

          break;
      }
    }

    // =========================================================================
    // Event handlers
    // =========================================================================
    private void btnCancel_Click(object sender, EventArgs e)
    {
      IDockableWindow window = GetDocWindow();

      if (window != null)
      {
        window.Show(false);
      }
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
      switch (_customStep)
      {
        case CustomStep.SelectFormat:
          if (rbWFS.Checked)
          {
            SetNextStep(CustomStep.SelectWfsLayer);
          }
          break;
        case CustomStep.SelectWfsLayer:
          SetNextStep(CustomStep.SelectCompatibleLayer);
          break;
        case CustomStep.SelectCompatibleLayer:
          SetNextStep(CustomStep.SetViewingParameters);
          break;
      }
    }

    private void btnBack_Click(object sender, EventArgs e)
    {
      switch (_customStep)
      {
        case CustomStep.SelectWfsLayer:
          SetNextStep(CustomStep.SelectFormat);
          break;
        case CustomStep.SelectCompatibleLayer:
          SetNextStep(CustomStep.SelectWfsLayer);
          break;
        case CustomStep.SetViewingParameters:
          SetNextStep(CustomStep.SelectCompatibleLayer);
          break;
      }
    }

    private void btnLoad_Click(object sender, EventArgs e)
    {
      if (!string.IsNullOrEmpty(txtSelectWfsLayerLocation.Text))
      {
        try
        {
          _wfsLayers = WfsLayer.GetCapabilities(txtSelectWfsLayerLocation.Text, cbVersion.SelectedItem.ToString());
        }
        catch
        {
          _wfsLayers = null;
        }
      }

      if ((_wfsLayers != null) && (_wfsLayers.Count >= 1))
      {
        txtStatus.Text = _wfsStatus[0];
        btnNext.Enabled = true;
      }
      else
      {
        txtStatus.Text = _wfsStatus[1];
        btnNext.Enabled = false;
      }
    }

    private void btnFinish_Click(object sender, EventArgs e)
    {
      if (_selectedLayer != null)
      {
        _selectedLayer.AddToGlobespotter(txtLayerName.Text, decimal.ToInt32(updZoomLevel.Value),
                                         ckUseCycloMediaProxy.Checked, btnColor.BackColor);
        Close();
        _wfsLayers = null;
        _selectedLayer = null;
        txtStatus.Text = string.Empty;
      }
    }

    private void btnColor_Click(object sender, EventArgs e)
    {
      if (cdWfsLayer.ShowDialog() == DialogResult.OK)
      {
        btnColor.BackColor = cdWfsLayer.Color;
      }
    }

    private void lvLayers_SelectedIndexChanged(object sender, EventArgs e)
    {
      if (lvLayers.SelectedItems.Count == 1)
      {
        _selectedLayer = lvLayers.SelectedItems[0].Tag as WfsLayer;
      }
      
      btnNext.Enabled = _selectedLayer != null;
    }
  }
}
