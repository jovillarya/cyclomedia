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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Client;
using IntegrationArcMap.Layers;
using IntegrationArcMap.Utilities;

namespace IntegrationArcMap.Forms
{
  public partial class FrmCycloMediaOptions : Form
  {
    // =========================================================================
    // Login failed / successfully text
    // =========================================================================
    private const string LoginFailed = "Login Failed";
    private const string LoginSuccessfully = "Login Successfully";

    private const string MeasuringSupported = "Measuring supported";
    private const string MeasuringNotSupported = "Measuring not supported";

    #region members

    // =========================================================================
    // Members
    // =========================================================================
    private static FrmCycloMediaOptions _frmCycloMediaOptions;
    private static Login _login;

    private Config _config;
    private bool _mssgBoxShow;

    #endregion

    #region constructor

    // =========================================================================
    // Constructor
    // =========================================================================
    public FrmCycloMediaOptions()
    {
      // Initialize
      InitializeComponent();
      _mssgBoxShow = false;
      _config = Config.Instance;
      GsExtension.OpenDocumentEvent += LoginReload;

      // Fonts
      Font font = SystemFonts.MenuFont;
      tcSettings.Font = (Font) font.Clone();
      txtPassword.Font = (Font) font.Clone();
      txtUsername.Font = (Font) font.Clone();
      nudMaxViewers.Font = (Font) font.Clone();
      nudDistVectLayerViewer.Font = (Font) font.Clone();
      txtBaseUrlLocation.Font = (Font) font.Clone();
      txtRecordingServiceLocation.Font = (Font) font.Clone();
      txtSwfUrlLocation.Font = (Font) font.Clone();
      grCoordinateSystems.Font = (Font) font.Clone();
      grGeneral.Font = (Font) font.Clone();
      grBaseUrl.Font = (Font) font.Clone();
      grRecordingService.Font = (Font) font.Clone();
      grSwfUrl.Font = (Font) font.Clone();
      cbSpatialReferences.Font = (Font) font.Clone();
      txtAgreement.Font = (Font) font.Clone();
      grLogin.Font = (Font) font.Clone();
      grStatus.Font = (Font) font.Clone();
      grProxyServer.Font = (Font) font.Clone();
      txtProxyAddress.Font = (Font) font.Clone();
      txtProxyPort.Font = (Font) font.Clone();
      txtProxyUsername.Font = (Font) font.Clone();
      txtProxyPassword.Font = (Font) font.Clone();
      txtProxyDomain.Font = (Font) font.Clone();

      // Assembly info
      Type type = GetType();
      Assembly assembly = type.Assembly;
      string location = assembly.Location;
      FileVersionInfo info = FileVersionInfo.GetVersionInfo(location);
      AssemblyName assName = assembly.GetName();

      // Version info
      string product = info.ProductName;
      string copyright = info.LegalCopyright;
      Version version = assName.Version;
      rtbAbout.Text = string.Format("{0}{3}{1}{3}Version: {2}.{3}http://www.cyclomedia.com/", product, copyright, version, Environment.NewLine);

      // Agreement
      const string agreementPath = "IntegrationArcMap.Doc.Agreement.txt";
      Stream agreementStream = assembly.GetManifestResourceStream(agreementPath);

      if (agreementStream != null)
      {
        var reader = new StreamReader(agreementStream);
        string agreement = reader.ReadToEnd();
        reader.Close();
        txtAgreement.Text = agreement;
      }

      // Initialize
      btnApply.Enabled = false;
      btnOk.Select();
    }

    static FrmCycloMediaOptions()
    {
      _login = Client.Login.Instance;
    }

    #endregion

    #region properties

    // =========================================================================
    // Properties
    // =========================================================================
    public static bool IsVisible
    {
      get { return _frmCycloMediaOptions != null; }
    }

    #endregion

    #region functions

    // =========================================================================
    // Functions
    // =========================================================================
    public static void OpenCloseSwitch()
    {
      if (_frmCycloMediaOptions == null)
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
      if (_frmCycloMediaOptions != null)
      {
        _frmCycloMediaOptions.Close();
      }
    }

    public static void OpenIfNoCredentials()
    {
      if (!_login.Credentials)
      {
        OpenForm();
      }
    }

    public static void CheckOpenCredentials()
    {
      bool credentials = _login.Check();

      if (!credentials)
      {
        OpenForm();
      }
    }

    public static void ReloadData()
    {
      if (_frmCycloMediaOptions != null)
      {
        _frmCycloMediaOptions.LoadData();
      }
    }

    private static void OpenForm()
    {
      if (_frmCycloMediaOptions == null)
      {
        _frmCycloMediaOptions = new FrmCycloMediaOptions();
        var application = ArcMap.Application;
        int hWnd = application.hWnd;
        IWin32Window parent = new WindowWrapper(hWnd);
        _frmCycloMediaOptions.Show(parent);
      }
    }

    private void LoadData()
    {
      lblLoginStatus.Text = _login.Credentials ? LoginSuccessfully : LoginFailed;

      bool apply = btnApply.Enabled;
      ckDefaultBaseUrl.Checked = _config.BaseUrlDefault;
      ckDefaultRecordingService.Checked = _config.RecordingsServiceDefault;
      ckDefaultSwfUrl.Checked = _config.SwfUrlDefault;
      ckUseProxyServer.Checked = _config.UseProxyServer;
      ckUseDefaultProxyCredentials.Checked = (!_config.UseProxyServer) || _config.ProxyUseDefaultCredentials;
      txtBaseUrlLocation.Text = _config.BaseUrlDefault ? string.Empty : _config.BaseUrl;
      txtRecordingServiceLocation.Text = _config.RecordingsServiceDefault ? string.Empty : _config.RecordingsService;
      txtSwfUrlLocation.Text = _config.SwfUrlDefault ? string.Empty : _config.SwfUrl;
      FillInProxyParameters();
      txtBaseUrlLocation.Enabled = !_config.BaseUrlDefault;
      txtRecordingServiceLocation.Enabled = !_config.RecordingsServiceDefault;
      txtSwfUrlLocation.Enabled = !_config.SwfUrlDefault;
      btnApply.Enabled = apply;

      nudMaxViewers.Value = _config.MaxViewers;
      nudDistVectLayerViewer.Value = _config.DistanceCycloramaVectorLayer;
      txtPassword.Text = _login.Password;
      txtUsername.Text = _login.Username;
      ckEnableSmartClick.Checked = (GlobeSpotterConfiguration.MeasureSmartClick && _config.SmartClickEnabled);
      ckEnableSmartClick.Enabled = GlobeSpotterConfiguration.MeasureSmartClick;
      ckDetailImages.Checked = _config.DetailImagesEnabled;
      cbSpatialReferences.Items.Clear();
      SpatialReferences spatialReferences = SpatialReferences.Instance;

      foreach (var spatialReference in spatialReferences)
      {
        if (spatialReference.KnownInArcMap)
        {
          cbSpatialReferences.Items.Add(spatialReference);
        }
      }

      SpatialReference configSpat = _config.SpatialReference;
      SpatialReference spatialRef =
        spatialReferences.GetItem((configSpat == null) ? ArcUtils.EpsgCode : configSpat.SRSName);
      cbSpatialReferences.SelectedItem = spatialRef;
      _config.SpatialReference = spatialRef;

      if (!_login.Credentials)
      {
        txtUsername.Select();
      }
    }

    private void FillInProxyParameters()
    {
      txtProxyAddress.Text = ckUseProxyServer.Checked ? _config.ProxyAddress : string.Empty;
      txtProxyPort.Text = ckUseProxyServer.Checked ? _config.ProxyPort.ToString(CultureInfo.InvariantCulture) : string.Empty;
      ckBypassProxyOnLocal.Checked = ckUseProxyServer.Checked && _config.BypassProxyOnLocal;
      ckUseDefaultProxyCredentials.Checked = (!ckUseProxyServer.Checked) || ckUseDefaultProxyCredentials.Checked;
      txtProxyUsername.Text = ((!ckUseProxyServer.Checked) || ckUseDefaultProxyCredentials.Checked)
        ? string.Empty
        : _config.ProxyUsername;
      txtProxyPassword.Text = ((!ckUseProxyServer.Checked) || ckUseDefaultProxyCredentials.Checked)
        ? string.Empty
        : _config.ProxyPassword;
      txtProxyDomain.Text = ((!ckUseProxyServer.Checked) || ckUseDefaultProxyCredentials.Checked)
        ? string.Empty
        : _config.ProxyDomain;

      txtProxyAddress.Enabled = ckUseProxyServer.Checked;
      txtProxyPort.Enabled = ckUseProxyServer.Checked;
      ckBypassProxyOnLocal.Enabled = ckUseProxyServer.Checked;
      ckUseDefaultProxyCredentials.Enabled = ckUseProxyServer.Checked;
      txtProxyUsername.Enabled = ckUseProxyServer.Checked && (!ckUseDefaultProxyCredentials.Checked);
      txtProxyPassword.Enabled = ckUseProxyServer.Checked && (!ckUseDefaultProxyCredentials.Checked);
      txtProxyDomain.Enabled = ckUseProxyServer.Checked && (!ckUseDefaultProxyCredentials.Checked);
    }

    private void FrmSettingsLoad()
    {
      lblLoginStatus.Text = _login.Check() ? LoginSuccessfully : LoginFailed;
      LoadData();
      txtUsername.Focus();
    }

    private void LoginReload()
    {
      _login = Client.Login.Load();
      _config = Config.Load();
      lblLoginStatus.Text = _login.Check() ? LoginSuccessfully : LoginFailed;
      LoadData();
    }

    private bool Login()
    {
      bool result = false;
      _login.SetLoginCredentials(txtUsername.Text, txtPassword.Text);

      if (_login.Check())
      {
        FrmGlobespotter.LoginSuccesfull();
        result = true;
      }
      else
      {
        GlobeSpotterConfiguration conf = GlobeSpotterConfiguration.Instance;

        if (conf.LoginFailed)
        {
          txtUsername.Focus();
        }

        if (conf.LoadException)
        {
          MessageBox.Show(string.Format("Exception occurred: {0}.", conf.Exception.Message));
          _mssgBoxShow = true;
        }
      }

      lblLoginStatus.Text = _login.Credentials ? LoginSuccessfully : LoginFailed;
      return result;
    }

    private void Save(bool close)
    {
      // determinate smart click permissions
      bool usernameChanged = (txtUsername.Text != _login.Username) || (txtPassword.Text != _login.Password);

      // determinate restart
      bool baseUrlChanged = (txtBaseUrlLocation.Text != _config.BaseUrl) ||
                            (ckDefaultBaseUrl.Checked != _config.BaseUrlDefault);
      bool recordingServiceChanged = (txtRecordingServiceLocation.Text != _config.RecordingsService) ||
                                     (ckDefaultRecordingService.Checked != _config.RecordingsServiceDefault);
      bool swfChanged = (_config.SwfUrlDefault != ckDefaultSwfUrl.Checked) || (_config.SwfUrl != txtSwfUrlLocation.Text);
      SpatialReference spat = _config.SpatialReference;
      var selectedItem = (SpatialReference) cbSpatialReferences.SelectedItem;
      bool spatChanged = (spat == null) || ((selectedItem != null) && (spat.ToString() != selectedItem.ToString()));
      bool restart = usernameChanged || baseUrlChanged || swfChanged || spatChanged || recordingServiceChanged;

      // Save values
      int proxyPort;
      bool proxyParsed = int.TryParse(txtProxyPort.Text, out proxyPort);
      int useProxyPort = proxyParsed ? proxyPort : 80;
      var maxViewers = (uint) nudMaxViewers.Value;
      var distLayer = (uint) nudDistVectLayerViewer.Value;
      bool smartClickEnabled = GlobeSpotterConfiguration.MeasureSmartClick && (!usernameChanged)
        ? ckEnableSmartClick.Checked
        : _config.SmartClickEnabled;

      bool proxyChanged = (ckUseProxyServer.Checked != _config.UseProxyServer);
      proxyChanged = proxyChanged || (_config.ProxyAddress != txtProxyAddress.Text);
      proxyChanged = proxyChanged || (_config.ProxyPort != useProxyPort);
      proxyChanged = proxyChanged || (_config.BypassProxyOnLocal != ckBypassProxyOnLocal.Checked);
      proxyChanged = proxyChanged || (_config.ProxyUseDefaultCredentials != ckUseDefaultProxyCredentials.Checked);
      proxyChanged = proxyChanged || (_config.ProxyUsername != txtProxyUsername.Text);
      proxyChanged = proxyChanged || (_config.ProxyPassword != txtProxyPassword.Text);
      proxyChanged = proxyChanged || (_config.ProxyDomain != txtProxyDomain.Text);

      _config.SpatialReference = selectedItem ?? _config.SpatialReference;
      _config.MaxViewers = maxViewers;
      _config.DistanceCycloramaVectorLayer = distLayer;
      _config.BaseUrl = txtBaseUrlLocation.Text;
      _config.RecordingsService = txtRecordingServiceLocation.Text;
      _config.SwfUrl = txtSwfUrlLocation.Text;
      _config.BaseUrlDefault = ckDefaultBaseUrl.Checked;
      _config.RecordingsServiceDefault = ckDefaultRecordingService.Checked;
      _config.SwfUrlDefault = ckDefaultSwfUrl.Checked;
      _config.SmartClickEnabled = smartClickEnabled;
      _config.DetailImagesEnabled = ckDetailImages.Checked;
      _config.UseProxyServer = ckUseProxyServer.Checked;
      _config.ProxyAddress = txtProxyAddress.Text;
      _config.ProxyPort = useProxyPort;
      _config.BypassProxyOnLocal = ckBypassProxyOnLocal.Checked;
      _config.ProxyUseDefaultCredentials = ckUseDefaultProxyCredentials.Checked;
      _config.ProxyUsername = txtProxyUsername.Text;
      _config.ProxyPassword = txtProxyPassword.Text;
      _config.ProxyDomain = txtProxyDomain.Text;
      _config.Save();

      // Check restart GlobeSpotter
      bool loginSucces = (usernameChanged || baseUrlChanged || proxyChanged) && Login();

      if (proxyChanged)
      {
        ckEnableSmartClick.Checked = (GlobeSpotterConfiguration.MeasureSmartClick && _config.SmartClickEnabled);
        ckEnableSmartClick.Enabled = GlobeSpotterConfiguration.MeasureSmartClick;
      }

      if (_login.Credentials || loginSucces)
      {
        if (restart && FrmGlobespotter.IsStarted())
        {
          FrmGlobespotter.Restart();
        }
        else
        {
          FrmGlobespotter.UpdateParameters();
        }
      }

      // Close form
      if (close)
      {
        Close();
      }

      // Check if the layer has to make empty
      if (usernameChanged)
      {
        GsExtension extension = GsExtension.GetExtension();
        CycloMediaGroupLayer groupLayer = extension.CycloMediaGroupLayer;

        if (groupLayer != null)
        {
          groupLayer.MakeEmpty();
        }
      }

      btnApply.Enabled = false;
    }

    #endregion

    #region event handlers

    // =========================================================================
    // Eventhandlers
    // =========================================================================
    private void FrmCycloMediaOptions_Load(object sender, EventArgs e)
    {
      FrmSettingsLoad();
    }

    private void BtnOk_Click(object sender, EventArgs e)
    {
      Save(true);
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnApply_Click(object sender, EventArgs e)
    {
      Save(false);
    }

    private void TxtUsername_KeyUp(object sender, KeyEventArgs e)
    {
      if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtUsername.Text))) && (!_mssgBoxShow))
      {
        txtPassword.Focus();
      }
      else
      {
        _mssgBoxShow = false;
      }

      btnApply.Enabled = true;
    }

    private void TxtPassword_KeyUp(object sender, KeyEventArgs e)
    {
      if (((e.KeyCode == Keys.Enter) && (!string.IsNullOrEmpty(txtPassword.Text))) && (!_mssgBoxShow))
      {
        Login();
      }
      else
      {
        _mssgBoxShow = false;
      }

      btnApply.Enabled = true;
    }

    private void FrmCycloMediaOptions_FormClosed(object sender, FormClosedEventArgs e)
    {
      GsExtension.OpenDocumentEvent -= LoginReload;
      _frmCycloMediaOptions = null;
    }

    private void tcSettings_Click(object sender, EventArgs e)
    {
      btnOk.Select();
    }

    private void nudMaxViewers_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void nudMaxViewers_Click(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void nudDistVectLayerViewer_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void nudDistVectLayerViewer_Click(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void ckEnableSmartClick_Click(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void ckDetailImages_Click(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void cbSpatialReferences_Click(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void cbSpatialReferences_SelectedIndexChanged(object sender, EventArgs e)
    {
      var selectedItem = (SpatialReference) cbSpatialReferences.SelectedItem;
      lblMeasuringSupported.Text = (selectedItem == null)
        ? MeasuringNotSupported
        : (selectedItem.CanMeasuring ? MeasuringSupported : MeasuringNotSupported);
    }

    private void rtbAbout_LinkClicked(object sender, LinkClickedEventArgs e)
    {
      Process.Start(e.LinkText);
    }

    private void ckDefaultBaseUrl_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
      txtBaseUrlLocation.Text = ckDefaultBaseUrl.Checked ? string.Empty : _config.BaseUrl;
      txtBaseUrlLocation.Enabled = !ckDefaultBaseUrl.Checked;
    }

    private void ckDefaultRecordingsService_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
      txtRecordingServiceLocation.Text = ckDefaultRecordingService.Checked ? string.Empty : _config.RecordingsService;
      txtRecordingServiceLocation.Enabled = !ckDefaultRecordingService.Checked;
    }

    private void ckDefaultSwfUrl_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
      txtSwfUrlLocation.Text = ckDefaultSwfUrl.Checked ? string.Empty : _config.SwfUrl;
      txtSwfUrlLocation.Enabled = !ckDefaultSwfUrl.Checked;
    }

    private void ckUseProxyServer_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
      FillInProxyParameters();
    }

    private void ckUseDefaultProxyCredentials_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
      FillInProxyParameters();
    }

    private void txtProxyAddress_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtProxyPort_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void ckBypassProxyOnLocal_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtProxyUsername_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtProxyPassword_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtProxyDomain_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtBaseUrlLocation_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtRecordingServiceLocation_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtSwfUrlLocation_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtPort_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
      {
        e.Handled = true;
      }
    }

    #endregion
  }
}
