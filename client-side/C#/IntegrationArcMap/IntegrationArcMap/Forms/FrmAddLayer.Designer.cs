namespace IntegrationArcMap.Forms
{
  partial class FrmAddLayer
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.plSelectFormat = new System.Windows.Forms.Panel();
      this.rbSLD = new System.Windows.Forms.RadioButton();
      this.rbWFS = new System.Windows.Forms.RadioButton();
      this.lblSelectFormat = new System.Windows.Forms.Label();
      this.plButtons = new System.Windows.Forms.Panel();
      this.btnFinish = new System.Windows.Forms.Button();
      this.btnNext = new System.Windows.Forms.Button();
      this.btnBack = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.plSelectWfsLayer = new System.Windows.Forms.Panel();
      this.txtStatus = new System.Windows.Forms.TextBox();
      this.lblStatus = new System.Windows.Forms.Label();
      this.btnLoad = new System.Windows.Forms.Button();
      this.cbVersion = new System.Windows.Forms.ComboBox();
      this.lblVersion = new System.Windows.Forms.Label();
      this.ckUseCycloMediaProxy = new System.Windows.Forms.CheckBox();
      this.lblServerUrl = new System.Windows.Forms.Label();
      this.txtSelectWfsLayerLocation = new System.Windows.Forms.TextBox();
      this.lblSelectWfsLayerLocation = new System.Windows.Forms.Label();
      this.plCompatibleLayer = new System.Windows.Forms.Panel();
      this.lvLayers = new System.Windows.Forms.ListView();
      this.LayerName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.VisibleOnMap = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.VisibleInCyclorama = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.lblSelectCompatibleLayer = new System.Windows.Forms.Label();
      this.plSetViewingParameters = new System.Windows.Forms.Panel();
      this.lblColorLayer = new System.Windows.Forms.Label();
      this.btnColor = new System.Windows.Forms.Button();
      this.lblMinimumZoomLevel = new System.Windows.Forms.Label();
      this.updZoomLevel = new System.Windows.Forms.NumericUpDown();
      this.lblLayerName = new System.Windows.Forms.Label();
      this.txtLayerName = new System.Windows.Forms.TextBox();
      this.lblSetViewingParameters = new System.Windows.Forms.Label();
      this.cdWfsLayer = new System.Windows.Forms.ColorDialog();
      this.plSelectFormat.SuspendLayout();
      this.plButtons.SuspendLayout();
      this.plSelectWfsLayer.SuspendLayout();
      this.plCompatibleLayer.SuspendLayout();
      this.plSetViewingParameters.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.updZoomLevel)).BeginInit();
      this.SuspendLayout();
      // 
      // plSelectFormat
      // 
      this.plSelectFormat.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.plSelectFormat.Controls.Add(this.rbSLD);
      this.plSelectFormat.Controls.Add(this.rbWFS);
      this.plSelectFormat.Controls.Add(this.lblSelectFormat);
      this.plSelectFormat.Location = new System.Drawing.Point(0, 0);
      this.plSelectFormat.Name = "plSelectFormat";
      this.plSelectFormat.Size = new System.Drawing.Size(450, 200);
      this.plSelectFormat.TabIndex = 0;
      // 
      // rbSLD
      // 
      this.rbSLD.AutoSize = true;
      this.rbSLD.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rbSLD.Location = new System.Drawing.Point(100, 90);
      this.rbSLD.Name = "rbSLD";
      this.rbSLD.Size = new System.Drawing.Size(240, 20);
      this.rbSLD.TabIndex = 2;
      this.rbSLD.Text = "SLD file (styled layer from local files)";
      this.rbSLD.UseVisualStyleBackColor = true;
      // 
      // rbWFS
      // 
      this.rbWFS.AutoSize = true;
      this.rbWFS.Checked = true;
      this.rbWFS.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rbWFS.Location = new System.Drawing.Point(100, 60);
      this.rbWFS.Name = "rbWFS";
      this.rbWFS.Size = new System.Drawing.Size(185, 20);
      this.rbWFS.TabIndex = 1;
      this.rbWFS.TabStop = true;
      this.rbWFS.Text = "WFS (features from server)";
      this.rbWFS.UseVisualStyleBackColor = true;
      // 
      // lblSelectFormat
      // 
      this.lblSelectFormat.AutoSize = true;
      this.lblSelectFormat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSelectFormat.Location = new System.Drawing.Point(100, 30);
      this.lblSelectFormat.Name = "lblSelectFormat";
      this.lblSelectFormat.Size = new System.Drawing.Size(99, 16);
      this.lblSelectFormat.TabIndex = 0;
      this.lblSelectFormat.Text = "Select format";
      // 
      // plButtons
      // 
      this.plButtons.Controls.Add(this.btnFinish);
      this.plButtons.Controls.Add(this.btnNext);
      this.plButtons.Controls.Add(this.btnBack);
      this.plButtons.Controls.Add(this.btnCancel);
      this.plButtons.Location = new System.Drawing.Point(0, 200);
      this.plButtons.Name = "plButtons";
      this.plButtons.Size = new System.Drawing.Size(450, 50);
      this.plButtons.TabIndex = 2;
      // 
      // btnFinish
      // 
      this.btnFinish.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFinish.Location = new System.Drawing.Point(325, 15);
      this.btnFinish.Name = "btnFinish";
      this.btnFinish.Size = new System.Drawing.Size(100, 25);
      this.btnFinish.TabIndex = 3;
      this.btnFinish.Text = "Finish";
      this.btnFinish.UseVisualStyleBackColor = true;
      this.btnFinish.Click += new System.EventHandler(this.btnFinish_Click);
      // 
      // btnNext
      // 
      this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNext.Location = new System.Drawing.Point(225, 15);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(100, 25);
      this.btnNext.TabIndex = 2;
      this.btnNext.Text = "Next >";
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // btnBack
      // 
      this.btnBack.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnBack.Location = new System.Drawing.Point(125, 15);
      this.btnBack.Name = "btnBack";
      this.btnBack.Size = new System.Drawing.Size(100, 25);
      this.btnBack.TabIndex = 1;
      this.btnBack.Text = "< Back";
      this.btnBack.UseVisualStyleBackColor = true;
      this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(25, 15);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(100, 25);
      this.btnCancel.TabIndex = 0;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // plSelectWfsLayer
      // 
      this.plSelectWfsLayer.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.plSelectWfsLayer.Controls.Add(this.txtStatus);
      this.plSelectWfsLayer.Controls.Add(this.lblStatus);
      this.plSelectWfsLayer.Controls.Add(this.btnLoad);
      this.plSelectWfsLayer.Controls.Add(this.cbVersion);
      this.plSelectWfsLayer.Controls.Add(this.lblVersion);
      this.plSelectWfsLayer.Controls.Add(this.ckUseCycloMediaProxy);
      this.plSelectWfsLayer.Controls.Add(this.lblServerUrl);
      this.plSelectWfsLayer.Controls.Add(this.txtSelectWfsLayerLocation);
      this.plSelectWfsLayer.Controls.Add(this.lblSelectWfsLayerLocation);
      this.plSelectWfsLayer.Location = new System.Drawing.Point(0, 0);
      this.plSelectWfsLayer.Name = "plSelectWfsLayer";
      this.plSelectWfsLayer.Size = new System.Drawing.Size(450, 200);
      this.plSelectWfsLayer.TabIndex = 3;
      // 
      // txtStatus
      // 
      this.txtStatus.BackColor = System.Drawing.SystemColors.ActiveBorder;
      this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtStatus.Location = new System.Drawing.Point(120, 150);
      this.txtStatus.Name = "txtStatus";
      this.txtStatus.Size = new System.Drawing.Size(280, 22);
      this.txtStatus.TabIndex = 8;
      // 
      // lblStatus
      // 
      this.lblStatus.AutoSize = true;
      this.lblStatus.Location = new System.Drawing.Point(70, 150);
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(37, 13);
      this.lblStatus.TabIndex = 7;
      this.lblStatus.Text = "Status";
      // 
      // btnLoad
      // 
      this.btnLoad.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnLoad.Location = new System.Drawing.Point(405, 60);
      this.btnLoad.Name = "btnLoad";
      this.btnLoad.Size = new System.Drawing.Size(40, 25);
      this.btnLoad.TabIndex = 6;
      this.btnLoad.Text = "->";
      this.btnLoad.UseVisualStyleBackColor = true;
      this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
      // 
      // cbVersion
      // 
      this.cbVersion.FormattingEnabled = true;
      this.cbVersion.Location = new System.Drawing.Point(120, 120);
      this.cbVersion.Name = "cbVersion";
      this.cbVersion.Size = new System.Drawing.Size(100, 21);
      this.cbVersion.TabIndex = 5;
      // 
      // lblVersion
      // 
      this.lblVersion.AutoSize = true;
      this.lblVersion.Location = new System.Drawing.Point(70, 120);
      this.lblVersion.Name = "lblVersion";
      this.lblVersion.Size = new System.Drawing.Size(42, 13);
      this.lblVersion.TabIndex = 4;
      this.lblVersion.Text = "Version";
      // 
      // ckUseCycloMediaProxy
      // 
      this.ckUseCycloMediaProxy.AutoSize = true;
      this.ckUseCycloMediaProxy.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.ckUseCycloMediaProxy.Location = new System.Drawing.Point(5, 90);
      this.ckUseCycloMediaProxy.Name = "ckUseCycloMediaProxy";
      this.ckUseCycloMediaProxy.Size = new System.Drawing.Size(131, 17);
      this.ckUseCycloMediaProxy.TabIndex = 3;
      this.ckUseCycloMediaProxy.Text = "Use CycloMedia proxy";
      this.ckUseCycloMediaProxy.UseVisualStyleBackColor = true;
      // 
      // lblServerUrl
      // 
      this.lblServerUrl.AutoSize = true;
      this.lblServerUrl.Location = new System.Drawing.Point(50, 60);
      this.lblServerUrl.Name = "lblServerUrl";
      this.lblServerUrl.Size = new System.Drawing.Size(63, 13);
      this.lblServerUrl.TabIndex = 2;
      this.lblServerUrl.Text = "Server URL";
      // 
      // txtSelectWfsLayerLocation
      // 
      this.txtSelectWfsLayerLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtSelectWfsLayerLocation.Location = new System.Drawing.Point(120, 60);
      this.txtSelectWfsLayerLocation.Name = "txtSelectWfsLayerLocation";
      this.txtSelectWfsLayerLocation.Size = new System.Drawing.Size(280, 22);
      this.txtSelectWfsLayerLocation.TabIndex = 1;
      // 
      // lblSelectWfsLayerLocation
      // 
      this.lblSelectWfsLayerLocation.AutoSize = true;
      this.lblSelectWfsLayerLocation.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSelectWfsLayerLocation.Location = new System.Drawing.Point(120, 30);
      this.lblSelectWfsLayerLocation.Name = "lblSelectWfsLayerLocation";
      this.lblSelectWfsLayerLocation.Size = new System.Drawing.Size(187, 16);
      this.lblSelectWfsLayerLocation.TabIndex = 0;
      this.lblSelectWfsLayerLocation.Text = "Select WFS layer location";
      // 
      // plCompatibleLayer
      // 
      this.plCompatibleLayer.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.plCompatibleLayer.Controls.Add(this.lvLayers);
      this.plCompatibleLayer.Controls.Add(this.lblSelectCompatibleLayer);
      this.plCompatibleLayer.Location = new System.Drawing.Point(0, 0);
      this.plCompatibleLayer.Name = "plCompatibleLayer";
      this.plCompatibleLayer.Size = new System.Drawing.Size(450, 200);
      this.plCompatibleLayer.TabIndex = 9;
      // 
      // lvLayers
      // 
      this.lvLayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LayerName,
            this.VisibleOnMap,
            this.VisibleInCyclorama});
      this.lvLayers.FullRowSelect = true;
      this.lvLayers.Location = new System.Drawing.Point(25, 50);
      this.lvLayers.MultiSelect = false;
      this.lvLayers.Name = "lvLayers";
      this.lvLayers.Size = new System.Drawing.Size(400, 125);
      this.lvLayers.TabIndex = 3;
      this.lvLayers.UseCompatibleStateImageBehavior = false;
      this.lvLayers.View = System.Windows.Forms.View.Details;
      this.lvLayers.SelectedIndexChanged += new System.EventHandler(this.lvLayers_SelectedIndexChanged);
      // 
      // LayerName
      // 
      this.LayerName.Text = "Name";
      this.LayerName.Width = 200;
      // 
      // VisibleOnMap
      // 
      this.VisibleOnMap.Text = "Visible on map";
      this.VisibleOnMap.Width = 80;
      // 
      // VisibleInCyclorama
      // 
      this.VisibleInCyclorama.Text = "Visible in cyclorama";
      this.VisibleInCyclorama.Width = 115;
      // 
      // lblSelectCompatibleLayer
      // 
      this.lblSelectCompatibleLayer.AutoSize = true;
      this.lblSelectCompatibleLayer.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSelectCompatibleLayer.Location = new System.Drawing.Point(25, 25);
      this.lblSelectCompatibleLayer.Name = "lblSelectCompatibleLayer";
      this.lblSelectCompatibleLayer.Size = new System.Drawing.Size(233, 16);
      this.lblSelectCompatibleLayer.TabIndex = 0;
      this.lblSelectCompatibleLayer.Text = "Select a compatible layer to add";
      // 
      // plSetViewingParameters
      // 
      this.plSetViewingParameters.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.plSetViewingParameters.Controls.Add(this.lblColorLayer);
      this.plSetViewingParameters.Controls.Add(this.btnColor);
      this.plSetViewingParameters.Controls.Add(this.lblMinimumZoomLevel);
      this.plSetViewingParameters.Controls.Add(this.updZoomLevel);
      this.plSetViewingParameters.Controls.Add(this.lblLayerName);
      this.plSetViewingParameters.Controls.Add(this.txtLayerName);
      this.plSetViewingParameters.Controls.Add(this.lblSetViewingParameters);
      this.plSetViewingParameters.Location = new System.Drawing.Point(0, 0);
      this.plSetViewingParameters.Name = "plSetViewingParameters";
      this.plSetViewingParameters.Size = new System.Drawing.Size(450, 200);
      this.plSetViewingParameters.TabIndex = 4;
      // 
      // lblColorLayer
      // 
      this.lblColorLayer.AutoSize = true;
      this.lblColorLayer.Location = new System.Drawing.Point(75, 100);
      this.lblColorLayer.Name = "lblColorLayer";
      this.lblColorLayer.Size = new System.Drawing.Size(31, 13);
      this.lblColorLayer.TabIndex = 6;
      this.lblColorLayer.Text = "Color";
      // 
      // btnColor
      // 
      this.btnColor.BackColor = System.Drawing.Color.Green;
      this.btnColor.Location = new System.Drawing.Point(115, 100);
      this.btnColor.Name = "btnColor";
      this.btnColor.Size = new System.Drawing.Size(25, 25);
      this.btnColor.TabIndex = 5;
      this.btnColor.UseVisualStyleBackColor = false;
      this.btnColor.Click += new System.EventHandler(this.btnColor_Click);
      // 
      // lblMinimumZoomLevel
      // 
      this.lblMinimumZoomLevel.AutoSize = true;
      this.lblMinimumZoomLevel.Location = new System.Drawing.Point(5, 75);
      this.lblMinimumZoomLevel.Name = "lblMinimumZoomLevel";
      this.lblMinimumZoomLevel.Size = new System.Drawing.Size(101, 13);
      this.lblMinimumZoomLevel.TabIndex = 4;
      this.lblMinimumZoomLevel.Text = "Minimum zoom level";
      // 
      // updZoomLevel
      // 
      this.updZoomLevel.Location = new System.Drawing.Point(115, 75);
      this.updZoomLevel.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
      this.updZoomLevel.Name = "updZoomLevel";
      this.updZoomLevel.Size = new System.Drawing.Size(200, 20);
      this.updZoomLevel.TabIndex = 3;
      this.updZoomLevel.Value = new decimal(new int[] {
            11,
            0,
            0,
            0});
      // 
      // lblLayerName
      // 
      this.lblLayerName.AutoSize = true;
      this.lblLayerName.Location = new System.Drawing.Point(70, 50);
      this.lblLayerName.Name = "lblLayerName";
      this.lblLayerName.Size = new System.Drawing.Size(35, 13);
      this.lblLayerName.TabIndex = 2;
      this.lblLayerName.Text = "Name";
      // 
      // txtLayerName
      // 
      this.txtLayerName.Location = new System.Drawing.Point(115, 50);
      this.txtLayerName.Name = "txtLayerName";
      this.txtLayerName.Size = new System.Drawing.Size(200, 20);
      this.txtLayerName.TabIndex = 1;
      // 
      // lblSetViewingParameters
      // 
      this.lblSetViewingParameters.AutoSize = true;
      this.lblSetViewingParameters.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSetViewingParameters.Location = new System.Drawing.Point(115, 25);
      this.lblSetViewingParameters.Name = "lblSetViewingParameters";
      this.lblSetViewingParameters.Size = new System.Drawing.Size(170, 16);
      this.lblSetViewingParameters.TabIndex = 0;
      this.lblSetViewingParameters.Text = "Set viewing parameters";
      // 
      // cdWfsLayer
      // 
      this.cdWfsLayer.Color = System.Drawing.Color.Green;
      // 
      // FrmAddLayer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.plButtons);
      this.Controls.Add(this.plSetViewingParameters);
      this.Controls.Add(this.plCompatibleLayer);
      this.Controls.Add(this.plSelectWfsLayer);
      this.Controls.Add(this.plSelectFormat);
      this.Name = "FrmAddLayer";
      this.Size = new System.Drawing.Size(450, 250);
      this.plSelectFormat.ResumeLayout(false);
      this.plSelectFormat.PerformLayout();
      this.plButtons.ResumeLayout(false);
      this.plSelectWfsLayer.ResumeLayout(false);
      this.plSelectWfsLayer.PerformLayout();
      this.plCompatibleLayer.ResumeLayout(false);
      this.plCompatibleLayer.PerformLayout();
      this.plSetViewingParameters.ResumeLayout(false);
      this.plSetViewingParameters.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.updZoomLevel)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel plSelectFormat;
    private System.Windows.Forms.RadioButton rbSLD;
    private System.Windows.Forms.RadioButton rbWFS;
    private System.Windows.Forms.Label lblSelectFormat;
    private System.Windows.Forms.Panel plButtons;
    private System.Windows.Forms.Button btnFinish;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Button btnBack;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Panel plSelectWfsLayer;
    private System.Windows.Forms.Label lblServerUrl;
    private System.Windows.Forms.TextBox txtSelectWfsLayerLocation;
    private System.Windows.Forms.Label lblSelectWfsLayerLocation;
    private System.Windows.Forms.CheckBox ckUseCycloMediaProxy;
    private System.Windows.Forms.ComboBox cbVersion;
    private System.Windows.Forms.Label lblVersion;
    private System.Windows.Forms.Button btnLoad;
    private System.Windows.Forms.TextBox txtStatus;
    private System.Windows.Forms.Label lblStatus;
    private System.Windows.Forms.Panel plCompatibleLayer;
    private System.Windows.Forms.Label lblSelectCompatibleLayer;
    private System.Windows.Forms.ListView lvLayers;
    private System.Windows.Forms.ColumnHeader LayerName;
    private System.Windows.Forms.ColumnHeader VisibleOnMap;
    private System.Windows.Forms.ColumnHeader VisibleInCyclorama;
    private System.Windows.Forms.Panel plSetViewingParameters;
    private System.Windows.Forms.Label lblSetViewingParameters;
    private System.Windows.Forms.Label lblMinimumZoomLevel;
    private System.Windows.Forms.NumericUpDown updZoomLevel;
    private System.Windows.Forms.Label lblLayerName;
    private System.Windows.Forms.TextBox txtLayerName;
    private System.Windows.Forms.Label lblColorLayer;
    private System.Windows.Forms.Button btnColor;
    private System.Windows.Forms.ColorDialog cdWfsLayer;
  }
}
