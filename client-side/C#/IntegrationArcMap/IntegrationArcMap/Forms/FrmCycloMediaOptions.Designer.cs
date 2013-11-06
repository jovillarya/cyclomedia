namespace IntegrationArcMap.Forms
{
  partial class FrmCycloMediaOptions
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.tcSettings = new System.Windows.Forms.TabControl();
      this.tbLogin = new System.Windows.Forms.TabPage();
      this.lblLoginStatus = new System.Windows.Forms.Label();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.txtUsername = new System.Windows.Forms.TextBox();
      this.lblPassword = new System.Windows.Forms.Label();
      this.lblUsername = new System.Windows.Forms.Label();
      this.tbCycloramaViewer = new System.Windows.Forms.TabPage();
      this.ckDetailImages = new System.Windows.Forms.CheckBox();
      this.nudDistVectLayerViewer = new System.Windows.Forms.NumericUpDown();
      this.nudMaxViewers = new System.Windows.Forms.NumericUpDown();
      this.lblDistVectLayerViewer = new System.Windows.Forms.Label();
      this.lblMaxViewers = new System.Windows.Forms.Label();
      this.tbMeasurement = new System.Windows.Forms.TabPage();
      this.ckEnableSmartClick = new System.Windows.Forms.CheckBox();
      this.tbServices = new System.Windows.Forms.TabPage();
      this.txtRecordingService = new System.Windows.Forms.TextBox();
      this.lblRecordingService = new System.Windows.Forms.Label();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.tcSettings.SuspendLayout();
      this.tbLogin.SuspendLayout();
      this.tbCycloramaViewer.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudDistVectLayerViewer)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nudMaxViewers)).BeginInit();
      this.tbMeasurement.SuspendLayout();
      this.tbServices.SuspendLayout();
      this.SuspendLayout();
      // 
      // tcSettings
      // 
      this.tcSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tcSettings.Controls.Add(this.tbLogin);
      this.tcSettings.Controls.Add(this.tbCycloramaViewer);
      this.tcSettings.Controls.Add(this.tbMeasurement);
      this.tcSettings.Controls.Add(this.tbServices);
      this.tcSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tcSettings.Location = new System.Drawing.Point(5, 5);
      this.tcSettings.Multiline = true;
      this.tcSettings.Name = "tcSettings";
      this.tcSettings.SelectedIndex = 0;
      this.tcSettings.Size = new System.Drawing.Size(405, 105);
      this.tcSettings.TabIndex = 0;
      this.tcSettings.Selected += new System.Windows.Forms.TabControlEventHandler(this.tcSettings_Selected);
      this.tcSettings.Click += new System.EventHandler(this.tcSettings_Click);
      // 
      // tbLogin
      // 
      this.tbLogin.Controls.Add(this.lblLoginStatus);
      this.tbLogin.Controls.Add(this.txtPassword);
      this.tbLogin.Controls.Add(this.txtUsername);
      this.tbLogin.Controls.Add(this.lblPassword);
      this.tbLogin.Controls.Add(this.lblUsername);
      this.tbLogin.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tbLogin.Location = new System.Drawing.Point(4, 23);
      this.tbLogin.Name = "tbLogin";
      this.tbLogin.Padding = new System.Windows.Forms.Padding(3);
      this.tbLogin.Size = new System.Drawing.Size(397, 78);
      this.tbLogin.TabIndex = 0;
      this.tbLogin.Text = "Login";
      this.tbLogin.UseVisualStyleBackColor = true;
      // 
      // lblLoginStatus
      // 
      this.lblLoginStatus.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblLoginStatus.Location = new System.Drawing.Point(70, 50);
      this.lblLoginStatus.Name = "lblLoginStatus";
      this.lblLoginStatus.Size = new System.Drawing.Size(150, 22);
      this.lblLoginStatus.TabIndex = 7;
      this.lblLoginStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // txtPassword
      // 
      this.txtPassword.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtPassword.Location = new System.Drawing.Point(70, 25);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(150, 20);
      this.txtPassword.TabIndex = 2;
      this.txtPassword.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtPassword_KeyUp);
      // 
      // txtUsername
      // 
      this.txtUsername.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtUsername.Location = new System.Drawing.Point(70, 5);
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.Size = new System.Drawing.Size(150, 20);
      this.txtUsername.TabIndex = 1;
      this.txtUsername.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TxtUsername_KeyUp);
      // 
      // lblPassword
      // 
      this.lblPassword.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblPassword.Location = new System.Drawing.Point(5, 25);
      this.lblPassword.Name = "lblPassword";
      this.lblPassword.Size = new System.Drawing.Size(60, 22);
      this.lblPassword.TabIndex = 6;
      this.lblPassword.Text = "Password:";
      this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblUsername
      // 
      this.lblUsername.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblUsername.Location = new System.Drawing.Point(5, 5);
      this.lblUsername.Name = "lblUsername";
      this.lblUsername.Size = new System.Drawing.Size(60, 22);
      this.lblUsername.TabIndex = 4;
      this.lblUsername.Text = "Username:";
      this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tbCycloramaViewer
      // 
      this.tbCycloramaViewer.Controls.Add(this.ckDetailImages);
      this.tbCycloramaViewer.Controls.Add(this.nudDistVectLayerViewer);
      this.tbCycloramaViewer.Controls.Add(this.nudMaxViewers);
      this.tbCycloramaViewer.Controls.Add(this.lblDistVectLayerViewer);
      this.tbCycloramaViewer.Controls.Add(this.lblMaxViewers);
      this.tbCycloramaViewer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tbCycloramaViewer.Location = new System.Drawing.Point(4, 23);
      this.tbCycloramaViewer.Name = "tbCycloramaViewer";
      this.tbCycloramaViewer.Padding = new System.Windows.Forms.Padding(3);
      this.tbCycloramaViewer.Size = new System.Drawing.Size(397, 78);
      this.tbCycloramaViewer.TabIndex = 1;
      this.tbCycloramaViewer.Text = "Cyclorama viewer";
      this.tbCycloramaViewer.UseVisualStyleBackColor = true;
      // 
      // ckDetailImages
      // 
      this.ckDetailImages.AutoSize = true;
      this.ckDetailImages.Location = new System.Drawing.Point(205, 5);
      this.ckDetailImages.Name = "ckDetailImages";
      this.ckDetailImages.Size = new System.Drawing.Size(89, 18);
      this.ckDetailImages.TabIndex = 18;
      this.ckDetailImages.Text = "Detail images";
      this.ckDetailImages.UseVisualStyleBackColor = true;
      this.ckDetailImages.Click += new System.EventHandler(this.ckDetailImages_Click);
      // 
      // nudDistVectLayerViewer
      // 
      this.nudDistVectLayerViewer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.nudDistVectLayerViewer.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.nudDistVectLayerViewer.Location = new System.Drawing.Point(140, 25);
      this.nudDistVectLayerViewer.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
      this.nudDistVectLayerViewer.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nudDistVectLayerViewer.Name = "nudDistVectLayerViewer";
      this.nudDistVectLayerViewer.Size = new System.Drawing.Size(50, 20);
      this.nudDistVectLayerViewer.TabIndex = 17;
      this.nudDistVectLayerViewer.Value = new decimal(new int[] {
            30,
            0,
            0,
            0});
      this.nudDistVectLayerViewer.Click += new System.EventHandler(this.nudDistVectLayerViewer_Click);
      this.nudDistVectLayerViewer.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudDistVectLayerViewer_KeyUp);
      // 
      // nudMaxViewers
      // 
      this.nudMaxViewers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.nudMaxViewers.Location = new System.Drawing.Point(140, 5);
      this.nudMaxViewers.Maximum = new decimal(new int[] {
            6,
            0,
            0,
            0});
      this.nudMaxViewers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.nudMaxViewers.Name = "nudMaxViewers";
      this.nudMaxViewers.Size = new System.Drawing.Size(50, 20);
      this.nudMaxViewers.TabIndex = 16;
      this.nudMaxViewers.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.nudMaxViewers.Click += new System.EventHandler(this.nudMaxViewers_Click);
      this.nudMaxViewers.KeyUp += new System.Windows.Forms.KeyEventHandler(this.nudMaxViewers_KeyUp);
      // 
      // lblDistVectLayerViewer
      // 
      this.lblDistVectLayerViewer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDistVectLayerViewer.Location = new System.Drawing.Point(5, 25);
      this.lblDistVectLayerViewer.Name = "lblDistVectLayerViewer";
      this.lblDistVectLayerViewer.Size = new System.Drawing.Size(130, 22);
      this.lblDistVectLayerViewer.TabIndex = 15;
      this.lblDistVectLayerViewer.Text = "Dist. vect. layer - viewer:";
      this.lblDistVectLayerViewer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblMaxViewers
      // 
      this.lblMaxViewers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMaxViewers.Location = new System.Drawing.Point(5, 5);
      this.lblMaxViewers.Name = "lblMaxViewers";
      this.lblMaxViewers.Size = new System.Drawing.Size(75, 22);
      this.lblMaxViewers.TabIndex = 13;
      this.lblMaxViewers.Text = "Max viewers:";
      this.lblMaxViewers.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // tbMeasurement
      // 
      this.tbMeasurement.Controls.Add(this.ckEnableSmartClick);
      this.tbMeasurement.Location = new System.Drawing.Point(4, 23);
      this.tbMeasurement.Name = "tbMeasurement";
      this.tbMeasurement.Padding = new System.Windows.Forms.Padding(3);
      this.tbMeasurement.Size = new System.Drawing.Size(397, 78);
      this.tbMeasurement.TabIndex = 2;
      this.tbMeasurement.Text = "Measurement";
      this.tbMeasurement.UseVisualStyleBackColor = true;
      // 
      // ckEnableSmartClick
      // 
      this.ckEnableSmartClick.AutoSize = true;
      this.ckEnableSmartClick.Location = new System.Drawing.Point(5, 5);
      this.ckEnableSmartClick.Name = "ckEnableSmartClick";
      this.ckEnableSmartClick.Size = new System.Drawing.Size(188, 18);
      this.ckEnableSmartClick.TabIndex = 1;
      this.ckEnableSmartClick.Text = "Enable Smart Click measurements";
      this.ckEnableSmartClick.UseVisualStyleBackColor = true;
      this.ckEnableSmartClick.Click += new System.EventHandler(this.ckEnableSmartClick_Click);
      // 
      // tbServices
      // 
      this.tbServices.Controls.Add(this.txtRecordingService);
      this.tbServices.Controls.Add(this.lblRecordingService);
      this.tbServices.Location = new System.Drawing.Point(4, 23);
      this.tbServices.Name = "tbServices";
      this.tbServices.Size = new System.Drawing.Size(397, 78);
      this.tbServices.TabIndex = 3;
      this.tbServices.Text = "Services";
      this.tbServices.UseVisualStyleBackColor = true;
      // 
      // txtRecordingService
      // 
      this.txtRecordingService.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtRecordingService.Location = new System.Drawing.Point(110, 5);
      this.txtRecordingService.Name = "txtRecordingService";
      this.txtRecordingService.Size = new System.Drawing.Size(280, 20);
      this.txtRecordingService.TabIndex = 1;
      this.txtRecordingService.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtRecordingService_KeyUp);
      // 
      // lblRecordingService
      // 
      this.lblRecordingService.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblRecordingService.Location = new System.Drawing.Point(5, 5);
      this.lblRecordingService.Name = "lblRecordingService";
      this.lblRecordingService.Size = new System.Drawing.Size(100, 22);
      this.lblRecordingService.TabIndex = 18;
      this.lblRecordingService.Text = "Recording service:";
      this.lblRecordingService.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // btnOk
      // 
      this.btnOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnOk.Location = new System.Drawing.Point(169, 115);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 25);
      this.btnOk.TabIndex = 10;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(249, 115);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 25);
      this.btnCancel.TabIndex = 11;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnApply
      // 
      this.btnApply.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnApply.Location = new System.Drawing.Point(329, 115);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(75, 25);
      this.btnApply.TabIndex = 12;
      this.btnApply.Text = "Apply";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // FrmCycloMediaOptions
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(414, 147);
      this.Controls.Add(this.btnApply);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.tcSettings);
      this.Controls.Add(this.btnOk);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmCycloMediaOptions";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "CycloMedia Options";
      this.TopMost = true;
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCycloMediaOptions_FormClosed);
      this.Load += new System.EventHandler(this.FrmCycloMediaOptions_Load);
      this.tcSettings.ResumeLayout(false);
      this.tbLogin.ResumeLayout(false);
      this.tbLogin.PerformLayout();
      this.tbCycloramaViewer.ResumeLayout(false);
      this.tbCycloramaViewer.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudDistVectLayerViewer)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nudMaxViewers)).EndInit();
      this.tbMeasurement.ResumeLayout(false);
      this.tbMeasurement.PerformLayout();
      this.tbServices.ResumeLayout(false);
      this.tbServices.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tcSettings;
    private System.Windows.Forms.TabPage tbLogin;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TabPage tbCycloramaViewer;
    private System.Windows.Forms.Label lblDistVectLayerViewer;
    private System.Windows.Forms.Label lblMaxViewers;
    private System.Windows.Forms.TabPage tbMeasurement;
    private System.Windows.Forms.CheckBox ckEnableSmartClick;
    private System.Windows.Forms.TabPage tbServices;
    private System.Windows.Forms.TextBox txtRecordingService;
    private System.Windows.Forms.Label lblRecordingService;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnApply;
    private System.Windows.Forms.Label lblLoginStatus;
    private System.Windows.Forms.NumericUpDown nudMaxViewers;
    private System.Windows.Forms.NumericUpDown nudDistVectLayerViewer;
    private System.Windows.Forms.CheckBox ckDetailImages;
  }
}