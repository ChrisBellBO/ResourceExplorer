/* Written by Igor Gribanov 
 * Last Updated: 16 February 2004
 * 
 * This file is provided "as is" with no expressed or implied warranty.
 * The author accepts no liability for any damage/loss of business that
 * this product may cause.
 * 
 * dotnet@bk.ru
 */

using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using Microsoft.Win32;
using System.ComponentModel.Design;

namespace ResourceExplorer
{
	/// <summary>
	/// Component class that allows saving and restoring position and size of 
	/// any System.Windows.Froms.Control, contained on a Form or UserControl.
	/// It does not save/restore Form position in minimized state.
	/// For ListView controls it can save/restore widths of colums.
	/// This is based on code available at http://www.codeproject.com/cs/miscctrl/RealPosition.asp
	/// </summary>
	[Designer(typeof(RealPositionDesigner))]	
	[ToolboxItem(true)]
	[ToolboxBitmap(typeof(RealPosition))]
	[ProvideProperty("RestoreLocation", typeof(Control))]
	[ProvideProperty("RestoreColumnsWidth", typeof(ListView))]
	public sealed class RealPosition : Component, IExtenderProvider
	{
		private System.ComponentModel.Container components = null;
		private string m_RegPath;
		private string m_subRegPath;
		private Hashtable m_properties;
		private Control m_parent = null;
		private Form m_parentForm = null;

		#region RealPosition Constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="RealPosition"/> class.
    /// </summary>
		public RealPosition()
		{
			m_RegPath = @"Software\RestoreState\RealPosition"; // default key to store data
			m_properties = new Hashtable();
		}
		
    /// <summary>
    /// Initializes a new instance of the <see cref="RealPosition"/> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
		public RealPosition(IContainer parent) : this()
		{
			parent.Add(this);
			InitializeComponent();
		}
		#endregion

		#region Indexer
    /// <summary>
    /// Indexer is provided to simplify implementation of properties.
    /// </summary>
		public Properties this[object o]
		{
			get
			{
				return EnsureExists(o);
			}
			set
			{
				Properties p = EnsureExists(o);
				p = value;
			}
		}
		private Properties EnsureExists(object key)
		{
			Properties p = (Properties)m_properties[key];
			if(p == null)
			{
				p = new Properties(key);
				m_properties[key] = p;
			}
			return p;
		}
		#endregion

		#region Properties
		
    /// <summary>
    /// Registry path to store controls' positions.
    /// </summary>
		[Category("RealPosition")]
		[DefaultValue(@"Software\RestoreState\RealPosition")]
		[Description(@"Registry path to store controls' positions")]
		public string RegistryPath
		{
			get {return m_RegPath;}
			set {m_RegPath = value;}
		}

    /// <summary>
    /// Gets or sets the parent control.
    /// </summary>
		[Browsable(false)]
		public Control Parent
		{
			get
			{
				return m_parent;
			}
			set
			{
				if(m_parent == value) return;

				// unload current handlers
				if(!this.DesignMode)
				{
					Form f = m_parentForm;
					if(f != null)
					{
						f.Closed -= new EventHandler(OnClosed);
						f.Load -= new EventHandler(OnLoad);
					}
				}

				m_parent = value;

				// attach handlers
				if(!this.DesignMode)
				{
					if(m_parent is Form) 
					{
						m_parentForm = (Form)m_parent;
						m_parentForm.Load += new EventHandler(OnLoad);
					}
					else if(m_parent is UserControl)
					{
						UserControl u = (UserControl)m_parent;
						u.Load += new EventHandler(OnLoad);
					}
				}
			}
		}
		#endregion

		#region Extended Properties

    /// <summary>
    /// Save and restore Control's position and size.
    /// </summary>
		[Description("Save and restore Control's position and size")]
		[Category("RealPosition")]
		public bool GetRestoreLocation(Control c)
		{
			return this[c].RestoreLocation;
		}

    /// <summary>
    /// Save and restore Control's position and size.
    /// </summary>
		public void SetRestoreLocation(Control c, bool val)
		{
			this[c].RestoreLocation = val;
		}
		private bool ShouldSerializeRestoreLocation(Control c)
		{
			if(this[c].RestoreLocation == true) 
				return true;
			else 
				return false;
		}
		private void ResetRestoreLocation(Control c)
		{
			// by default Restore the location of the main Form 
			// by default do not restore location of UserControl
			this[c].RestoreLocation = (c is Form); 
		}

    /// <summary>
    /// Save and restore ListView's column widths.
    /// </summary>
		[Description("Save and restore ListView's column widths")]
		[Category("RealPosition")]
		public bool GetRestoreColumnsWidth(Control c)
		{
			return this[c].RestoreColumnsWidth;
		}

    /// <summary>
    /// Save and restore ListView's column widths.
    /// </summary>
		public void SetRestoreColumnsWidth(Control c, bool val)
		{
			this[c].RestoreColumnsWidth = val;
		}
		private bool ShouldSerializeRestoreColumnsWidth(Control c)
		{
			return this[c].RestoreColumnsWidth;
		}

		#endregion

		#region Event Handlers

		private void OnClosed(object sender, EventArgs e)
		{
			foreach(DictionaryEntry property in m_properties)
			{
				Properties p = (Properties)property.Value;
				string regpath = m_RegPath + m_subRegPath + p.m_subRegPath;
				RegistryKey key = Registry.CurrentUser.CreateSubKey(regpath);
				if(p.RestoreLocation == true) // Save only if RestoreLocation is True
				{
					if(p.m_parent is Form)
					{
						FormWindowState windowState = ((Form)p.m_parent).WindowState;
						key.SetValue("WindowState", (int)windowState);
					}
					SaveControlLocation(key,p);
				}
				ListView lv = p.m_parent as ListView;
				string regpath_headers = regpath + @"\ColumnHeaders";
				if(p.RestoreColumnsWidth == true && lv != null)
				{
					// save ColumnHeaders
					RegistryKey key_headers = Registry.CurrentUser.CreateSubKey(regpath_headers);
					foreach(ColumnHeader ch in lv.Columns)
					{
						key_headers.SetValue(lv.Columns.IndexOf(ch).ToString(),ch.Width);
					}
				}
			}
		}

		private void SaveControlLocation(RegistryKey key, Properties p)
		{
			if(p.m_wasMoved) // save Position
			{
				key.SetValue("Left",p.m_normalLeft);
				key.SetValue("Top", p.m_normalTop);
			}
			if(p.m_wasResized) // save Size
			{
				key.SetValue("Width", p.m_normalWidth);
				key.SetValue("Height", p.m_normalHeight);
			}
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
			// attach OnClosed event handler
			// Find the parent Form 
			Control parent = m_parent;
			m_subRegPath = @"\";
			while(!(parent is Form) && parent != null)
			{
				m_subRegPath = @"\" + parent.Name + m_subRegPath;
				parent = parent.Parent;
			} 
			m_parentForm = parent as Form;
			// add handlers if we have found a parent Form
			if(m_parentForm != null)
			{
				m_parentForm.Closed += new EventHandler(OnClosed);
			}
			else
			{
				// do not restore position if no parent form is found
				return;
			}

			// restore state/position
			m_parent.SuspendLayout();
			foreach(DictionaryEntry property in m_properties)
			{
				Properties p = (Properties)property.Value;
				string regpath = m_RegPath + m_subRegPath + p.m_subRegPath;
				if(p.RestoreLocation == true) // Restore only if RestoreLocation is True
				{
					RegistryKey key = Registry.CurrentUser.OpenSubKey(regpath);

					if(key != null)
					{
						LoadControlLocationAndState(key,p);
					}
				}
				if(p.RestoreColumnsWidth == true)
				{
					string regpath_headers = regpath + @"\ColumnHeaders";
					RegistryKey key_headers = Registry.CurrentUser.OpenSubKey(regpath_headers);
					LoadListViewColumns(key_headers, p);
				}
			}
			m_parent.ResumeLayout();
		}

		/// <summary>
		/// Loads widths of the ListView columns from the registry
		/// </summary>
		/// <param name="key">Registry key where the widths of the ListView columns are stored</param>
		/// <param name="p">Instance of Properties class which a valid reference to ListView being restored</param>
		private void LoadListViewColumns(RegistryKey key, Properties p)
		{
			ListView lv = p.m_parent as ListView;
			if(lv != null && key != null)
			{
				foreach(ColumnHeader ch in lv.Columns)
				{
					object width = key.GetValue(lv.Columns.IndexOf(ch).ToString());
					if(width != null) ch.Width = (int)width;
				}
			}
		}

		/// <summary>
		/// Loads size/position from the registry and sets Control.Location and Control.Size
		/// </summary>
		/// <param name="key">Registry key where the size/position of the control is stored</param>
		/// <param name="p">Instance of Properties class which a valid reference to control being restored</param>
		private void LoadControlLocationAndState(RegistryKey key, Properties p)
		{
			object left = key.GetValue("Left");
			object top = key.GetValue("Top");
			object width = key.GetValue("Width");
			object height = key.GetValue("Height");
			object windowState = key.GetValue("WindowState");

			// restore location
			if(left != null && top != null) 
			{
				Point location = new Point((int)left,(int)top);
				// Verify that the location falls inside one of the available screens,
				// otherwise don't restore it
				foreach (Screen screen in Screen.AllScreens)
				{
					if (screen.Bounds.Contains(location))
					{
						p.m_parent.Location = location;
						break;
					}
				}
			}

			// restore size
			if(width != null && height != null) 
			{
				p.m_parent.Size = new Size((int)width,(int)height);
			}

			// restore window state for the Form
			Form f = p.m_parent as Form;
			if(f != null && windowState != null)
			{
				f.WindowState = (FormWindowState)windowState;
				// do not allow minimized restore
				if(f.WindowState == FormWindowState.Minimized)
					f.WindowState = FormWindowState.Normal;
			}
		}
		#endregion

		bool IExtenderProvider.CanExtend(object extendee)
		{
			if(extendee is Control)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Main purpose: represent Control's location 
		/// Additional purpose: store values of extended properties
		/// note: extended properties are: RestoreLocation and RestoreColumnsWidth
		/// </summary>
		public class Properties
		{
			internal bool RestoreLocation;	// "Extended" property
			internal bool RestoreColumnsWidth;// "Extended" property

			// variables to save to Registry
			internal int m_normalLeft;
			internal int m_normalTop;
			internal int m_normalWidth;
			internal int m_normalHeight;

			// helper variables
			internal bool m_wasResized = false;
			internal bool m_wasMoved = false;
			internal bool m_isForm;

			internal Control m_parent;
			internal string m_subRegPath;

			internal Properties(object o)
			{
				RestoreLocation = (o is Form); // default true for Form
				RestoreColumnsWidth = false;	// default is false
				m_parent = (Control)o;		// type cast should not result to null
				m_isForm = (o is Form);
				// if o is Form then m_subRegPath is empty string
				m_subRegPath = (o is Form) ? String.Empty : m_parent.Name;
				
				// attach to Resize and Move
				m_parent.Resize += new System.EventHandler(OnResize);
				m_parent.Move += new System.EventHandler(OnMove);
			}
			#region Event Handlers
			private void OnResize(object sender, System.EventArgs e)
			{
				// when the control is resized we will save the new size 
				// in m_normalWidth and m_normalheight variables
				// unless the control is a Form, which is currently Maximized or Minimized
				if(!m_isForm || ((Form)m_parent).WindowState == FormWindowState.Normal)
				{
					m_wasResized = true;
					m_normalWidth = m_parent.Width;
					m_normalHeight = m_parent.Height;
				}
			}

			private void OnMove(object sender, System.EventArgs e)
			{
				// when the control is moved we will save the new position 
				// in m_normalLeft and m_normalTop variables
				// unless the control is a Form, which is currently Maximized or Minimized
				if(!m_isForm || ((Form)m_parent).WindowState == FormWindowState.Normal)
				{
					m_wasMoved = true;
					m_normalLeft = m_parent.Left;
					m_normalTop = m_parent.Top;
				}
			}

		#endregion
		}
	
		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}

	/// <summary>
	///   Designer object used to set the Parent property. 
	/// </summary>
	internal class RealPositionDesigner : ComponentDesigner 
	{
		///   <summary>
		///   Sets the Parent property to "this" - 
		///   the Form/UserControl where the component is being dropped. 
		///   </summary>
    [Obsolete]
		public override void OnSetComponentDefaults()
		{
			RealPosition rp = (RealPosition)Component;
			rp.Parent = (Control)Component.Site.Container.Components[0];
		}
	}	
}
