namespace IntegrationArcMap.Forms
{
  partial class FrmLayers
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
      this.tvLayers = new System.Windows.Forms.TreeView();
      this.SuspendLayout();
      // 
      // tvLayers
      // 
      this.tvLayers.CheckBoxes = true;
      this.tvLayers.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tvLayers.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
      this.tvLayers.Location = new System.Drawing.Point(0, 0);
      this.tvLayers.Name = "tvLayers";
      this.tvLayers.Size = new System.Drawing.Size(200, 200);
      this.tvLayers.TabIndex = 1;
      this.tvLayers.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvLayers_AfterCheck);
      this.tvLayers.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.tvLayers_DrawNode);
      this.tvLayers.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvLayers_NodeMouseClick);
      // 
      // FrmLayers
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tvLayers);
      this.Name = "FrmLayers";
      this.Size = new System.Drawing.Size(200, 200);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TreeView tvLayers;
  }
}
