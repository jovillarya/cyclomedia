using System;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using IntegrationArcMap.AddIns;
using IntegrationArcMap.Utilities;
using IntegrationArcMap.WebClient;

namespace IntegrationArcMap.Forms
{
  public partial class FrmCycloMediaOptions : Form
  {
    // =========================================================================
    // Login failed / successfully text
    // =========================================================================
    private const string LoginFailed = "Login Failed";
    private const string LoginSuccessfully = "Login Successfully";

    // =========================================================================
    // Members
    // =========================================================================
    private static FrmCycloMediaOptions _frmCycloMediaOptions;
    private static ClientLogin _login;

    private ClientConfig _config;
    private bool _mssgBoxShow;

    // =========================================================================
    // Constructor
    // =========================================================================
    public FrmCycloMediaOptions()
    {
      InitializeComponent();
      _mssgBoxShow = false;
      _config = ClientConfig.Instance;
      GsExtension.LoginReloadEvent += LoginReload;
      Font font = SystemFonts.MenuFont;
      tcSettings.Font = (Font) font.Clone();
      txtPassword.Font = (Font) font.Clone();
      txtUsername.Font = (Font) font.Clone();
      nudMaxViewers.Font = (Font) font.Clone();
      nudDistVectLayerViewer.Font = (Font) font.Clone();
      txtRecordingService.Font = (Font) font.Clone();
      btnApply.Enabled = false;
      btnOk.Select();
    }

    static FrmCycloMediaOptions()
    {
      _login = ClientLogin.Instance;
    }

    // =========================================================================
    // Properties
    // =========================================================================
    public static bool IsVisible
    {
      get { return _frmCycloMediaOptions != null; }
    }

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
      txtRecordingService.Text = _config.RecordingsService;
      nudMaxViewers.Value = _config.MaxViewers;
      nudDistVectLayerViewer.Value = _config.DistanceCycloramaVectorLayer;
      txtPassword.Text = _login.Password;
      txtUsername.Text = _login.Username;
      ckEnableSmartClick.Checked = _config.SmartClickEnabled;

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
      _login = ClientLogin.Load();
      _config = ClientConfig.Load();
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
          if (responce.StatusCode == HttpStatusCode.Unauthorized)
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
      bool loginSucces = ((txtUsername.Text != _login.Username) || (txtPassword.Text != _login.Password)) && Login();

      if (_login.Credentials || loginSucces)
      {
        var maxViewers = (uint) nudMaxViewers.Value;
        var distLayer = (uint) nudDistVectLayerViewer.Value;
        bool restart = (_config.RecordingsService != txtRecordingService.Text);
        _config.MaxViewers = maxViewers;
        _config.DistanceCycloramaVectorLayer = distLayer;
        _config.RecordingsService = txtRecordingService.Text;
        _config.SmartClickEnabled = ckEnableSmartClick.Checked;
        _config.Save();
        FrmGlobespotter.UpdateParameters();

        if (restart)
        {
          FrmGlobespotter.Restart();
        }

        if (close)
        {
          Close();
        }
      }

      btnApply.Enabled = false;
    }

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

    private void tcSettings_Selected(object sender, TabControlEventArgs e)
    {
      if (!_login.Credentials)
      {
        tcSettings.SelectedTab = tbLogin;
      }
    }

    private void FrmCycloMediaOptions_FormClosed(object sender, FormClosedEventArgs e)
    {
      GsExtension.LoginReloadEvent -= LoginReload;
      _frmCycloMediaOptions = null;
    }

    private void txtRecordingService_KeyUp(object sender, KeyEventArgs e)
    {
      btnApply.Enabled = true;
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
  }
}
