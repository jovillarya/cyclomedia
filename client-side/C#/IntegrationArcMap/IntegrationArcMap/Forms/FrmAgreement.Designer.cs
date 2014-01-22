namespace IntegrationArcMap.Forms
{
  partial class FrmAgreement
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
      this.plAgreement = new System.Windows.Forms.Panel();
      this.txtAgreement = new System.Windows.Forms.TextBox();
      this.plAccept = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.ckAgreement = new System.Windows.Forms.CheckBox();
      this.plAgreement.SuspendLayout();
      this.plAccept.SuspendLayout();
      this.SuspendLayout();
      // 
      // plAgreement
      // 
      this.plAgreement.Controls.Add(this.txtAgreement);
      this.plAgreement.Dock = System.Windows.Forms.DockStyle.Top;
      this.plAgreement.Location = new System.Drawing.Point(0, 0);
      this.plAgreement.Name = "plAgreement";
      this.plAgreement.Size = new System.Drawing.Size(402, 368);
      this.plAgreement.TabIndex = 15;
      // 
      // txtAgreement
      // 
      this.txtAgreement.BackColor = System.Drawing.SystemColors.Window;
      this.txtAgreement.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.txtAgreement.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtAgreement.Location = new System.Drawing.Point(0, 0);
      this.txtAgreement.Multiline = true;
      this.txtAgreement.Name = "txtAgreement";
      this.txtAgreement.ReadOnly = true;
      this.txtAgreement.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtAgreement.Size = new System.Drawing.Size(402, 368);
      this.txtAgreement.TabIndex = 1;
      // 
      // plAccept
      // 
      this.plAccept.Controls.Add(this.btnCancel);
      this.plAccept.Controls.Add(this.btnOk);
      this.plAccept.Controls.Add(this.ckAgreement);
      this.plAccept.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.plAccept.Location = new System.Drawing.Point(0, 368);
      this.plAccept.Name = "plAccept";
      this.plAccept.Size = new System.Drawing.Size(402, 35);
      this.plAccept.TabIndex = 16;
      // 
      // btnCancel
      // 
      this.btnCancel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnCancel.Location = new System.Drawing.Point(322, 5);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 25);
      this.btnCancel.TabIndex = 12;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnOk
      // 
      this.btnOk.Enabled = false;
      this.btnOk.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btnOk.Location = new System.Drawing.Point(242, 5);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 25);
      this.btnOk.TabIndex = 11;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
      // 
      // ckAgreement
      // 
      this.ckAgreement.AutoSize = true;
      this.ckAgreement.Location = new System.Drawing.Point(5, 5);
      this.ckAgreement.Name = "ckAgreement";
      this.ckAgreement.Size = new System.Drawing.Size(224, 17);
      this.ckAgreement.TabIndex = 0;
      this.ckAgreement.Text = "I have read and agree to the terms of use.";
      this.ckAgreement.UseVisualStyleBackColor = true;
      this.ckAgreement.CheckedChanged += new System.EventHandler(this.ckAgreement_CheckedChanged);
      // 
      // FrmAgreement
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(402, 403);
      this.Controls.Add(this.plAccept);
      this.Controls.Add(this.plAgreement);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.HelpButton = true;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmAgreement";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "GlobeSpotter for ArcGIS Desktop Agreement";
      this.TopMost = true;
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmHelp_FormClosed);
      this.Load += new System.EventHandler(this.FrmHelp_Load);
      this.plAgreement.ResumeLayout(false);
      this.plAgreement.PerformLayout();
      this.plAccept.ResumeLayout(false);
      this.plAccept.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel plAgreement;
    private System.Windows.Forms.Panel plAccept;
    private System.Windows.Forms.TextBox txtAgreement;
    private System.Windows.Forms.CheckBox ckAgreement;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
  }
}