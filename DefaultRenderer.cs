using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ResourceExplorer
{
  public class DefaultRenderer : Renderer
  {
    private System.Windows.Forms.Label valueLabel;
    private System.Windows.Forms.Label typeLabel;

    public DefaultRenderer()
    {
      InitializeComponent();
    }

    public override void LoadData(string resourceName, object data)
    {
      if (data == null)
      {
        typeLabel.Text = "";
        valueLabel.Text = "";
      }
      else
      {
        typeLabel.Text = data.GetType().Name;
        valueLabel.Text = data.ToString();
      }
    }

    private void InitializeComponent()
    {
      this.typeLabel = new System.Windows.Forms.Label();
      this.valueLabel = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // typeLabel
      // 
      this.typeLabel.AutoSize = true;
      this.typeLabel.Location = new System.Drawing.Point(13, 11);
      this.typeLabel.Name = "typeLabel";
      this.typeLabel.Size = new System.Drawing.Size(35, 13);
      this.typeLabel.TabIndex = 0;
      this.typeLabel.Text = "label1";
      // 
      // valueLabel
      // 
      this.valueLabel.AutoSize = true;
      this.valueLabel.Location = new System.Drawing.Point(13, 34);
      this.valueLabel.Name = "valueLabel";
      this.valueLabel.Size = new System.Drawing.Size(35, 13);
      this.valueLabel.TabIndex = 1;
      this.valueLabel.Text = "label2";
      // 
      // DefaultRenderer
      // 
      this.Controls.Add(this.valueLabel);
      this.Controls.Add(this.typeLabel);
      this.Name = "DefaultRenderer";
      this.Size = new System.Drawing.Size(544, 351);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    public override void CopyToClipboard()
    {
      Clipboard.SetText(valueLabel.Text);
    }
  }
}
