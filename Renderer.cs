using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace ResourceExplorer
{
  public class Renderer : UserControl
  {
    protected ContextMenuStrip contextMenuStrip;
    private System.ComponentModel.IContainer components;
    private ToolStripMenuItem copyToolStripMenuItem1;
  
    public Renderer()
    {
      InitializeComponent();
    }

    public virtual void LoadData(string resourceName, object data)
    {
      throw new NotImplementedException("Implement LoadData");
    }

    public static Renderer GetRenderer(string resourceName, object value)
    {
      if (value is Stream)
      {
        if ((value as Stream).CanRead)
        {
          // check file extension to guess what kind of resource it is
          string lowercaseName = resourceName.ToLower();
          if (lowercaseName.EndsWith(".bmp"))
            return new BitmapRenderer();
          if (lowercaseName.EndsWith(".png"))
            return new BitmapRenderer();
          if (lowercaseName.EndsWith(".ico"))
            return new BitmapRenderer();
          if (lowercaseName.EndsWith(".cur"))
            return new BitmapRenderer();
          if (lowercaseName.EndsWith(".jpg"))
            return new BitmapRenderer();
          if (lowercaseName.EndsWith(".jpeg"))
            return new BitmapRenderer();

          return new TextRenderer();
        }

        return new DefaultRenderer();
      }
      if (value is Icon)
        return new BitmapRenderer();
      if (value is Bitmap)
        return new BitmapRenderer();
      if (value is ImageListStreamer)
        return new ImageListRenderer();

      return new DefaultRenderer();
    }

    public virtual void CopyToClipboard()
    {
      throw new NotImplementedException("Implement CopyToClipboard");
    }

    public virtual void SaveToFile(string fileName)
    {
      throw new NotImplementedException("Implement SaveToFile");
    }

    public virtual string GetFileName(string resourceName)
    {
      throw new NotImplementedException("Implement GetFileName");
    }

    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.contextMenuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(153, 48);
      // 
      // copyToolStripMenuItem1
      // 
      this.copyToolStripMenuItem1.Image = global::ResourceExplorer.Properties.Resources.copy;
      this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
      this.copyToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
      this.copyToolStripMenuItem1.Text = "Copy";
      this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
      // 
      // Renderer
      // 
      this.ContextMenuStrip = this.contextMenuStrip;
      this.Name = "Renderer";
      this.Size = new System.Drawing.Size(553, 385);
      this.contextMenuStrip.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
    {
      CopyToClipboard();
    }
  }
}
