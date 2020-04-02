using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SDLP.Helpers
{
    public static class IconHelper
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_DLGMODALFRAME = 0x0001;
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_FRAMECHANGED = 0x0020;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);

        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref FileInfo psfi, uint cbFileInfo, uint uFlags);

        public static void RemoveIcon(this Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_DLGMODALFRAME);
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_FRAMECHANGED);
        }

        private static readonly Dictionary<string, ImageSource> _smallIconCache = new Dictionary<string, ImageSource>();
        private static readonly Dictionary<string, ImageSource> _largeIconCache = new Dictionary<string, ImageSource>();
        /// <summary>
        /// Get an icon for a given filename
        /// </summary>
        /// <param name="fileName">any filename</param>
        /// <param name="large">16x16 or 32x32 icon</param>
        /// <returns>null if path is null, otherwise - an icon</returns>
        public static ImageSource FindIconForFilename(string fileName, bool large)
        {
            var extension = Path.GetExtension(fileName);
            if (extension == null)
                return null;
            var cache = large ? _largeIconCache : _smallIconCache;
            ImageSource icon;
            if (cache.TryGetValue(extension, out icon))
                return icon;
            icon = IconReader.GetFileIcon(fileName, large ? IconReader.IconSize.Large : IconReader.IconSize.Small, false).ToImageSource();
            cache.Add(extension, icon);
            return icon;
        }
        /// <summary>
        /// http://stackoverflow.com/a/6580799/1943849
        /// </summary>
        static ImageSource ToImageSource(this Icon icon) => Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

        /// <summary>
        /// Provides static methods to read system icons for both folders and files.
        /// </summary>
        /// <example>
        /// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
        /// </example>
        static class IconReader
        {
            /// <summary>
            /// Options to specify the size of icons to return.
            /// </summary>
            public enum IconSize
            {
                /// <summary>
                /// Specify large icon - 32 pixels by 32 pixels.
                /// </summary>
                Large = 0,
                /// <summary>
                /// Specify small icon - 16 pixels by 16 pixels.
                /// </summary>
                Small = 1
            }
            /// <summary>
            /// Returns an icon for a given file - indicated by the name parameter.
            /// </summary>
            /// <param name="name">Pathname for file.</param>
            /// <param name="size">Large or small</param>
            /// <param name="linkOverlay">Whether to include the link icon</param>
            /// <returns>System.Drawing.Icon</returns>
            public static Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
            {
                var shfi = new Shell32.Shfileinfo();
                var flags = Shell32.ShgfiIcon | Shell32.ShgfiUsefileattributes;
                if (linkOverlay) flags += Shell32.ShgfiLinkoverlay;
                /* Check the size specified for return. */
                if (IconSize.Small == size)
                    flags += Shell32.ShgfiSmallicon;
                else
                    flags += Shell32.ShgfiLargeicon;
                Shell32.SHGetFileInfo(name,
                    Shell32.FileAttributeNormal,
                    ref shfi,
                    (uint)Marshal.SizeOf(shfi),
                    flags);
                // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
                var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
                User32.DestroyIcon(shfi.hIcon);     // Cleanup
                return icon;
            }
        }

        static class Shell32
        {
            private const int MaxPath = 256;
            [StructLayout(LayoutKind.Sequential)]
            public struct Shfileinfo
            {
                private const int Namesize = 80;
                public readonly IntPtr hIcon;
                private readonly int iIcon;
                private readonly uint dwAttributes;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
                private readonly string szDisplayName;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Namesize)]
                private readonly string szTypeName;
            };
            public const uint ShgfiIcon = 0x000000100;     // get icon
            public const uint ShgfiLinkoverlay = 0x000008000;     // put a link overlay on icon
            public const uint ShgfiLargeicon = 0x000000000;     // get large icon
            public const uint ShgfiSmallicon = 0x000000001;     // get small icon
            public const uint ShgfiUsefileattributes = 0x000000010;     // use passed dwFileAttribute
            public const uint FileAttributeNormal = 0x00000080;
            [DllImport("Shell32.dll")]
            public static extern IntPtr SHGetFileInfo(
                string pszPath,
                uint dwFileAttributes,
                ref Shfileinfo psfi,
                uint cbFileInfo,
                uint uFlags
                );
        }

        static class User32
        {
            [DllImport("User32.dll")]
            public static extern int DestroyIcon(IntPtr hIcon);
        }
    }
}
