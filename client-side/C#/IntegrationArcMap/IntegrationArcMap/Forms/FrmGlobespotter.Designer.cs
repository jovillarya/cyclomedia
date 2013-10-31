namespace IntegrationArcMap.Forms
{
  partial class FrmGlobespotter
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
      this.plGlobespotter = new System.Windows.Forms.Panel();
      this.SuspendLayout();
      // 
      // plGlobespotter
      // 
      this.plGlobespotter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.plGlobespotter.Location = new System.Drawing.Point(0, 0);
      this.plGlobespotter.Name = "plGlobespotter";
      this.plGlobespotter.Size = new System.Drawing.Size(600, 600);
      this.plGlobespotter.TabIndex = 0;
      this.plGlobespotter.MouseEnter += new System.EventHandler(this.PlGlobespotterMouseEnter);
      this.plGlobespotter.MouseLeave += new System.EventHandler(this.PlGlobespotterMouseLeave);
      // 
      // FrmGlobespotter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.plGlobespotter);
      this.Name = "FrmGlobespotter";
      this.Size = new System.Drawing.Size(600, 600);
      this.Load += new System.EventHandler(this.FrmGlobespotterLoad);
      this.ResumeLayout(false);

    }

    #endregion

    public System.Windows.Forms.Panel plGlobespotter;


  }
}
