namespace IntegrationArcMap.Forms
{
  partial class FrmCycloramaSearch
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
      this.btnFind = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.plSettings = new System.Windows.Forms.Panel();
      this.tbCycloramaSearch = new System.Windows.Forms.TabControl();
      this.tbCyclorama = new System.Windows.Forms.TabPage();
      this.lblImageId = new System.Windows.Forms.Label();
      this.txtImageId = new System.Windows.Forms.TextBox();
      this.plButtons = new System.Windows.Forms.Panel();
      this.plResults = new System.Windows.Forms.Panel();
      this.lblResults = new System.Windows.Forms.Label();
      this.lvResults = new System.Windows.Forms.ListView();
      this.chImageId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.chRecordedAt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.backgroundImageSearch = new System.ComponentModel.BackgroundWorker();
      this.prSearching = new System.Windows.Forms.ProgressBar();
      this.plSettings.SuspendLayout();
      this.tbCycloramaSearch.SuspendLayout();
      this.tbCyclorama.SuspendLayout();
      this.plButtons.SuspendLayout();
      this.plResults.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnFind
      // 
      this.btnFind.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnFind.Location = new System.Drawing.Point(10, 10);
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(75, 25);
      this.btnFind.TabIndex = 10;
      this.btnFind.Text = "Find";
      this.btnFind.UseVisualStyleBackColor = true;
      this.btnFind.Click += new System.EventHandler(this.BtnFind_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(10, 147);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 25);
      this.btnCancel.TabIndex = 11;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // plSettings
      // 
      this.plSettings.Controls.Add(this.tbCycloramaSearch);
      this.plSettings.Location = new System.Drawing.Point(0, 0);
      this.plSettings.Name = "plSettings";
      this.plSettings.Size = new System.Drawing.Size(375, 182);
      this.plSettings.TabIndex = 13;
      // 
      // tbCycloramaSearch
      // 
      this.tbCycloramaSearch.Controls.Add(this.tbCyclorama);
      this.tbCycloramaSearch.Location = new System.Drawing.Point(10, 10);
      this.tbCycloramaSearch.Name = "tbCycloramaSearch";
      this.tbCycloramaSearch.SelectedIndex = 0;
      this.tbCycloramaSearch.Size = new System.Drawing.Size(355, 160);
      this.tbCycloramaSearch.TabIndex = 0;
      // 
      // tbCyclorama
      // 
      this.tbCyclorama.Controls.Add(this.lblImageId);
      this.tbCyclorama.Controls.Add(this.txtImageId);
      this.tbCyclorama.Location = new System.Drawing.Point(4, 22);
      this.tbCyclorama.Name = "tbCyclorama";
      this.tbCyclorama.Padding = new System.Windows.Forms.Padding(3);
      this.tbCyclorama.Size = new System.Drawing.Size(347, 134);
      this.tbCyclorama.TabIndex = 0;
      this.tbCyclorama.Text = "Cyclorama";
      this.tbCyclorama.UseVisualStyleBackColor = true;
      // 
      // lblImageId
      // 
      this.lblImageId.Location = new System.Drawing.Point(3, 5);
      this.lblImageId.Name = "lblImageId";
      this.lblImageId.Size = new System.Drawing.Size(50, 20);
      this.lblImageId.TabIndex = 3;
      this.lblImageId.Text = "ImageId:";
      this.lblImageId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // txtImageId
      // 
      this.txtImageId.Location = new System.Drawing.Point(53, 5);
      this.txtImageId.Name = "txtImageId";
      this.txtImageId.Size = new System.Drawing.Size(145, 20);
      this.txtImageId.TabIndex = 4;
      // 
      // plButtons
      // 
      this.plButtons.Controls.Add(this.prSearching);
      this.plButtons.Controls.Add(this.btnFind);
      this.plButtons.Controls.Add(this.btnCancel);
      this.plButtons.Location = new System.Drawing.Point(375, 0);
      this.plButtons.Name = "plButtons";
      this.plButtons.Size = new System.Drawing.Size(95, 182);
      this.plButtons.TabIndex = 14;
      // 
      // plResults
      // 
      this.plResults.Controls.Add(this.lblResults);
      this.plResults.Controls.Add(this.lvResults);
      this.plResults.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.plResults.Location = new System.Drawing.Point(0, 182);
      this.plResults.Name = "plResults";
      this.plResults.Size = new System.Drawing.Size(470, 100);
      this.plResults.TabIndex = 15;
      // 
      // lblResults
      // 
      this.lblResults.AutoSize = true;
      this.lblResults.Location = new System.Drawing.Point(10, 3);
      this.lblResults.Name = "lblResults";
      this.lblResults.Size = new System.Drawing.Size(45, 13);
      this.lblResults.TabIndex = 1;
      this.lblResults.Text = "Results:";
      // 
      // lvResults
      // 
      this.lvResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chImageId,
            this.chRecordedAt});
      this.lvResults.FullRowSelect = true;
      this.lvResults.Location = new System.Drawing.Point(10, 20);
      this.lvResults.MultiSelect = false;
      this.lvResults.Name = "lvResults";
      this.lvResults.Scrollable = false;
      this.lvResults.Size = new System.Drawing.Size(450, 75);
      this.lvResults.TabIndex = 0;
      this.lvResults.UseCompatibleStateImageBehavior = false;
      this.lvResults.View = System.Windows.Forms.View.Details;
      this.lvResults.DoubleClick += new System.EventHandler(this.lvResults_DoubleClick);
      // 
      // chImageId
      // 
      this.chImageId.Text = "ImageId";
      this.chImageId.Width = 100;
      // 
      // chRecordedAt
      // 
      this.chRecordedAt.Text = "RecordedAt";
      this.chRecordedAt.Width = 150;
      // 
      // backgroundImageSearch
      // 
      this.backgroundImageSearch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundImageSearch_DoWork);
      this.backgroundImageSearch.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundImageSearch_RunWorkerCompleted);
      // 
      // prSearching
      // 
      this.prSearching.BackColor = System.Drawing.SystemColors.Control;
      this.prSearching.Location = new System.Drawing.Point(10, 53);
      this.prSearching.Name = "prSearching";
      this.prSearching.Size = new System.Drawing.Size(75, 75);
      this.prSearching.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
      this.prSearching.TabIndex = 12;
      this.prSearching.Visible = false;
      // 
      // FrmCycloramaSearch
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(470, 282);
      this.Controls.Add(this.plResults);
      this.Controls.Add(this.plButtons);
      this.Controls.Add(this.plSettings);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmCycloramaSearch";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Find Cyclorama";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCycloramaSearch_FormClosing);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmCycloramaSearch_FormClosed);
      this.plSettings.ResumeLayout(false);
      this.tbCycloramaSearch.ResumeLayout(false);
      this.tbCyclorama.ResumeLayout(false);
      this.tbCyclorama.PerformLayout();
      this.plButtons.ResumeLayout(false);
      this.plResults.ResumeLayout(false);
      this.plResults.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Panel plSettings;
    private System.Windows.Forms.Panel plButtons;
    private System.Windows.Forms.TabControl tbCycloramaSearch;
    private System.Windows.Forms.TabPage tbCyclorama;
    private System.Windows.Forms.Label lblImageId;
    private System.Windows.Forms.TextBox txtImageId;
    private System.Windows.Forms.Panel plResults;
    private System.Windows.Forms.Label lblResults;
    private System.Windows.Forms.ListView lvResults;
    private System.Windows.Forms.ColumnHeader chImageId;
    private System.Windows.Forms.ColumnHeader chRecordedAt;
    private System.ComponentModel.BackgroundWorker backgroundImageSearch;
    private System.Windows.Forms.ProgressBar prSearching;
  }
}