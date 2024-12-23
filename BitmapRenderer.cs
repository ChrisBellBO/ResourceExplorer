using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ResourceExplorer
{
  class BitmapRenderer : Renderer
  {
    private System.Windows.Forms.PictureBox pictureBox;

    public BitmapRenderer()
    {
      InitializeComponent();
    }

    public override void LoadData(string resourceName, object data)
    {
      if (data is Icon)
        pictureBox.Image = (data as Icon).ToBitmap();
      else if (data is Bitmap)
        pictureBox.Image = data as Bitmap;
      else if (data is Stream)
      {
        Stream stream = data as Stream;
        stream.Position = 0;
        string lowercaseName = resourceName.ToLower();
        if (lowercaseName.EndsWith(".cur"))
        {
          System.Windows.Forms.Cursor cursor = new System.Windows.Forms.Cursor(stream);
          Bitmap bitmap = new Bitmap(cursor.Size.Width, cursor.Size.Height);
          using (Graphics gBmp = Graphics.FromImage(bitmap))
          {
            cursor.Draw(gBmp, new Rectangle(0, 0, cursor.Size.Width, cursor.Size.Height));
            pictureBox.Image = bitmap;
          }
        }
        else
          pictureBox.Image = Image.FromStream(stream);
      }
      else throw new ResourceExplorerException("Can not load data");
    }

    private void InitializeComponent()
    {
      this.pictureBox = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox
      // 
      this.pictureBox.ContextMenuStrip = this.contextMenuStrip;
      this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.pictureBox.Location = new System.Drawing.Point(0, 0);
      this.pictureBox.Name = "pictureBox";
      this.pictureBox.Size = new System.Drawing.Size(553, 385);
      this.pictureBox.TabIndex = 0;
      this.pictureBox.TabStop = false;
      // 
      // BitmapRenderer
      // 
      this.Controls.Add(this.pictureBox);
      this.Name = "BitmapRenderer";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
      this.ResumeLayout(false);

    }

    // our BITMAPINFOHEADER struct, as per gdi
    // use LayoutKind to make sure data is marshalled as we've laid it out
    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
      public uint biSize;
      public int biWidth;
      public int biHeight;
      public ushort biPlanes;
      public ushort biBitCount;
      public uint biCompression;
      public uint biSizeImage;
      public int biXPelsPerMeter;
      public int biYPelsPerMeter;
      public uint biClrUsed;
      public uint biClrImportant;
    }

    private static Bitmap BitmapFromDIB(MemoryStream dib)
    {
      // get byte array of device independent bitmap
      byte[] dibBytes = dib.ToArray();

      // get the handle for the byte array and "pin" that memory (i.e. prevent 
      // garbage collector from gobbling it up right away)...
      GCHandle hdl = GCHandle.Alloc(dibBytes, GCHandleType.Pinned);

      // marshal our data into a BITMAPINFOHEADER struct per Win32
      // definition of BITMAPINFOHEADER
      BITMAPINFOHEADER dibHdr = (BITMAPINFOHEADER)Marshal.PtrToStructure
                   (hdl.AddrOfPinnedObject(), typeof(BITMAPINFOHEADER));
      bool is555 = true;

      Bitmap bmp = null;

      if (dibHdr.biBitCount == 8)
      {
        // set our pointer to end of BITMAPINFOHEADER
        Int64 jumpTo = hdl.AddrOfPinnedObject().ToInt64() + dibHdr.biSize;
        bmp = new Bitmap(dibHdr.biWidth, dibHdr.biHeight, PixelFormat.Format8bppIndexed);
        bmp.SetResolution((100f * (float)dibHdr.biXPelsPerMeter) / 2.54f,
                 (100f * (float)dibHdr.biYPelsPerMeter) / 2.54f);

        // set the colors in our palette
        ColorPalette palette = bmp.Palette;
        IntPtr ptr = IntPtr.Zero;
        int colors = (int)(dibBytes.Length - (bmp.Width * bmp.Height) - dibHdr.biSize);
        for (int i = 0; i < 256; i++)
        {
          ptr = new IntPtr(jumpTo);
          uint bmiColor = (uint)Marshal.ReadInt32(ptr);
          int r = (int)((bmiColor & 0xFF0000) >> 16),
              g = (int)((bmiColor & 0xFF00) >> 8),
              b = (int)((bmiColor & 0xFF));
          palette.Entries[i] = Color.FromArgb(r, g, b);
          jumpTo += 4;
        }
        bmp.Palette = palette;

        // now write the remaining bmp data to our bitmap
        BitmapData _8bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
        jumpTo -= hdl.AddrOfPinnedObject().ToInt64();
        Marshal.Copy(dibBytes, (int)jumpTo, _8bd.Scan0, _8bd.Stride * _8bd.Height);
        bmp.UnlockBits(_8bd);
      }
      else if ((dibHdr.biBitCount == 16) && (dibHdr.biCompression == 3))
      {
        Int64 jumpTo = (Int64)(dibHdr.biClrUsed * (uint)4 + dibHdr.biSize);
        IntPtr ptr = new IntPtr(hdl.AddrOfPinnedObject().ToInt64() + jumpTo);
        ushort redMask = (ushort)Marshal.ReadInt16(ptr);
        ptr = new IntPtr(ptr.ToInt64() + (2 * Marshal.SizeOf(typeof(UInt16))));
        ushort greenMask = (ushort)Marshal.ReadInt16(ptr);
        ptr = new IntPtr(ptr.ToInt64() + (2 * Marshal.SizeOf(typeof(UInt16))));
        ushort blueMask = (ushort)Marshal.ReadInt16(ptr);

        is555 = ((redMask == 0x7C00) && (greenMask == 0x03E0) && (blueMask == 0x001F));
      }

      // go ahead and release the "pin" from our handle on that memory
      hdl.Free();

      // If the target device does not have one plane, or we're working with a bitmap other 
      // than a non-compressed (BI_RGB) bitmap, we're not gonna work woith it
      if (dibHdr.biPlanes != 1 || (dibHdr.biCompression != 0 && dibHdr.biCompression != 3))
        return null;

      if (bmp == null)
      {
        // we need to know beforehand the pixel-depth of our bitmap
        PixelFormat fmt = PixelFormat.Format24bppRgb;

        switch (dibHdr.biBitCount)
        {
          case 32:
            fmt = PixelFormat.Format32bppRgb;
            break;
          case 24:
            fmt = PixelFormat.Format24bppRgb;
            break;
          case 16:
            fmt = (is555) ? PixelFormat.Format16bppRgb555 :
                        PixelFormat.Format16bppRgb565;
            break;
          default:
            return null;
        }

        // prepare for our output bitmap
        bmp = new Bitmap(dibHdr.biWidth, dibHdr.biHeight, fmt);

        // load our "empty" bitmap into memory and lock it for
        // writing in the format we specified
        BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly, fmt);

        // marshal our device independent bitmap data over to our output bitmap
        Marshal.Copy(dibBytes, Marshal.SizeOf(dibHdr), bd.Scan0, bd.Stride * bd.Height);

        // we're done marshalling, so release our bitmapdata lock
        bmp.UnlockBits(bd);

      }

      if (dibHdr.biHeight > 0)
      {
        // DIB data is upside-down for some reason, so flip it
        bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
      }

      // return our bitmap
      return bmp;
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
