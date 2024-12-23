using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Collections;

namespace ResourceExplorer
{
  public class ResourcesTreeView : TreeView
  {
    public ResourcesTreeView()
    {
      AppDomain currentDomain = AppDomain.CurrentDomain;
      currentDomain.AssemblyResolve += new ResolveEventHandler(LoadFromSameFolder);
    }

    private Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
    {
      if (currentAppDirectory == null)
        return null;

      string assemblyPath = Path.Combine(currentAppDirectory, new AssemblyName(args.Name).Name + ".dll");
      if (!File.Exists(assemblyPath)) 
        return null;
      
      Assembly assembly = Assembly.LoadFrom(assemblyPath);
      return assembly;
    }

    private TreeNode root;
    public void LoadResourcesFromFile(string fileName)
    {
      Nodes.Clear();
      root = Nodes.Add(Path.GetFileName(fileName));

      switch (Path.GetExtension(fileName).ToLower())
      {
        case ".exe":
        case ".dll":
          LoadResourcesFromAssemblyFile(fileName);
          break;

        case ".resx":
          LoadResourcesFromResxFile(fileName);
          break;

        case ".resources":
          LoadResourcesFromResourcesFile(fileName);
          break;

        default:
          throw new ResourceExplorerException("Unknown file format");
      }

      root.Expand();
    }

    private string currentAppDirectory;
    private void LoadResourcesFromAssemblyFile(string fileName)
    {
      Assembly assem = Assembly.LoadFrom(fileName);
      currentAppDirectory = Path.GetDirectoryName(fileName);

      foreach (string resourceName in assem.GetManifestResourceNames())
      {
        Stream stream = assem.GetManifestResourceStream(resourceName);
        TreeNode node = root.Nodes.Add(resourceName);
        node.Tag = stream;
        if (resourceName.ToLower().EndsWith(".resources"))
        {
          LoadResourcesFromResourcesStream(node, stream);
        }
      }
    }

    private void LoadResourcesFromResxFile(string fileName)
    {
      using (ResXResourceReader reader = new ResXResourceReader(fileName))
      {
        foreach (DictionaryEntry entry in reader)
        {
          TreeNode node = root.Nodes.Add(string.Format("{0} ({1})", entry.Key.ToString(), entry.Value.GetType()));
          node.Tag = entry.Value;
        }
      }
    }

    private void LoadResourcesFromResourcesFile(string fileName)
    {
      using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        LoadResourcesFromResourcesStream(root, stream);
      }
    }

    private void LoadResourcesFromResourcesStream(TreeNode parent, Stream stream)
    {
      using (ResourceReader reader = new ResourceReader(stream))
      {
        foreach (DictionaryEntry entry in reader)
        {
          TreeNode node = parent.Nodes.Add(string.Format("{0} ({1})", entry.Key.ToString(), (entry.Value != null ? entry.Value.GetType().ToString() : "<none>")));
          node.Tag = entry.Value;
        }
      }
    }
  }
}
