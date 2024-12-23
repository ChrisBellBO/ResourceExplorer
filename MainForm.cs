using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows.Forms; 

namespace ResourceExplorer
{
  /// <summary>
  /// Summary description for Form1.
  /// </summary>
  public class MainForm : System.Windows.Forms.Form
  {
    private System.Windows.Forms.OpenFileDialog openFileDialog;
    private SplitContainer splitContainer;
    private ResourcesTreeView resourcesTreeView;
    private MenuStrip menuStrip;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem openToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem editToolStripMenuItem;
    private ToolStripMenuItem copyToolStripMenuItem;
    private ContextMenuStrip contextMenuStrip;
    private ToolStripMenuItem copyToolStripMenuItem1;
    private ToolStripMenuItem copyImageToolStripMenuItem;
    private ToolStripMenuItem toolStripMenuItem1;
    private RealPosition realPosition1;
    private IContainer components;

    public MainForm()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      bool is64bit = IntPtr.Size == 8;
      if (is64bit)
        Text = "Resource Explorer " + Application.ProductVersion + " (64-bit)";
      else
        Text = "Resource Explorer " + Application.ProductVersion + " (32-bit)";
    }

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
      this.splitContainer = new System.Windows.Forms.SplitContainer();
      this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.menuStrip = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.copyImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
      this.resourcesTreeView = new ResourceExplorer.ResourcesTreeView();
      this.realPosition1 = new ResourceExplorer.RealPosition(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
      this.splitContainer.Panel1.SuspendLayout();
      this.splitContainer.SuspendLayout();
      this.contextMenuStrip.SuspendLayout();
      this.menuStrip.SuspendLayout();
      this.SuspendLayout();
      // 
      // openFileDialog
      // 
      this.openFileDialog.Filter = "Assemblies (*.exe, *.dll)|*.exe;*.dll|ResX files (*.resx)|*.resx|Resource files (" +
    "*.resources)|*.resource|All files (*.*)|*.*";
      // 
      // splitContainer
      // 
      this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
      this.splitContainer.Location = new System.Drawing.Point(0, 24);
      this.splitContainer.Name = "splitContainer";
      // 
      // splitContainer.Panel1
      // 
      this.splitContainer.Panel1.Controls.Add(this.resourcesTreeView);
      this.splitContainer.Size = new System.Drawing.Size(642, 302);
      this.splitContainer.SplitterDistance = 214;
      this.splitContainer.TabIndex = 1;
      // 
      // contextMenuStrip
      // 
      this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem1});
      this.contextMenuStrip.Name = "contextMenuStrip";
      this.contextMenuStrip.Size = new System.Drawing.Size(103, 26);
      // 
      // menuStrip
      // 
      this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem});
      this.menuStrip.Location = new System.Drawing.Point(0, 0);
      this.menuStrip.Name = "menuStrip";
      this.menuStrip.Size = new System.Drawing.Size(642, 24);
      this.menuStrip.TabIndex = 2;
      this.menuStrip.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.fileExitMenuItem_Click);
      // 
      // editToolStripMenuItem
      // 
      this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem,
            this.copyImageToolStripMenuItem});
      this.editToolStripMenuItem.Name = "editToolStripMenuItem";
      this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
      this.editToolStripMenuItem.Text = "Edit";
      // 
      // copyToolStripMenuItem1
      // 
      this.copyToolStripMenuItem1.Image = global::ResourceExplorer.Properties.Resources.copy;
      this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
      this.copyToolStripMenuItem1.Size = new System.Drawing.Size(102, 22);
      this.copyToolStripMenuItem1.Text = "Copy";
      this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyMenuItem_Click);
      // 
      // openToolStripMenuItem
      // 
      this.openToolStripMenuItem.Image = global::ResourceExplorer.Properties.Resources.folder;
      this.openToolStripMenuItem.Name = "openToolStripMenuItem";
      this.openToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
      this.openToolStripMenuItem.Text = "Open...";
      this.openToolStripMenuItem.Click += new System.EventHandler(this.fileOpenMenuItem_Click);
      // 
      // copyToolStripMenuItem
      // 
      this.copyToolStripMenuItem.Image = global::ResourceExplorer.Properties.Resources.copy;
      this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
      this.copyToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.copyToolStripMenuItem.Text = "Copy Name";
      this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
      // 
      // copyImageToolStripMenuItem
      // 
      this.copyImageToolStripMenuItem.Image = global::ResourceExplorer.Properties.Resources.copy;
      this.copyImageToolStripMenuItem.Name = "copyImageToolStripMenuItem";
      this.copyImageToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
      this.copyImageToolStripMenuItem.Text = "Copy Resource";
      this.copyImageToolStripMenuItem.Click += new System.EventHandler(this.copyImageToolStripMenuItem_Click);
      // 
      // toolStripMenuItem1
      // 
      this.toolStripMenuItem1.Name = "toolStripMenuItem1";
      this.toolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
      this.toolStripMenuItem1.Text = "Export Images...";
      this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
      // 
      // resourcesTreeView
      // 
      this.resourcesTreeView.ContextMenuStrip = this.contextMenuStrip;
      this.resourcesTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resourcesTreeView.Location = new System.Drawing.Point(0, 0);
      this.resourcesTreeView.Name = "resourcesTreeView";
      this.resourcesTreeView.Size = new System.Drawing.Size(214, 302);
      this.resourcesTreeView.Sorted = true;
      this.resourcesTreeView.TabIndex = 1;
      this.resourcesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.resourcesTreeView_AfterSelect);
      // 
      // realPosition1
      // 
      this.realPosition1.Parent = this;
      this.realPosition1.RegistryPath = "Software\\ResourceExplorer\\RealPosition";
      // 
      // MainForm
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(642, 326);
      this.Controls.Add(this.splitContainer);
      this.Controls.Add(this.menuStrip);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "MainForm";
      this.realPosition1.SetRestoreLocation(this, true);
      this.Text = "Resource Explorer";
      this.splitContainer.Panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
      this.splitContainer.ResumeLayout(false);
      this.contextMenuStrip.ResumeLayout(false);
      this.menuStrip.ResumeLayout(false);
      this.menuStrip.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }
    #endregion

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.Run(new MainForm());
    }

    private void fileOpenMenuItem_Click(object sender, System.EventArgs e)
    {
      if (openFileDialog.ShowDialog() == DialogResult.OK)
      {
        using (CursorHandler handler = new CursorHandler())
        {
          resourcesTreeView.LoadResourcesFromFile(openFileDialog.FileName);
        }
      }
    }

    private void fileExitMenuItem_Click(object sender, System.EventArgs e)
    {
      this.Close();
    }

    private Renderer currentRenderer;
    private void resourcesTreeView_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
    {
      TreeNode node = resourcesTreeView.SelectedNode;
      if (node != null)
      {
        object value = node.Tag;
        // get the renderer
        splitContainer.Panel2.Controls.Clear();
        currentRenderer = Renderer.GetRenderer(node.Text, value);
        splitContainer.Panel2.Controls.Add(currentRenderer);
        currentRenderer.Dock = DockStyle.Fill;
        currentRenderer.LoadData(node.Text, value);
      }
    }

    private void copyMenuItem_Click(object sender, EventArgs e)
    {
      if (resourcesTreeView.SelectedNode != null)
      {
        Clipboard.SetText(resourcesTreeView.SelectedNode.Text);
      }
    }

    private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
    {
      if (currentRenderer != null)
        currentRenderer.CopyToClipboard();
    }

    private void ExportImages(TreeNodeCollection nodes, string folder)
    {
      for (int i = 0; i < nodes.Count; i++)
      {
        TreeNode node = nodes[i];
        using (Renderer renderer = Renderer.GetRenderer(node.Text, node.Tag))
        {
          if ((renderer is BitmapRenderer) || (renderer is ImageListRenderer))
          {
            renderer.LoadData(node.Text, node.Tag);
            string fileName = renderer.GetFileName(node.Text);
            fileName = Path.Combine(folder, fileName);
            renderer.SaveToFile(fileName);
          }
          ExportImages(node.Nodes, folder);
        }
      }
    }

    private void toolStripMenuItem1_Click(object sender, EventArgs e)
    {
      using (FolderBrowserDialog dlg = new FolderBrowserDialog())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          using (CursorHandler handler = new CursorHandler())
          {
            ExportImages(resourcesTreeView.Nodes, dlg.SelectedPath);
          }
        }
      }
    }
  }
}
