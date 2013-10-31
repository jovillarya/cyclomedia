namespace IntegrationArcMap.Forms
{
  partial class FrmSmartClick
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
      this.btnDelete = new System.Windows.Forms.Button();
      this.btnCheck = new System.Windows.Forms.Button();
      this.pctImageResult3 = new System.Windows.Forms.PictureBox();
      this.pctImageResult2 = new System.Windows.Forms.PictureBox();
      this.pctImageResult1 = new System.Windows.Forms.PictureBox();
      this.lvObservations = new System.Windows.Forms.ListView();
      this.ImageId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.Trash = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.lblReliability = new System.Windows.Forms.Label();
      this.RelO = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pctImageResult3)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pctImageResult2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pctImageResult1)).BeginInit();
      this.SuspendLayout();
      // 
      // btnDelete
      // 
      this.btnDelete.Enabled = false;
      this.btnDelete.Image = global::IntegrationArcMap.Properties.Resources.GsDelete;
      this.btnDelete.Location = new System.Drawing.Point(234, 9);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(32, 32);
      this.btnDelete.TabIndex = 7;
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // btnCheck
      // 
      this.btnCheck.Enabled = false;
      this.btnCheck.Image = global::IntegrationArcMap.Properties.Resources.GsCheck;
      this.btnCheck.Location = new System.Drawing.Point(34, 9);
      this.btnCheck.Name = "btnCheck";
      this.btnCheck.Size = new System.Drawing.Size(32, 32);
      this.btnCheck.TabIndex = 6;
      this.btnCheck.UseVisualStyleBackColor = true;
      this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
      // 
      // pctImageResult3
      // 
      this.pctImageResult3.Location = new System.Drawing.Point(200, 50);
      this.pctImageResult3.Name = "pctImageResult3";
      this.pctImageResult3.Size = new System.Drawing.Size(100, 100);
      this.pctImageResult3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pctImageResult3.TabIndex = 5;
      this.pctImageResult3.TabStop = false;
      this.pctImageResult3.Click += new System.EventHandler(this.pctImageResult3_Click);
      // 
      // pctImageResult2
      // 
      this.pctImageResult2.Location = new System.Drawing.Point(100, 50);
      this.pctImageResult2.Name = "pctImageResult2";
      this.pctImageResult2.Size = new System.Drawing.Size(100, 100);
      this.pctImageResult2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pctImageResult2.TabIndex = 4;
      this.pctImageResult2.TabStop = false;
      this.pctImageResult2.Click += new System.EventHandler(this.pctImageResult2_Click);
      // 
      // pctImageResult1
      // 
      this.pctImageResult1.Location = new System.Drawing.Point(0, 50);
      this.pctImageResult1.Name = "pctImageResult1";
      this.pctImageResult1.Size = new System.Drawing.Size(100, 100);
      this.pctImageResult1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pctImageResult1.TabIndex = 3;
      this.pctImageResult1.TabStop = false;
      this.pctImageResult1.Click += new System.EventHandler(this.pctImageResult1_Click);
      // 
      // lvObservations
      // 
      this.lvObservations.Alignment = System.Windows.Forms.ListViewAlignment.Default;
      this.lvObservations.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ImageId,
            this.Trash});
      this.lvObservations.FullRowSelect = true;
      this.lvObservations.Location = new System.Drawing.Point(0, 205);
      this.lvObservations.MultiSelect = false;
      this.lvObservations.Name = "lvObservations";
      this.lvObservations.OwnerDraw = true;
      this.lvObservations.Scrollable = false;
      this.lvObservations.Size = new System.Drawing.Size(300, 140);
      this.lvObservations.TabIndex = 8;
      this.lvObservations.UseCompatibleStateImageBehavior = false;
      this.lvObservations.View = System.Windows.Forms.View.Details;
      this.lvObservations.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.lvObservations_DrawColumnHeader);
      this.lvObservations.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.lvObservations_DrawItem);
      this.lvObservations.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.lvObservations_DrawSubItem);
      this.lvObservations.DoubleClick += new System.EventHandler(this.lvObservations_DoubleClick);
      this.lvObservations.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvObservations_MouseClick);
      // 
      // ImageId
      // 
      this.ImageId.Text = "ImageId";
      this.ImageId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.ImageId.Width = 92;
      // 
      // Trash
      // 
      this.Trash.Text = "Trash";
      this.Trash.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // lblReliability
      // 
      this.lblReliability.AutoSize = true;
      this.lblReliability.Location = new System.Drawing.Point(30, 165);
      this.lblReliability.Name = "lblReliability";
      this.lblReliability.Size = new System.Drawing.Size(51, 13);
      this.lblReliability.TabIndex = 9;
      this.lblReliability.Text = "Reliability";
      // 
      // RelO
      // 
      this.RelO.BackColor = System.Drawing.SystemColors.Control;
      this.RelO.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.RelO.Location = new System.Drawing.Point(84, 161);
      this.RelO.Name = "RelO";
      this.RelO.Size = new System.Drawing.Size(18, 25);
      this.RelO.TabIndex = 10;
      this.RelO.Text = " ";
      // 
      // FrmSmartClick
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.RelO);
      this.Controls.Add(this.lblReliability);
      this.Controls.Add(this.lvObservations);
      this.Controls.Add(this.btnDelete);
      this.Controls.Add(this.btnCheck);
      this.Controls.Add(this.pctImageResult3);
      this.Controls.Add(this.pctImageResult2);
      this.Controls.Add(this.pctImageResult1);
      this.Name = "FrmSmartClick";
      this.Size = new System.Drawing.Size(300, 350);
      ((System.ComponentModel.ISupportInitialize)(this.pctImageResult3)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pctImageResult2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pctImageResult1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pctImageResult3;
    private System.Windows.Forms.PictureBox pctImageResult2;
    private System.Windows.Forms.PictureBox pctImageResult1;
    private System.Windows.Forms.Button btnCheck;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.ListView lvObservations;
    private System.Windows.Forms.ColumnHeader ImageId;
    private System.Windows.Forms.ColumnHeader Trash;
    private System.Windows.Forms.Label lblReliability;
    private System.Windows.Forms.Label RelO;

  }
}
