using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ZenDesk.Helpers
{
    public static class IconHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;
        public const uint SHGFI_USEFILEATTRIBUTES = 0x10;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, out SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public static ImageSource GetIcon(string path, bool largeIcon = true)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | (largeIcon ? SHGFI_LARGEICON : SHGFI_SMALLICON);
            
            // To get icons for files that might just be extensions without needing the real file to exist
            // flags |= SHGFI_USEFILEATTRIBUTES; 

            IntPtr res = SHGetFileInfo(path, 0, out shinfo, (uint)Marshal.SizeOf(shinfo), flags);
            if (res == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
            {
                return null;
            }

            try
            {
                ImageSource imgScale = Imaging.CreateBitmapSourceFromHIcon(
                    shinfo.hIcon,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                
                imgScale.Freeze(); // Freezing is necessary for cross-thread operations and performance
                return imgScale;
            }
            finally
            {
                DestroyIcon(shinfo.hIcon);
            }
        }
    }
}
