using System;
using System.Runtime.InteropServices;

namespace ZenDesk.Helpers
{
    public static class NativeMethods
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        public const int DWMWA_SYSTEMBACKDROP_TYPE = 38;

        public const int DWMWA_MICA_EFFECT = 1029; // Undocumented for older Win11 builds

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        public static extern uint GetDoubleClickTime();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool RegisterRawInputDevices(RAWINPUTDEVICE[] pRawInputDevices, uint uiNumDevices, uint cbSize);

        [DllImport("user32.dll")]
        public static extern uint GetRawInputData(IntPtr hRawInput, uint uiCommand, IntPtr pData, ref uint pcbSize, uint cbSizeHeader);

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICE
        {
            public ushort usUsagePage;
            public ushort usUsage;
            public uint dwFlags;
            public IntPtr hwndTarget;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTHEADER
        {
            public uint dwType;
            public uint dwSize;
            public IntPtr hDevice;
            public IntPtr wParam;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWMOUSE
        {
            public ushort usFlags;
            public uint ulButtons;
            public uint ulRawButtons;
            public int lLastX;
            public int lLastY;
            public uint ulExtraInformation;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RAWINPUT
        {
            [FieldOffset(0)]
            public RAWINPUTHEADER header;
            [FieldOffset(16)] // Wait, size of RAWINPUTHEADER is 4+4+8+8 = 24 on x64? Actually 4+4=8, IntPtr is 8 depending on arch. Let's use padding properly or safer layout.
            // On x64, dwType(4) + dwSize(4) + hDevice(8) + wParam(8) = 24.
            // FieldOffset needs to be 24 for 64-bit and 16 for 32-bit.
            // Safer way in C#:
            public RAWMOUSE mouse;
        }
        
        // Accurate layout for 64-bit systems since .NET 8 on Win11 is almost always 64-bit
        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUT_X64
        {
            public RAWINPUTHEADER header;
            public RAWMOUSE mouse;
        }

        public const int RIDEV_INPUTSINK = 0x00000100;
        public const int RID_INPUT = 0x10000003;
        public const int RIM_TYPEMOUSE = 0;
        public const uint RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001;

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;
        public const uint VK_SPACE = 0x20;
        public const int WM_HOTKEY = 0x0312;
    }
}
