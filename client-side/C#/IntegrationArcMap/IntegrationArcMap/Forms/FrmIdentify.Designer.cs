namespace IntegrationArcMap.Forms
{
  partial class FrmIdentify
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
      this.lvFields = new System.Windows.Forms.ListView();
      this.lvColField = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.lvColValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.txtFeatureClassName = new System.Windows.Forms.TextBox();
      this.plFields = new System.Windows.Forms.Panel();
      this.plFeatureClassName = new System.Windows.Forms.Panel();
      this.plFields.SuspendLayout();
      this.plFeatureClassName.SuspendLayout();
      this.SuspendLayout();
      // 
      // lvFields
      // 
      this.lvFields.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lvColField,
            this.lvColValue});
      this.lvFields.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lvFields.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lvFields.GridLines = true;
      this.lvFields.Location = new System.Drawing.Point(0, 0);
      this.lvFields.Name = "lvFields";
      this.lvFields.Size = new System.Drawing.Size(300, 300);
      this.lvFields.TabIndex = 0;
      this.lvFields.UseCompatibleStateImageBehavior = false;
      this.lvFields.View = System.Windows.Forms.View.Details;
      // 
      // lvColField
      // 
      this.lvColField.Text = "Field";
      this.lvColField.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // lvColValue
      // 
      this.lvColValue.Text = "Value";
      this.lvColValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // txtFeatureClassName
      // 
      this.txtFeatureClassName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.txtFeatureClassName.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtFeatureClassName.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtFeatureClassName.Location = new System.Drawing.Point(0, 0);
      this.txtFeatureClassName.Name = "txtFeatureClassName";
      this.txtFeatureClassName.ReadOnly = true;
      this.txtFeatureClassName.Size = new System.Drawing.Size(300, 20);
      this.txtFeatureClassName.TabIndex = 1;
      // 
      // plFields
      // 
      this.plFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.plFields.Controls.Add(this.lvFields);
      this.plFields.Location = new System.Drawing.Point(0, 20);
      this.plFields.Name = "plFields";
      this.plFields.Size = new System.Drawing.Size(300, 300);
      this.plFields.TabIndex = 2;
      // 
      // plFeatureClassName
      // 
      this.plFeatureClassName.Controls.Add(this.txtFeatureClassName);
      this.plFeatureClassName.Dock = System.Windows.Forms.DockStyle.Top;
      this.plFeatureClassName.Location = new System.Drawing.Point(0, 0);
      this.plFeatureClassName.Name = "plFeatureClassName";
      this.plFeatureClassName.Size = new System.Drawing.Size(300, 20);
      this.plFeatureClassName.TabIndex = 3;
      // 
      // FrmIdentify
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.plFeatureClassName);
      this.Controls.Add(this.plFields);
      this.Name = "FrmIdentify";
      this.Size = new System.Drawing.Size(300, 320);
      this.plFields.ResumeLayout(false);
      this.plFeatureClassName.ResumeLayout(false);
      this.plFeatureClassName.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListView lvFields;
    private System.Windows.Forms.ColumnHeader lvColField;
    private System.Windows.Forms.ColumnHeader lvColValue;
    private System.Windows.Forms.TextBox txtFeatureClassName;
    private System.Windows.Forms.Panel plFields;
    private System.Windows.Forms.Panel plFeatureClassName;
  }
}
