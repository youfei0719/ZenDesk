using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ZenDesk.Helpers
{
    public static class DwmHelper
    {
        public enum BackdropType
        {
            None = 1,
            MainWindow = 2, // Mica
            TransientWindow = 3, // Acrylic
            TabbedWindow = 4 // Mica Alt
        }

        public static void EnableBackdrop(Window window, BackdropType type, bool isDarkMode = true)
        {
            window.SourceInitialized += (s, e) =>
            {
                var interop = new WindowInteropHelper(window);
                var hwnd = interop.Handle;

                // Window background must be transparent for Mica/Acrylic to show
                window.Background = Brushes.Transparent;
                
                // Allow transparency explicitly
                var source = HwndSource.FromHwnd(hwnd);
                if (source != null)
                {
                    source.CompositionTarget.BackgroundColor = Colors.Transparent;
                }

                // Enable dark mode
                int trueValue = isDarkMode ? 1 : 0;
                NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_USE_IMMERSIVE_DARK_MODE, ref trueValue, Marshal.SizeOf(typeof(int)));

                // Set backdrop
                int backdropValue = (int)type;
                NativeMethods.DwmSetWindowAttribute(hwnd, NativeMethods.DWMWA_SYSTEMBACKDROP_TYPE, ref backdropValue, Marshal.SizeOf(typeof(int)));
            };
        }
    }
}
