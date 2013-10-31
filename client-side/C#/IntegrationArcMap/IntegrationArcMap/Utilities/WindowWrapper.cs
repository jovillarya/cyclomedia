using System;
using System.Windows.Forms;

namespace IntegrationArcMap.Utilities
{
  class WindowWrapper : IWin32Window
  {
    public IntPtr Handle { get; private set; }

    public WindowWrapper(IntPtr handle)
    {
      Handle = handle;
    }

    public WindowWrapper(int handle)
    {
      Handle = new IntPtr(handle);
    }
  }
}
