using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceExplorer
{
  class ResourceExplorerException : Exception
  {
    public ResourceExplorerException(string message)
      : base(message)
    {
    }
  }
}
