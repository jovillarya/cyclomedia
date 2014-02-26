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
using System.IO;
using System.Net;
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
      txtSwfUrlLocation.Font = (Font) font.Clone();
      grRemoteLocal.Font = (Font) font.Clone();
      grBaseUrl.Font = (Font) font.Clone();
      grSwfUrl.Font = (Font) font.Clone();
      cbSpatialReferences.Font = (Font) font.Clone();
      txtAgreement.Font = (Font) font.Clone();

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
      bool credentials;

      try
      {
        credentials = _login.Check();
      }
      catch
      {
        credentials = false;
      }

      if (!credentials)
      {
        OpenForm();
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
      ckDefaultSwfUrl.Checked = _config.SwfUrlDefault;
      txtBaseUrlLocation.Text = _config.BaseUrlDefault ? string.Empty : _config.BaseUrl;
      txtSwfUrlLocation.Text = _config.SwfUrlDefault ? string.Empty : _config.SwfUrl;
      txtBaseUrlLocation.Enabled = !_config.BaseUrlDefault;
      txtSwfUrlLocation.Enabled = !_config.SwfUrlDefault;
      rbLocal.Checked = _config.SwfLocal;
      rbRemote.Checked = !_config.SwfLocal;
      btnApply.Enabled = apply;

      nudMaxViewers.Value = _config.MaxViewers;
      nudDistVectLayerViewer.Value = _config.DistanceCycloramaVectorLayer;
      txtPassword.Text = _login.Password;
      txtUsername.Text = _login.Username;
      ckEnableSmartClick.Checked = _config.SmartClickEnabled;
      ckDetailImages.Checked = _config.DetailImagesEnabled;
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

    private void ExceptionOccured(Exception ex)
    {
      MessageBox.Show(string.Format("Exception occurred: {0}.", ex.Message));
      _mssgBoxShow = true;
    }

    private void FrmSettingsLoad()
    {
      try
      {
        lblLoginStatus.Text = _login.Check() ? LoginSuccessfully : LoginFailed;
      }
      catch
      {
        lblLoginStatus.Text = LoginFailed;
      }

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

      try
      {
        if (_login.Check())
        {
          FrmGlobespotter.LoginSuccesfull();
          result = true;
        }
      }
      catch (WebException ex)
      {
        var responce = ex.Response as HttpWebResponse;

        if (responce != null)
        {
          if ((responce.StatusCode == HttpStatusCode.Unauthorized) || (responce.StatusCode == HttpStatusCode.Forbidden))
          {
            txtUsername.Focus();
          }
          else
          {
            ExceptionOccured(ex);
          }
        }
        else
        {
          ExceptionOccured(ex);
        }
      }
      catch (Exception ex)
      {
        ExceptionOccured(ex);
      }

      lblLoginStatus.Text = _login.Credentials ? LoginSuccessfully : LoginFailed;
      return result;
    }

    private void Save(bool close)
    {
      bool usernameChanged = (txtUsername.Text != _login.Username) || (txtPassword.Text != _login.Password);
      bool loginSucces = usernameChanged && Login();

      if (_login.Credentials || loginSucces)
      {
        SpatialReference spat = _config.SpatialReference;
        var maxViewers = (uint) nudMaxViewers.Value;
        var distLayer = (uint) nudDistVectLayerViewer.Value;
        bool restart = (_config.BaseUrlDefault != ckDefaultBaseUrl.Checked) || (_config.BaseUrl != txtBaseUrlLocation.Text);
        restart = restart || (_config.SwfUrlDefault != ckDefaultSwfUrl.Checked) || (_config.SwfUrl != txtSwfUrlLocation.Text);
        restart = restart || (_config.SwfLocal != rbLocal.Checked);
        var selectedItem = (SpatialReference) cbSpatialReferences.SelectedItem;
        restart = restart ||
                  ((spat == null) || ((selectedItem != null) && (spat.ToString() != selectedItem.ToString())));
        restart = restart || usernameChanged;
        _config.SpatialReference = selectedItem ?? _config.SpatialReference;
        _config.MaxViewers = maxViewers;
        _config.DistanceCycloramaVectorLayer = distLayer;
        _config.BaseUrl = txtBaseUrlLocation.Text;
        _config.SwfLocal = rbLocal.Checked;
        _config.SwfUrl = txtSwfUrlLocation.Text;
        _config.BaseUrlDefault = ckDefaultBaseUrl.Checked;
        _config.SwfUrlDefault = ckDefaultSwfUrl.Checked;
        _config.SmartClickEnabled = ckEnableSmartClick.Checked;
        _config.DetailImagesEnabled = ckDetailImages.Checked;
        _config.Save();

        if (restart && FrmGlobespotter.IsStarted())
        {
          FrmGlobespotter.Restart();
        }
        else
        {
          FrmGlobespotter.UpdateParameters();
        }

        if (close)
        {
          Close();
        }
      }

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

    private void ckDefaultSwfUrl_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
      txtSwfUrlLocation.Text = ckDefaultSwfUrl.Checked ? string.Empty : _config.SwfUrl;
      txtSwfUrlLocation.Enabled = !ckDefaultSwfUrl.Checked;
    }

    private void txtBaseUrlLocation_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void txtSwfUrlLocation_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
    }

    private void rbLocal_CheckedChanged(object sender, EventArgs e)
    {
      btnApply.Enabled = true;
    }

    #endregion
  }
}
