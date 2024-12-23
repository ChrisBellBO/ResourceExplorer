using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace ResourceExplorer
{
  class TextRenderer : Renderer
  {
    private System.Windows.Forms.TextBox dataBox;
  
    public TextRenderer()
    {
      InitializeComponent();
    }

    public override void LoadData(string resourceName, object data)
    {
      if (data is string)
      {
        dataBox.Text = data.ToString();
      }
      else if (data is string[])
      {
        dataBox.Lines = data as string[];
      }
      else if (data is Stream)
      {
        Stream stream = data as Stream;
        stream.Position = 0;
        StreamReader reader = new StreamReader(stream);
        dataBox.Text = reader.ReadToEnd();
      }
      else
        throw new Exception("Unexpected data type - " + data.GetType().ToString());
    }

    private void InitializeComponent()
    {
      this.dataBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // dataBox
      // 
      this.dataBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataBox.Location = new System.Drawing.Point(0, 0);
      this.dataBox.Multiline = true;
      this.dataBox.Name = "dataBox";
      this.dataBox.ReadOnly = true;
      this.dataBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.dataBox.Size = new System.Drawing.Size(553, 385);
      this.dataBox.TabIndex = 0;
      // 
      // TextRenderer
      // 
      this.Controls.Add(this.dataBox);
      this.Name = "TextRenderer";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    public override void CopyToClipboard()
    {
      Clipboard.SetText(dataBox.Text);
    }
  }
}
