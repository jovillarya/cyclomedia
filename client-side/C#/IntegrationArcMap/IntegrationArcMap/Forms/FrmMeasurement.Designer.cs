namespace IntegrationArcMap.Forms
{
  partial class FrmMeasurement
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMeasurement));
      this.lvObservations = new System.Windows.Forms.ListView();
      this.ImageId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.ImageStd = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Trash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.lblReliability = new System.Windows.Forms.Label();
      this.RelO = new System.Windows.Forms.Label();
      this.btnShow = new System.Windows.Forms.Button();
      this.lblObservations = new System.Windows.Forms.Label();
      this.lblMatches = new System.Windows.Forms.Label();
      this.lblPositionStd = new System.Windows.Forms.Label();
      this.txtPositionStd = new System.Windows.Forms.TextBox();
      this.lblPosition = new System.Windows.Forms.Label();
      this.lblSelectedMeasurementDetails = new System.Windows.Forms.Label();
      this.plMeasurementDetails = new System.Windows.Forms.Panel();
      this.plOpenFocus = new System.Windows.Forms.Panel();
      this.tsMeasurement = new System.Windows.Forms.ToolStrip();
      this.tsBtOpen = new System.Windows.Forms.ToolStripButton();
      this.tsBtFocus = new System.Windows.Forms.ToolStripButton();
      this.plReliability = new System.Windows.Forms.Panel();
      this.plMatchesImages = new System.Windows.Forms.Panel();
      this.plImages = new System.Windows.Forms.Panel();
      this.pctImages = new System.Windows.Forms.PictureBox();
      this.plMatches = new System.Windows.Forms.Panel();
      this.plPosition = new System.Windows.Forms.Panel();
      this.lblNumber = new System.Windows.Forms.Label();
      this.txtNumber = new System.Windows.Forms.TextBox();
      this.txtPosition = new System.Windows.Forms.TextBox();
      this.plObservations = new System.Windows.Forms.Panel();
      this.plButtons = new System.Windows.Forms.Panel();
      this.btnNext = new System.Windows.Forms.Button();
      this.btnPrev = new System.Windows.Forms.Button();
      this.btnOpenClose = new System.Windows.Forms.Button();
      this.btnUndo = new System.Windows.Forms.Button();
      this.plMeasurementDetails.SuspendLayout();
      this.plOpenFocus.SuspendLayout();
      this.tsMeasurement.SuspendLayout();
      this.plReliability.SuspendLayout();
      this.plMatchesImages.SuspendLayout();
      this.plImages.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pctImages)).BeginInit();
      this.plMatches.SuspendLayout();
      this.plPosition.SuspendLayout();
      this.plObservations.SuspendLayout();
      this.plButtons.SuspendLayout();
      this.SuspendLayout();
      // 
      // lvObservations
      // 
      this.lvObservations.Alignment = System.Windows.Forms.ListViewAlignment.Default;
      this.lvObservations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ImageId,
            this.ImageStd,
            this.Trash});
      this.lvObservations.Dock = System.Windows.Forms.DockStyle.Left;
      this.lvObservations.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lvObservations.FullRowSelect = true;
      this.lvObservations.Location = new System.Drawing.Point(0, 0);
      this.lvObservations.MultiSelect = false;
      this.lvObservations.Name = "lvObservations";
      this.lvObservations.OwnerDraw = true;
      this.lvObservations.Scrollable = false;
      this.lvObservations.Size = new System.Drawing.Size(276, 276);
      this.lvObservations.TabIndex = 8;
      this.lvObservations.UseCompatibleStateImageBehavior = false;
      this.lvObservations.View = System.Windows.Forms.View.Details;
      this.lvObservations.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvObservations_DrawColumnHeader);
      this.lvObservations.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvObservations_DrawItem);
      this.lvObservations.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvObservations_DrawSubItem);
      this.lvObservations.SelectedIndexChanged += new System.EventHandler(this.lvObservations_SelectedIndexChanged);
      this.lvObservations.DoubleClick += new System.EventHandler(this.lvObservations_DoubleClick);
      this.lvObservations.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvObservations_MouseClick);
      // 
      // ImageId
      // 
      this.ImageId.Text = "ImageId";
      this.ImageId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.ImageId.Width = 132;
      // 
      // ImageStd
      // 
      this.ImageStd.Text = "Image-σ(x,y,z)[m]";
      this.ImageStd.Width = 99;
      // 
      // Trash
      // 
      this.Trash.Text = "Trash";
      this.Trash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // lblReliability
      // 
      this.lblReliability.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblReliability.Location = new System.Drawing.Point(5, 5);
      this.lblReliability.Name = "lblReliability";
      this.lblReliability.Size = new System.Drawing.Size(62, 25);
      this.lblReliability.TabIndex = 9;
      this.lblReliability.Text = "Reliability:";
      this.lblReliability.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // RelO
      // 
      this.RelO.BackColor = System.Drawing.SystemColors.Window;
      this.RelO.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RelO.Location = new System.Drawing.Point(67, 5);
      this.RelO.Name = "RelO";
      this.RelO.Size = new System.Drawing.Size(18, 25);
      this.RelO.TabIndex = 10;
      this.RelO.Text = " ";
      this.RelO.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btnShow
      // 
      this.btnShow.Enabled = false;
      this.btnShow.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnShow.Location = new System.Drawing.Point(5, 5);
      this.btnShow.Name = "btnShow";
      this.btnShow.Size = new System.Drawing.Size(65, 25);
      this.btnShow.TabIndex = 12;
      this.btnShow.Text = "Show";
      this.btnShow.UseVisualStyleBackColor = true;
      this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
      // 
      // lblObservations
      // 
      this.lblObservations.AutoSize = true;
      this.lblObservations.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblObservations.Location = new System.Drawing.Point(5, 35);
      this.lblObservations.Name = "lblObservations";
      this.lblObservations.Size = new System.Drawing.Size(75, 14);
      this.lblObservations.TabIndex = 13;
      this.lblObservations.Text = "Observations:";
      // 
      // lblMatches
      // 
      this.lblMatches.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblMatches.Location = new System.Drawing.Point(5, 0);
      this.lblMatches.Name = "lblMatches";
      this.lblMatches.Size = new System.Drawing.Size(62, 20);
      this.lblMatches.TabIndex = 15;
      this.lblMatches.Text = "Matches:";
      this.lblMatches.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lblPositionStd
      // 
      this.lblPositionStd.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblPositionStd.Location = new System.Drawing.Point(5, 65);
      this.lblPositionStd.Name = "lblPositionStd";
      this.lblPositionStd.Size = new System.Drawing.Size(62, 20);
      this.lblPositionStd.TabIndex = 16;
      this.lblPositionStd.Text = "σ(x,y,z)[m]:";
      this.lblPositionStd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtPositionStd
      // 
      this.txtPositionStd.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtPositionStd.Location = new System.Drawing.Point(67, 65);
      this.txtPositionStd.Name = "txtPositionStd";
      this.txtPositionStd.ReadOnly = true;
      this.txtPositionStd.Size = new System.Drawing.Size(204, 20);
      this.txtPositionStd.TabIndex = 17;
      // 
      // lblPosition
      // 
      this.lblPosition.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblPosition.Location = new System.Drawing.Point(5, 45);
      this.lblPosition.Name = "lblPosition";
      this.lblPosition.Size = new System.Drawing.Size(62, 20);
      this.lblPosition.TabIndex = 18;
      this.lblPosition.Text = "Position:";
      this.lblPosition.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // lblSelectedMeasurementDetails
      // 
      this.lblSelectedMeasurementDetails.AutoSize = true;
      this.lblSelectedMeasurementDetails.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSelectedMeasurementDetails.Location = new System.Drawing.Point(5, 5);
      this.lblSelectedMeasurementDetails.Name = "lblSelectedMeasurementDetails";
      this.lblSelectedMeasurementDetails.Size = new System.Drawing.Size(154, 14);
      this.lblSelectedMeasurementDetails.TabIndex = 19;
      this.lblSelectedMeasurementDetails.Text = "Selected measurement details:";
      // 
      // plMeasurementDetails
      // 
      this.plMeasurementDetails.BackColor = System.Drawing.SystemColors.Window;
      this.plMeasurementDetails.Controls.Add(this.plOpenFocus);
      this.plMeasurementDetails.Controls.Add(this.plReliability);
      this.plMeasurementDetails.Controls.Add(this.plMatchesImages);
      this.plMeasurementDetails.Controls.Add(this.plPosition);
      this.plMeasurementDetails.Dock = System.Windows.Forms.DockStyle.Top;
      this.plMeasurementDetails.Location = new System.Drawing.Point(0, 0);
      this.plMeasurementDetails.Name = "plMeasurementDetails";
      this.plMeasurementDetails.Size = new System.Drawing.Size(276, 190);
      this.plMeasurementDetails.TabIndex = 20;
      // 
      // plOpenFocus
      // 
      this.plOpenFocus.BackColor = System.Drawing.SystemColors.Control;
      this.plOpenFocus.Controls.Add(this.tsMeasurement);
      this.plOpenFocus.Dock = System.Windows.Forms.DockStyle.Top;
      this.plOpenFocus.Location = new System.Drawing.Point(0, 0);
      this.plOpenFocus.Name = "plOpenFocus";
      this.plOpenFocus.Size = new System.Drawing.Size(276, 25);
      this.plOpenFocus.TabIndex = 24;
      // 
      // tsMeasurement
      // 
      this.tsMeasurement.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tsMeasurement.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsBtOpen,
            this.tsBtFocus});
      this.tsMeasurement.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
      this.tsMeasurement.Location = new System.Drawing.Point(0, 0);
      this.tsMeasurement.Name = "tsMeasurement";
      this.tsMeasurement.Size = new System.Drawing.Size(276, 25);
      this.tsMeasurement.TabIndex = 0;
      // 
      // tsBtOpen
      // 
      this.tsBtOpen.AutoSize = false;
      this.tsBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsBtOpen.Image = ((System.Drawing.Image)(resources.GetObject("tsBtOpen.Image")));
      this.tsBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsBtOpen.Name = "tsBtOpen";
      this.tsBtOpen.Size = new System.Drawing.Size(25, 25);
      this.tsBtOpen.Text = "Open";
      this.tsBtOpen.ToolTipText = "Open nearest cyclorama(s) to view the selected item";
      this.tsBtOpen.Click += new System.EventHandler(this.tsBtOpen_Click);
      // 
      // tsBtFocus
      // 
      this.tsBtFocus.AutoSize = false;
      this.tsBtFocus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.tsBtFocus.Image = ((System.Drawing.Image)(resources.GetObject("tsBtFocus.Image")));
      this.tsBtFocus.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.tsBtFocus.Name = "tsBtFocus";
      this.tsBtFocus.Size = new System.Drawing.Size(25, 25);
      this.tsBtFocus.Text = "Focus";
      this.tsBtFocus.ToolTipText = "Focus all viewers on the selected item";
      this.tsBtFocus.Click += new System.EventHandler(this.tsBtFocus_Click);
      // 
      // plReliability
      // 
      this.plReliability.Controls.Add(this.lblReliability);
      this.plReliability.Controls.Add(this.lblObservations);
      this.plReliability.Controls.Add(this.RelO);
      this.plReliability.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.plReliability.Location = new System.Drawing.Point(0, 135);
      this.plReliability.Name = "plReliability";
      this.plReliability.Size = new System.Drawing.Size(276, 55);
      this.plReliability.TabIndex = 26;
      // 
      // plMatchesImages
      // 
      this.plMatchesImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.plMatchesImages.Controls.Add(this.plImages);
      this.plMatchesImages.Controls.Add(this.plMatches);
      this.plMatchesImages.Location = new System.Drawing.Point(0, 115);
      this.plMatchesImages.Name = "plMatchesImages";
      this.plMatchesImages.Size = new System.Drawing.Size(272, 20);
      this.plMatchesImages.TabIndex = 25;
      // 
      // plImages
      // 
      this.plImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.plImages.Controls.Add(this.pctImages);
      this.plImages.Location = new System.Drawing.Point(67, 0);
      this.plImages.Name = "plImages";
      this.plImages.Size = new System.Drawing.Size(209, 20);
      this.plImages.TabIndex = 18;
      // 
      // pctImages
      // 
      this.pctImages.Dock = System.Windows.Forms.DockStyle.Left;
      this.pctImages.Location = new System.Drawing.Point(0, 0);
      this.pctImages.Name = "pctImages";
      this.pctImages.Size = new System.Drawing.Size(204, 20);
      this.pctImages.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.pctImages.TabIndex = 16;
      this.pctImages.TabStop = false;
      this.pctImages.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pctImages_MouseClick);
      this.pctImages.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pctImages_MouseDoubleClick);
      // 
      // plMatches
      // 
      this.plMatches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.plMatches.Controls.Add(this.lblMatches);
      this.plMatches.Location = new System.Drawing.Point(0, 0);
      this.plMatches.Name = "plMatches";
      this.plMatches.Size = new System.Drawing.Size(67, 20);
      this.plMatches.TabIndex = 17;
      // 
      // plPosition
      // 
      this.plPosition.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.plPosition.Controls.Add(this.lblNumber);
      this.plPosition.Controls.Add(this.txtNumber);
      this.plPosition.Controls.Add(this.lblSelectedMeasurementDetails);
      this.plPosition.Controls.Add(this.lblPosition);
      this.plPosition.Controls.Add(this.txtPosition);
      this.plPosition.Controls.Add(this.lblPositionStd);
      this.plPosition.Controls.Add(this.txtPositionStd);
      this.plPosition.Location = new System.Drawing.Point(0, 25);
      this.plPosition.Name = "plPosition";
      this.plPosition.Size = new System.Drawing.Size(276, 90);
      this.plPosition.TabIndex = 24;
      // 
      // lblNumber
      // 
      this.lblNumber.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblNumber.Location = new System.Drawing.Point(5, 25);
      this.lblNumber.Name = "lblNumber";
      this.lblNumber.Size = new System.Drawing.Size(62, 20);
      this.lblNumber.TabIndex = 22;
      this.lblNumber.Text = "Number:";
      this.lblNumber.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtNumber
      // 
      this.txtNumber.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtNumber.Location = new System.Drawing.Point(67, 25);
      this.txtNumber.Name = "txtNumber";
      this.txtNumber.ReadOnly = true;
      this.txtNumber.Size = new System.Drawing.Size(204, 20);
      this.txtNumber.TabIndex = 21;
      // 
      // txtPosition
      // 
      this.txtPosition.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtPosition.Location = new System.Drawing.Point(67, 45);
      this.txtPosition.Name = "txtPosition";
      this.txtPosition.ReadOnly = true;
      this.txtPosition.Size = new System.Drawing.Size(204, 20);
      this.txtPosition.TabIndex = 20;
      // 
      // plObservations
      // 
      this.plObservations.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.plObservations.BackColor = System.Drawing.SystemColors.Window;
      this.plObservations.Controls.Add(this.lvObservations);
      this.plObservations.Location = new System.Drawing.Point(0, 190);
      this.plObservations.Name = "plObservations";
      this.plObservations.Size = new System.Drawing.Size(276, 276);
      this.plObservations.TabIndex = 21;
      // 
      // plButtons
      // 
      this.plButtons.BackColor = System.Drawing.SystemColors.Window;
      this.plButtons.Controls.Add(this.btnUndo);
      this.plButtons.Controls.Add(this.btnNext);
      this.plButtons.Controls.Add(this.btnPrev);
      this.plButtons.Controls.Add(this.btnOpenClose);
      this.plButtons.Controls.Add(this.btnShow);
      this.plButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.plButtons.Location = new System.Drawing.Point(0, 466);
      this.plButtons.Name = "plButtons";
      this.plButtons.Size = new System.Drawing.Size(276, 35);
      this.plButtons.TabIndex = 22;
      // 
      // btnNext
      // 
      this.btnNext.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNext.Image = ((System.Drawing.Image)(resources.GetObject("btnNext.Image")));
      this.btnNext.Location = new System.Drawing.Point(245, 5);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(25, 25);
      this.btnNext.TabIndex = 16;
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // btnPrev
      // 
      this.btnPrev.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrev.Image = ((System.Drawing.Image)(resources.GetObject("btnPrev.Image")));
      this.btnPrev.Location = new System.Drawing.Point(215, 5);
      this.btnPrev.Name = "btnPrev";
      this.btnPrev.Size = new System.Drawing.Size(25, 25);
      this.btnPrev.TabIndex = 15;
      this.btnPrev.UseVisualStyleBackColor = true;
      this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
      // 
      // btnOpenClose
      // 
      this.btnOpenClose.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnOpenClose.Location = new System.Drawing.Point(75, 5);
      this.btnOpenClose.Name = "btnOpenClose";
      this.btnOpenClose.Size = new System.Drawing.Size(65, 25);
      this.btnOpenClose.TabIndex = 13;
      this.btnOpenClose.Text = "Close";
      this.btnOpenClose.UseVisualStyleBackColor = true;
      this.btnOpenClose.Click += new System.EventHandler(this.btnOpenClose_Click);
      // 
      // btnUndo
      // 
      this.btnUndo.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnUndo.Location = new System.Drawing.Point(145, 5);
      this.btnUndo.Name = "btnUndo";
      this.btnUndo.Size = new System.Drawing.Size(65, 25);
      this.btnUndo.TabIndex = 17;
      this.btnUndo.Text = "Undo";
      this.btnUndo.UseVisualStyleBackColor = true;
      this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
      // 
      // FrmMeasurement
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.plButtons);
      this.Controls.Add(this.plObservations);
      this.Controls.Add(this.plMeasurementDetails);
      this.Name = "FrmMeasurement";
      this.Size = new System.Drawing.Size(276, 501);
      this.plMeasurementDetails.ResumeLayout(false);
      this.plOpenFocus.ResumeLayout(false);
      this.plOpenFocus.PerformLayout();
      this.tsMeasurement.ResumeLayout(false);
      this.tsMeasurement.PerformLayout();
      this.plReliability.ResumeLayout(false);
      this.plReliability.PerformLayout();
      this.plMatchesImages.ResumeLayout(false);
      this.plImages.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pctImages)).EndInit();
      this.plMatches.ResumeLayout(false);
      this.plPosition.ResumeLayout(false);
      this.plPosition.PerformLayout();
      this.plObservations.ResumeLayout(false);
      this.plButtons.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lvObservations;
    private System.Windows.Forms.ColumnHeader ImageId;
    private System.Windows.Forms.ColumnHeader Trash;
    private System.Windows.Forms.Label lblReliability;
    private System.Windows.Forms.Label RelO;
    private System.Windows.Forms.Button btnShow;
    private System.Windows.Forms.ColumnHeader ImageStd;
    private System.Windows.Forms.Label lblObservations;
    private System.Windows.Forms.Label lblMatches;
    private System.Windows.Forms.Label lblPositionStd;
    private System.Windows.Forms.TextBox txtPositionStd;
    private System.Windows.Forms.Label lblPosition;
    private System.Windows.Forms.Label lblSelectedMeasurementDetails;
    private System.Windows.Forms.Panel plMeasurementDetails;
    private System.Windows.Forms.TextBox txtPosition;
    private System.Windows.Forms.Panel plObservations;
    private System.Windows.Forms.Panel plButtons;
    private System.Windows.Forms.Panel plOpenFocus;
    private System.Windows.Forms.ToolStrip tsMeasurement;
    private System.Windows.Forms.ToolStripButton tsBtOpen;
    private System.Windows.Forms.ToolStripButton tsBtFocus;
    private System.Windows.Forms.Panel plMatchesImages;
    private System.Windows.Forms.Panel plPosition;
    private System.Windows.Forms.Panel plReliability;
    private System.Windows.Forms.PictureBox pctImages;
    private System.Windows.Forms.Panel plImages;
    private System.Windows.Forms.Panel plMatches;
    private System.Windows.Forms.Button btnOpenClose;
    private System.Windows.Forms.Button btnPrev;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Label lblNumber;
    private System.Windows.Forms.TextBox txtNumber;
    private System.Windows.Forms.Button btnUndo;

  }
}
