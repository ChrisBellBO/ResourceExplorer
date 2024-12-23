using System;
using System.Windows.Forms;

namespace ResourceExplorer
{
	/// <summary>
	/// Cursor handling class.
	/// </summary>
	public sealed class CursorHandler : IDisposable
	{
    Cursor savedCursor;
    /// <summary>
    /// Creates a new <see cref="CursorHandler"/> instance.
    /// </summary>
		public CursorHandler() 
		{
			savedCursor = Cursor.Current;
      Cursor.Current = Cursors.AppStarting;
		}

    /// <summary>
    /// Class finalizer.
    /// </summary>
    ~CursorHandler()
    {
      Restore();
    }

    /// <summary>
    /// Disposes this instance, IDisposable implementation.
    /// </summary>
    public void Dispose()
    {
      Restore();
      GC.SuppressFinalize(this);
    }

    private void Restore()
    {
      Cursor.Current = savedCursor;
    }
	}
}
