using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CabineParty.Core.Windows
{
    public class WindowsManager
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        private readonly IntPtr _windowsHandle;

        public WindowsManager(Process process)
        {
            _windowsHandle = process.MainWindowHandle;
        }

        public void Show()
        {
            ShowWindow(_windowsHandle, 3);
        }

        public void Hide()
        {
            ShowWindow(_windowsHandle, 0);
        }
    }
}
