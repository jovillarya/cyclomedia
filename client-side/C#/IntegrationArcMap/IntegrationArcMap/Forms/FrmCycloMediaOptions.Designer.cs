﻿namespace IntegrationArcMap.Forms
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
      this.tbSettings = new System.Windows.Forms.TabPage();
      this.grGeneral = new System.Windows.Forms.GroupBox();
      this.ckEnableSmartClick = new System.Windows.Forms.CheckBox();
      this.lblMaximumCycloramaViewers = new System.Windows.Forms.Label();
      this.nudMaxViewers = new System.Windows.Forms.NumericUpDown();
      this.ckDetailImages = new System.Windows.Forms.CheckBox();
      this.lblDistVectLayerViewer = new System.Windows.Forms.Label();
      this.nudDistVectLayerViewer = new System.Windows.Forms.NumericUpDown();
      this.grCoordinateSystems = new System.Windows.Forms.GroupBox();
      this.lblMeasuringSupported = new System.Windows.Forms.Label();
      this.lblCoordinateSystemCycloramaViewer = new System.Windows.Forms.Label();
      this.cbSpatialReferences = new System.Windows.Forms.ComboBox();
      this.tbConfiguration = new System.Windows.Forms.TabPage();
      this.grBaseUrl = new System.Windows.Forms.GroupBox();
      this.lblLocationBaseUrl = new System.Windows.Forms.Label();
      this.ckDefaultBaseUrl = new System.Windows.Forms.CheckBox();
      this.txtBaseUrlLocation = new System.Windows.Forms.TextBox();
      this.grSwfUrl = new System.Windows.Forms.GroupBox();
      this.lblLocationSwfUrl = new System.Windows.Forms.Label();
      this.ckDefaultSwfUrl = new System.Windows.Forms.CheckBox();
      this.txtSwfUrlLocation = new System.Windows.Forms.TextBox();
      this.tbAbout = new System.Windows.Forms.TabPage();
      this.rtbAbout = new System.Windows.Forms.RichTextBox();
      this.tbAgreement = new System.Windows.Forms.TabPage();
      this.txtAgreement = new System.Windows.Forms.TextBox();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.plSettings = new System.Windows.Forms.Panel();
      this.plButtons = new System.Windows.Forms.Panel();
      this.tcSettings.SuspendLayout();
      this.tbLogin.SuspendLayout();
      this.tbSettings.SuspendLayout();
      this.grGeneral.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudMaxViewers)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.nudDistVectLayerViewer)).BeginInit();
      this.grCoordinateSystems.SuspendLayout();
      this.tbConfiguration.SuspendLayout();
      this.grBaseUrl.SuspendLayout();
      this.grSwfUrl.SuspendLayout();
      this.tbAbout.SuspendLayout();
      this.tbAgreement.SuspendLayout();
      this.plSettings.SuspendLayout();
      this.plButtons.SuspendLayout();
      this.SuspendLayout();
      // 
      // tcSettings
      // 
      this.tcSettings.Controls.Add(this.tbLogin);
      this.tcSettings.Controls.Add(this.tbSettings);
      this.tcSettings.Controls.Add(this.tbConfiguration);
      this.tcSettings.Controls.Add(this.tbAbout);
      this.tcSettings.Controls.Add(this.tbAgreement);
      this.tcSettings.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tcSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tcSettings.Location = new System.Drawing.Point(0, 0);
      this.tcSettings.Multiline = true;
      this.tcSettings.Name = "tcSettings";
      this.tcSettings.SelectedIndex = 0;
      this.tcSettings.Size = new System.Drawing.Size(420, 405);
      this.tcSettings.TabIndex = 0;
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
      this.tbLogin.Size = new System.Drawing.Size(412, 378);
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
      this.lblLoginStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
      this.lblPassword.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lblUsername
      // 
      this.lblUsername.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblUsername.Location = new System.Drawing.Point(5, 5);
      this.lblUsername.Name = "lblUsername";
      this.lblUsername.Size = new System.Drawing.Size(60, 22);
      this.lblUsername.TabIndex = 4;
      this.lblUsername.Text = "Username:";
      this.lblUsername.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // tbSettings
      // 
      this.tbSettings.Controls.Add(this.grGeneral);
      this.tbSettings.Controls.Add(this.grCoordinateSystems);
      this.tbSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tbSettings.Location = new System.Drawing.Point(4, 23);
      this.tbSettings.Name = "tbSettings";
      this.tbSettings.Padding = new System.Windows.Forms.Padding(3);
      this.tbSettings.Size = new System.Drawing.Size(412, 378);
      this.tbSettings.TabIndex = 1;
      this.tbSettings.Text = "Settings";
      this.tbSettings.UseVisualStyleBackColor = true;
      // 
      // grGeneral
      // 
      this.grGeneral.Controls.Add(this.ckEnableSmartClick);
      this.grGeneral.Controls.Add(this.lblMaximumCycloramaViewers);
      this.grGeneral.Controls.Add(this.nudMaxViewers);
      this.grGeneral.Controls.Add(this.ckDetailImages);
      this.grGeneral.Controls.Add(this.lblDistVectLayerViewer);
      this.grGeneral.Controls.Add(this.nudDistVectLayerViewer);
      this.grGeneral.Location = new System.Drawing.Point(3, 73);
      this.grGeneral.Name = "grGeneral";
      this.grGeneral.Size = new System.Drawing.Size(406, 115);
      this.grGeneral.TabIndex = 22;
      this.grGeneral.TabStop = false;
      this.grGeneral.Text = "General";
      // 
      // ckEnableSmartClick
      // 
      this.ckEnableSmartClick.AutoSize = true;
      this.ckEnableSmartClick.Location = new System.Drawing.Point(3, 90);
      this.ckEnableSmartClick.Name = "ckEnableSmartClick";
      this.ckEnableSmartClick.Size = new System.Drawing.Size(188, 18);
      this.ckEnableSmartClick.TabIndex = 19;
      this.ckEnableSmartClick.Text = "Enable Smart Click measurements";
      this.ckEnableSmartClick.UseVisualStyleBackColor = true;
      this.ckEnableSmartClick.Click += new System.EventHandler(this.ckEnableSmartClick_Click);
      // 
      // lblMaximumCycloramaViewers
      // 
      this.lblMaximumCycloramaViewers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMaximumCycloramaViewers.Location = new System.Drawing.Point(3, 15);
      this.lblMaximumCycloramaViewers.Name = "lblMaximumCycloramaViewers";
      this.lblMaximumCycloramaViewers.Size = new System.Drawing.Size(157, 20);
      this.lblMaximumCycloramaViewers.TabIndex = 13;
      this.lblMaximumCycloramaViewers.Text = "Maximum Cyclorama viewers:";
      this.lblMaximumCycloramaViewers.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // nudMaxViewers
      // 
      this.nudMaxViewers.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.nudMaxViewers.Location = new System.Drawing.Point(160, 15);
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
      // ckDetailImages
      // 
      this.ckDetailImages.AutoSize = true;
      this.ckDetailImages.Location = new System.Drawing.Point(3, 65);
      this.ckDetailImages.Name = "ckDetailImages";
      this.ckDetailImages.Size = new System.Drawing.Size(120, 18);
      this.ckDetailImages.TabIndex = 18;
      this.ckDetailImages.Text = "Show detail images";
      this.ckDetailImages.UseVisualStyleBackColor = true;
      this.ckDetailImages.Click += new System.EventHandler(this.ckDetailImages_Click);
      // 
      // lblDistVectLayerViewer
      // 
      this.lblDistVectLayerViewer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDistVectLayerViewer.Location = new System.Drawing.Point(3, 40);
      this.lblDistVectLayerViewer.Name = "lblDistVectLayerViewer";
      this.lblDistVectLayerViewer.Size = new System.Drawing.Size(157, 20);
      this.lblDistVectLayerViewer.TabIndex = 15;
      this.lblDistVectLayerViewer.Text = "Overlay draw distance[m]:";
      this.lblDistVectLayerViewer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // nudDistVectLayerViewer
      // 
      this.nudDistVectLayerViewer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.nudDistVectLayerViewer.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.nudDistVectLayerViewer.Location = new System.Drawing.Point(160, 40);
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
      // grCoordinateSystems
      // 
      this.grCoordinateSystems.Controls.Add(this.lblMeasuringSupported);
      this.grCoordinateSystems.Controls.Add(this.lblCoordinateSystemCycloramaViewer);
      this.grCoordinateSystems.Controls.Add(this.cbSpatialReferences);
      this.grCoordinateSystems.Location = new System.Drawing.Point(3, 3);
      this.grCoordinateSystems.Name = "grCoordinateSystems";
      this.grCoordinateSystems.Size = new System.Drawing.Size(406, 70);
      this.grCoordinateSystems.TabIndex = 21;
      this.grCoordinateSystems.TabStop = false;
      this.grCoordinateSystems.Text = "Coordinate systems";
      // 
      // lblMeasuringSupported
      // 
      this.lblMeasuringSupported.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMeasuringSupported.Location = new System.Drawing.Point(160, 40);
      this.lblMeasuringSupported.Name = "lblMeasuringSupported";
      this.lblMeasuringSupported.Size = new System.Drawing.Size(242, 22);
      this.lblMeasuringSupported.TabIndex = 21;
      this.lblMeasuringSupported.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // lblCoordinateSystemCycloramaViewer
      // 
      this.lblCoordinateSystemCycloramaViewer.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblCoordinateSystemCycloramaViewer.Location = new System.Drawing.Point(3, 15);
      this.lblCoordinateSystemCycloramaViewer.Name = "lblCoordinateSystemCycloramaViewer";
      this.lblCoordinateSystemCycloramaViewer.Size = new System.Drawing.Size(157, 22);
      this.lblCoordinateSystemCycloramaViewer.TabIndex = 19;
      this.lblCoordinateSystemCycloramaViewer.Text = "Cyclorama viewer:";
      this.lblCoordinateSystemCycloramaViewer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // cbSpatialReferences
      // 
      this.cbSpatialReferences.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cbSpatialReferences.FormattingEnabled = true;
      this.cbSpatialReferences.Location = new System.Drawing.Point(160, 15);
      this.cbSpatialReferences.Name = "cbSpatialReferences";
      this.cbSpatialReferences.Size = new System.Drawing.Size(242, 22);
      this.cbSpatialReferences.Sorted = true;
      this.cbSpatialReferences.TabIndex = 20;
      this.cbSpatialReferences.SelectedIndexChanged += new System.EventHandler(this.cbSpatialReferences_SelectedIndexChanged);
      this.cbSpatialReferences.Click += new System.EventHandler(this.cbSpatialReferences_Click);
      // 
      // tbConfiguration
      // 
      this.tbConfiguration.Controls.Add(this.grBaseUrl);
      this.tbConfiguration.Controls.Add(this.grSwfUrl);
      this.tbConfiguration.Location = new System.Drawing.Point(4, 23);
      this.tbConfiguration.Name = "tbConfiguration";
      this.tbConfiguration.Size = new System.Drawing.Size(412, 378);
      this.tbConfiguration.TabIndex = 3;
      this.tbConfiguration.Text = "Configuration";
      this.tbConfiguration.UseVisualStyleBackColor = true;
      // 
      // grBaseUrl
      // 
      this.grBaseUrl.Controls.Add(this.lblLocationBaseUrl);
      this.grBaseUrl.Controls.Add(this.ckDefaultBaseUrl);
      this.grBaseUrl.Controls.Add(this.txtBaseUrlLocation);
      this.grBaseUrl.Location = new System.Drawing.Point(3, 3);
      this.grBaseUrl.Name = "grBaseUrl";
      this.grBaseUrl.Size = new System.Drawing.Size(406, 65);
      this.grBaseUrl.TabIndex = 23;
      this.grBaseUrl.TabStop = false;
      this.grBaseUrl.Text = "Base url";
      // 
      // lblLocationBaseUrl
      // 
      this.lblLocationBaseUrl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblLocationBaseUrl.Location = new System.Drawing.Point(3, 37);
      this.lblLocationBaseUrl.Name = "lblLocationBaseUrl";
      this.lblLocationBaseUrl.Size = new System.Drawing.Size(79, 20);
      this.lblLocationBaseUrl.TabIndex = 22;
      this.lblLocationBaseUrl.Text = "Location:";
      this.lblLocationBaseUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // ckDefaultBaseUrl
      // 
      this.ckDefaultBaseUrl.AutoSize = true;
      this.ckDefaultBaseUrl.Location = new System.Drawing.Point(3, 15);
      this.ckDefaultBaseUrl.Name = "ckDefaultBaseUrl";
      this.ckDefaultBaseUrl.Size = new System.Drawing.Size(81, 18);
      this.ckDefaultBaseUrl.TabIndex = 20;
      this.ckDefaultBaseUrl.Text = "Use default";
      this.ckDefaultBaseUrl.UseVisualStyleBackColor = true;
      this.ckDefaultBaseUrl.CheckedChanged += new System.EventHandler(this.ckDefaultBaseUrl_CheckedChanged);
      // 
      // txtBaseUrlLocation
      // 
      this.txtBaseUrlLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtBaseUrlLocation.Location = new System.Drawing.Point(82, 37);
      this.txtBaseUrlLocation.Name = "txtBaseUrlLocation";
      this.txtBaseUrlLocation.Size = new System.Drawing.Size(320, 20);
      this.txtBaseUrlLocation.TabIndex = 21;
      this.txtBaseUrlLocation.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtBaseUrlLocation_KeyUp);
      // 
      // grSwfUrl
      // 
      this.grSwfUrl.Controls.Add(this.lblLocationSwfUrl);
      this.grSwfUrl.Controls.Add(this.ckDefaultSwfUrl);
      this.grSwfUrl.Controls.Add(this.txtSwfUrlLocation);
      this.grSwfUrl.Location = new System.Drawing.Point(3, 68);
      this.grSwfUrl.Name = "grSwfUrl";
      this.grSwfUrl.Size = new System.Drawing.Size(406, 65);
      this.grSwfUrl.TabIndex = 22;
      this.grSwfUrl.TabStop = false;
      this.grSwfUrl.Text = "swf";
      // 
      // lblLocationSwfUrl
      // 
      this.lblLocationSwfUrl.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblLocationSwfUrl.Location = new System.Drawing.Point(3, 37);
      this.lblLocationSwfUrl.Name = "lblLocationSwfUrl";
      this.lblLocationSwfUrl.Size = new System.Drawing.Size(79, 20);
      this.lblLocationSwfUrl.TabIndex = 22;
      this.lblLocationSwfUrl.Text = "Location:";
      this.lblLocationSwfUrl.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // ckDefaultSwfUrl
      // 
      this.ckDefaultSwfUrl.AutoSize = true;
      this.ckDefaultSwfUrl.Location = new System.Drawing.Point(3, 15);
      this.ckDefaultSwfUrl.Name = "ckDefaultSwfUrl";
      this.ckDefaultSwfUrl.Size = new System.Drawing.Size(81, 18);
      this.ckDefaultSwfUrl.TabIndex = 20;
      this.ckDefaultSwfUrl.Text = "Use default";
      this.ckDefaultSwfUrl.UseVisualStyleBackColor = true;
      this.ckDefaultSwfUrl.CheckedChanged += new System.EventHandler(this.ckDefaultSwfUrl_CheckedChanged);
      // 
      // txtSwfUrlLocation
      // 
      this.txtSwfUrlLocation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtSwfUrlLocation.Location = new System.Drawing.Point(82, 37);
      this.txtSwfUrlLocation.Name = "txtSwfUrlLocation";
      this.txtSwfUrlLocation.Size = new System.Drawing.Size(320, 20);
      this.txtSwfUrlLocation.TabIndex = 21;
      this.txtSwfUrlLocation.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSwfUrlLocation_KeyUp);
      // 
      // tbAbout
      // 
      this.tbAbout.Controls.Add(this.rtbAbout);
      this.tbAbout.Location = new System.Drawing.Point(4, 23);
      this.tbAbout.Name = "tbAbout";
      this.tbAbout.Size = new System.Drawing.Size(412, 378);
      this.tbAbout.TabIndex = 4;
      this.tbAbout.Text = "About";
      this.tbAbout.UseVisualStyleBackColor = true;
      // 
      // rtbAbout
      // 
      this.rtbAbout.BackColor = System.Drawing.SystemColors.Window;
      this.rtbAbout.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.rtbAbout.Location = new System.Drawing.Point(5, 5);
      this.rtbAbout.Name = "rtbAbout";
      this.rtbAbout.ReadOnly = true;
      this.rtbAbout.Size = new System.Drawing.Size(402, 68);
      this.rtbAbout.TabIndex = 20;
      this.rtbAbout.Text = "";
      this.rtbAbout.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbAbout_LinkClicked);
      // 
      // tbAgreement
      // 
      this.tbAgreement.Controls.Add(this.txtAgreement);
      this.tbAgreement.Location = new System.Drawing.Point(4, 23);
      this.tbAgreement.Name = "tbAgreement";
      this.tbAgreement.Size = new System.Drawing.Size(412, 378);
      this.tbAgreement.TabIndex = 5;
      this.tbAgreement.Text = "Agreement";
      this.tbAgreement.UseVisualStyleBackColor = true;
      // 
      // txtAgreement
      // 
      this.txtAgreement.BackColor = System.Drawing.SystemColors.Window;
      this.txtAgreement.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtAgreement.Location = new System.Drawing.Point(5, 5);
      this.txtAgreement.Multiline = true;
      this.txtAgreement.Name = "txtAgreement";
      this.txtAgreement.ReadOnly = true;
      this.txtAgreement.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtAgreement.Size = new System.Drawing.Size(402, 368);
      this.txtAgreement.TabIndex = 0;
      // 
      // btnOk
      // 
      this.btnOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnOk.Location = new System.Drawing.Point(180, 5);
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
      this.btnCancel.Location = new System.Drawing.Point(260, 5);
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
      this.btnApply.Location = new System.Drawing.Point(340, 5);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(75, 25);
      this.btnApply.TabIndex = 12;
      this.btnApply.Text = "Apply";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // plSettings
      // 
      this.plSettings.Controls.Add(this.tcSettings);
      this.plSettings.Dock = System.Windows.Forms.DockStyle.Top;
      this.plSettings.Location = new System.Drawing.Point(0, 0);
      this.plSettings.Name = "plSettings";
      this.plSettings.Size = new System.Drawing.Size(420, 405);
      this.plSettings.TabIndex = 13;
      // 
      // plButtons
      // 
      this.plButtons.Controls.Add(this.btnOk);
      this.plButtons.Controls.Add(this.btnCancel);
      this.plButtons.Controls.Add(this.btnApply);
      this.plButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.plButtons.Location = new System.Drawing.Point(0, 405);
      this.plButtons.Name = "plButtons";
      this.plButtons.Size = new System.Drawing.Size(420, 35);
      this.plButtons.TabIndex = 14;
      // 
      // FrmCycloMediaOptions
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(420, 440);
      this.Controls.Add(this.plButtons);
      this.Controls.Add(this.plSettings);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmCycloMediaOptions";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "GlobeSpotter for ArcGIS Desktop";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCycloMediaOptions_FormClosed);
      this.Load += new System.EventHandler(this.FrmCycloMediaOptions_Load);
      this.tcSettings.ResumeLayout(false);
      this.tbLogin.ResumeLayout(false);
      this.tbLogin.PerformLayout();
      this.tbSettings.ResumeLayout(false);
      this.grGeneral.ResumeLayout(false);
      this.grGeneral.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.nudMaxViewers)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.nudDistVectLayerViewer)).EndInit();
      this.grCoordinateSystems.ResumeLayout(false);
      this.tbConfiguration.ResumeLayout(false);
      this.grBaseUrl.ResumeLayout(false);
      this.grBaseUrl.PerformLayout();
      this.grSwfUrl.ResumeLayout(false);
      this.grSwfUrl.PerformLayout();
      this.tbAbout.ResumeLayout(false);
      this.tbAgreement.ResumeLayout(false);
      this.tbAgreement.PerformLayout();
      this.plSettings.ResumeLayout(false);
      this.plButtons.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TabControl tcSettings;
    private System.Windows.Forms.TabPage tbLogin;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label lblPassword;
    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TabPage tbSettings;
    private System.Windows.Forms.Label lblDistVectLayerViewer;
    private System.Windows.Forms.Label lblMaximumCycloramaViewers;
    private System.Windows.Forms.TabPage tbConfiguration;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnApply;
    private System.Windows.Forms.Label lblLoginStatus;
    private System.Windows.Forms.NumericUpDown nudMaxViewers;
    private System.Windows.Forms.NumericUpDown nudDistVectLayerViewer;
    private System.Windows.Forms.CheckBox ckDetailImages;
    private System.Windows.Forms.Label lblCoordinateSystemCycloramaViewer;
    private System.Windows.Forms.ComboBox cbSpatialReferences;
    private System.Windows.Forms.TabPage tbAbout;
    private System.Windows.Forms.TabPage tbAgreement;
    private System.Windows.Forms.TextBox txtAgreement;
    private System.Windows.Forms.Panel plSettings;
    private System.Windows.Forms.Panel plButtons;
    private System.Windows.Forms.RichTextBox rtbAbout;
    private System.Windows.Forms.CheckBox ckDefaultSwfUrl;
    private System.Windows.Forms.TextBox txtSwfUrlLocation;
    private System.Windows.Forms.GroupBox grSwfUrl;
    private System.Windows.Forms.Label lblLocationSwfUrl;
    private System.Windows.Forms.GroupBox grBaseUrl;
    private System.Windows.Forms.Label lblLocationBaseUrl;
    private System.Windows.Forms.CheckBox ckDefaultBaseUrl;
    private System.Windows.Forms.TextBox txtBaseUrlLocation;
    private System.Windows.Forms.GroupBox grCoordinateSystems;
    private System.Windows.Forms.GroupBox grGeneral;
    private System.Windows.Forms.CheckBox ckEnableSmartClick;
    private System.Windows.Forms.Label lblMeasuringSupported;
  }
}