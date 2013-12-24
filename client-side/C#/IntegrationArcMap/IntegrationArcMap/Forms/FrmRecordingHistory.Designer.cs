using RangeControl;

namespace IntegrationArcMap.Forms
{
  sealed partial class FrmRecordingHistory
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
            this.lblPoints = new System.Windows.Forms.Label();
            this.rsRecordingSelector = new RangeControl.RangeSelectorControl();
            this.SuspendLayout();
            // 
            // lblPoints
            // 
            this.lblPoints.BackColor = System.Drawing.Color.Transparent;
            this.lblPoints.Location = new System.Drawing.Point(0, 0);
            this.lblPoints.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPoints.Name = "lblPoints";
            this.lblPoints.Size = new System.Drawing.Size(482, 10);
            this.lblPoints.TabIndex = 6;
            // 
            // rsRecordingSelector
            // 
            this.rsRecordingSelector.BackColor = System.Drawing.Color.Transparent;
            this.rsRecordingSelector.DelimiterForRange = ",";
            this.rsRecordingSelector.DisabledBarColor = System.Drawing.Color.Gray;
            this.rsRecordingSelector.DisabledRangeLabelColor = System.Drawing.Color.Gray;
            this.rsRecordingSelector.GapFromLeftMargin = ((uint)(20u));
            this.rsRecordingSelector.GapFromRightMargin = ((uint)(15u));
            this.rsRecordingSelector.HeightOfThumb = 18F;
            this.rsRecordingSelector.InFocusBarColor = System.Drawing.SystemColors.Highlight;
            this.rsRecordingSelector.InFocusRangeLabelColor = System.Drawing.SystemColors.Highlight;
            this.rsRecordingSelector.LabelFont = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rsRecordingSelector.LeftThumbImagePath = "RangeControl.Slider.png";
            this.rsRecordingSelector.Location = new System.Drawing.Point(0, 0);
            this.rsRecordingSelector.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.rsRecordingSelector.MiddleBarWidth = ((uint)(2u));
            this.rsRecordingSelector.Name = "rsRecordingSelector";
            this.rsRecordingSelector.OutputStringFontColor = System.Drawing.Color.Black;
            this.rsRecordingSelector.Range1 = "2012";
            this.rsRecordingSelector.Range2 = "2014";
            this.rsRecordingSelector.RangeString = "Recording range from";
            this.rsRecordingSelector.RangeValues = "2000,2001,2002,2003,2004,2005,2006,2007,2008,2009,2010,2011,2012,2013,2014";
            this.rsRecordingSelector.RightThumbImagePath = "RangeControl.Slider.png";
            this.rsRecordingSelector.Size = new System.Drawing.Size(482, 64);
            this.rsRecordingSelector.TabIndex = 5;
            this.rsRecordingSelector.ThumbColor = System.Drawing.SystemColors.Highlight;
            this.rsRecordingSelector.WidthOfThumb = 10F;
            this.rsRecordingSelector.XmlFileName = null;
            this.rsRecordingSelector.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rsRecordingSelector_MouseUp);
            // 
            // FrmRecordingHistory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 67);
            this.Controls.Add(this.lblPoints);
            this.Controls.Add(this.rsRecordingSelector);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmRecordingHistory";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CycloMedia Recording History";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FrmRecordingHistory_FormClosed);
            this.Load += new System.EventHandler(this.FrmRecordingHistory_Load);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblPoints;
    private RangeSelectorControl rsRecordingSelector;
  }
}