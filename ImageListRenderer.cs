using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ResourceExplorer
{
  class ImageListRenderer : Renderer
  {
    private System.Windows.Forms.PictureBox pictureBox;
  
    public ImageListRenderer()
    {
      InitializeComponent();
    }

    private void InitializeComponent()
    {
      this.pictureBox = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox.Location = new System.Drawing.Point(0, 0);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(553, 385);
      this.pictureBox.TabIndex = 1;
      this.pictureBox.TabStop = false;
      // 
      // ImageListRenderer
      // 
      this.Controls.Add(this.pictureBox);
      this.Name = "ImageListRenderer";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    public override void LoadData(string resourceName, object data)
    {
      ImageListStreamer streamer = (ImageListStreamer)data;
      ImageList list = new ImageList();
      list.ImageStream = streamer;
      Bitmap fullBitmap = new Bitmap(list.ImageSize.Width * list.Images.Count, list.ImageSize.Height);
      Graphics graphics = Graphics.FromImage(fullBitmap);

      for (int i = 0; i < list.Images.Count; i++)
      {
        Image img = list.Images[i];
        graphics.DrawImage(img, new Point(list.ImageSize.Width * i, 0));
      }

      pictureBox.Image = fullBitmap;
    }

    public override void CopyToClipboard()
    {
      Clipboard.SetImage(pictureBox.Image);
    }

    public override void SaveToFile(string fileName)
    {
      pictureBox.Image.Save(fileName);
    }

    public override string GetFileName(string resourceName)
    {
      return resourceName + ".bmp";
    }
  }
}
