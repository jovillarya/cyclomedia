namespace IntegrationArcMap.Forms
{
  partial class FrmHelp
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
      this.btnNext = new System.Windows.Forms.Button();
      this.btnPrev = new System.Windows.Forms.Button();
      this.pctHelp = new System.Windows.Forms.PictureBox();
      this.txtNumber = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.pctHelp)).BeginInit();
      this.SuspendLayout();
      // 
      // btnNext
      // 
      this.btnNext.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnNext.Image = global::IntegrationArcMap.Properties.Resources.RightArrow;
      this.btnNext.Location = new System.Drawing.Point(514, 845);
      this.btnNext.Name = "btnNext";
      this.btnNext.Size = new System.Drawing.Size(75, 25);
      this.btnNext.TabIndex = 13;
      this.btnNext.UseVisualStyleBackColor = true;
      this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
      // 
      // btnPrev
      // 
      this.btnPrev.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnPrev.Image = global::IntegrationArcMap.Properties.Resources.LeftArrow;
      this.btnPrev.Location = new System.Drawing.Point(434, 845);
      this.btnPrev.Name = "btnPrev";
      this.btnPrev.Size = new System.Drawing.Size(75, 25);
      this.btnPrev.TabIndex = 12;
      this.btnPrev.UseVisualStyleBackColor = true;
      this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
      // 
      // pctHelp
      // 
      this.pctHelp.Dock = System.Windows.Forms.DockStyle.Top;
      this.pctHelp.Location = new System.Drawing.Point(0, 0);
      this.pctHelp.Name = "pctHelp";
      this.pctHelp.Size = new System.Drawing.Size(594, 840);
      this.pctHelp.TabIndex = 0;
      this.pctHelp.TabStop = false;
      // 
      // txtNumber
      // 
      this.txtNumber.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtNumber.Location = new System.Drawing.Point(379, 845);
      this.txtNumber.Name = "txtNumber";
      this.txtNumber.ReadOnly = true;
      this.txtNumber.Size = new System.Drawing.Size(50, 20);
      this.txtNumber.TabIndex = 14;
      this.txtNumber.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // FrmHelp
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(594, 875);
      this.Controls.Add(this.txtNumber);
      this.Controls.Add(this.btnNext);
      this.Controls.Add(this.btnPrev);
      this.Controls.Add(this.pctHelp);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmHelp";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "Help";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmHelp_FormClosed);
      this.Load += new System.EventHandler(this.FrmHelp_Load);
      ((System.ComponentModel.ISupportInitialize)(this.pctHelp)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pctHelp;
    private System.Windows.Forms.Button btnNext;
    private System.Windows.Forms.Button btnPrev;
    private System.Windows.Forms.TextBox txtNumber;
  }
}