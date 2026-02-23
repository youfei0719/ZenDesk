using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using ZenDesk.Helpers;

namespace ZenDesk.Services
{
    public class FocusModeService : IDisposable
    {
        private readonly ContainerManager _containerManager;
        private HwndSource _source;
        private bool _isHidden = false;
        
        private long _lastClickTime = 0;
        private readonly uint _doubleClickTime;

        public FocusModeService(Window mainWindow, ContainerManager containerManager)
        {
            _containerManager = containerManager;
            _doubleClickTime = NativeMethods.GetDoubleClickTime();

            var interop = new WindowInteropHelper(mainWindow);
            var hwnd = interop.EnsureHandle();

            _source = HwndSource.FromHwnd(hwnd);
            _source.AddHook(HwndHook);

            RegisterRawInput(hwnd);
        }

        private void RegisterRawInput(IntPtr hwnd)
        {
            var rid = new NativeMethods.RAWINPUTDEVICE[1];
            rid[0].usUsagePage = 0x01; // Generic Desktop Controls
            rid[0].usUsage = 0x02;     // Mouse
            rid[0].dwFlags = NativeMethods.RIDEV_INPUTSINK;
            rid[0].hwndTarget = hwnd;

            if (!NativeMethods.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)Marshal.SizeOf(typeof(NativeMethods.RAWINPUTDEVICE))))
            {
                System.Diagnostics.Debug.WriteLine("Failed to register raw input device.");
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.RID_INPUT)
            {
                ProcessRawInput(lParam);
            }
            return IntPtr.Zero;
        }

        private void ProcessRawInput(IntPtr lParam)
        {
            uint dwSize = 0;
            NativeMethods.GetRawInputData(lParam, NativeMethods.RID_INPUT, IntPtr.Zero, ref dwSize, (uint)Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER)));

            IntPtr pData = Marshal.AllocHGlobal((int)dwSize);
            try
            {
                if (NativeMethods.GetRawInputData(lParam, NativeMethods.RID_INPUT, pData, ref dwSize, (uint)Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER))) == dwSize)
                {
                    // Since .NET 8 on Windows 11 is x64 mostly, cast it safely
                    var raw = (NativeMethods.RAWINPUT_X64)Marshal.PtrToStructure(pData, typeof(NativeMethods.RAWINPUT_X64))!;

                    if (raw.header.dwType == NativeMethods.RIM_TYPEMOUSE)
                    {
                        if ((raw.mouse.usFlags & NativeMethods.RI_MOUSE_LEFT_BUTTON_DOWN) == NativeMethods.RI_MOUSE_LEFT_BUTTON_DOWN ||
                            (raw.mouse.ulButtons & NativeMethods.RI_MOUSE_LEFT_BUTTON_DOWN) == NativeMethods.RI_MOUSE_LEFT_BUTTON_DOWN)
                        {
                            IntPtr foregroundWindow = NativeMethods.GetForegroundWindow();
                            
                            StringBuilder className = new StringBuilder(256);
                            NativeMethods.GetClassName(foregroundWindow, className, className.Capacity);

                            string clsName = className.ToString();
                            if (clsName == "Progman" || clsName == "WorkerW")
                            {
                                long currentTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                                if (currentTime - _lastClickTime <= _doubleClickTime)
                                {
                                    // Double click detected on desktop blank space!
                                    ToggleFocusMode();
                                    _lastClickTime = 0; // Reset
                                }
                                else
                                {
                                    _lastClickTime = currentTime;
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pData);
            }
        }

        private void ToggleFocusMode()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _isHidden = !_isHidden;

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is Views.ContainerWindow cw)
                    {
                        if (_isHidden)
                        {
                            AnimationHelper.FadeOut(cw, 250, () => cw.Visibility = Visibility.Hidden);
                        }
                        else
                        {
                            cw.Visibility = Visibility.Visible;
                            AnimationHelper.FadeIn(cw, 250);
                        }
                    }
                }

                // In a real product, we would also hide Windows Desktop Icons by sending a message to the WorkerW window.
                // For MVP, hiding ZenDesk's own containers is the core "Zen" experience.
            });
        }

        public void Dispose()
        {
            if (_source != null)
            {
                _source.RemoveHook(HwndHook);
                _source.Dispose();
                _source = null;
            }
        }
    }
}
