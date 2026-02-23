using System;
using System.Windows;
using System.Windows.Interop;
using ZenDesk.Helpers;

namespace ZenDesk.Services
{
    public class ZenPeekService : IDisposable
    {
        private HwndSource _source;
        private readonly int _hotKeyId = 9000;
        private bool _isPeekActive = false;

        public ZenPeekService(Window mainWindow)
        {
            var interop = new WindowInteropHelper(mainWindow);
            var hwnd = interop.EnsureHandle();

            _source = HwndSource.FromHwnd(hwnd);
            _source.AddHook(HwndHook);

            RegisterGlobalHotKey(hwnd);
        }

        private void RegisterGlobalHotKey(IntPtr hwnd)
        {
            if (!NativeMethods.RegisterHotKey(hwnd, _hotKeyId, NativeMethods.MOD_ALT, NativeMethods.VK_SPACE))
            {
                System.Diagnostics.Debug.WriteLine("Failed to register Alt+Space shortcut.");
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_HOTKEY)
            {
                if (wParam.ToInt32() == _hotKeyId)
                {
                    ToggleZenPeek();
                    handled = true;
                }
            }
            return IntPtr.Zero;
        }

        private void ToggleZenPeek()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _isPeekActive = !_isPeekActive;

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.ContainerWindow cw)
                    {
                        if (_isPeekActive)
                        {
                            // Bring to front
                            cw.Visibility = Visibility.Visible;
                            AnimationHelper.FadeIn(cw, 200);
                            cw.Topmost = true;
                            cw.Topmost = false; 
                            cw.Activate();
                        }
                        else
                        {
                            // If they were hidden by Focus Mode, maybe they shouldn't fade out here.
                            // But for simple Zen Peek, pressing it again puts them back.
                            // Let's just do a visual bounce.
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            if (_source != null)
            {
                NativeMethods.UnregisterHotKey(_source.Handle, _hotKeyId);
                _source.RemoveHook(HwndHook);
                _source.Dispose();
                _source = null;
            }
        }
    }
}
