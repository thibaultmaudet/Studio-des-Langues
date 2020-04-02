using SDLP.Helpers;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour OpenDocumentWindow.xaml
    /// </summary>
    public partial class OpenDocumentWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;

        const uint SC_CLOSE = 0xF060;

        const int WM_SHOWWINDOW = 0x00000018;
        const int WM_CLOSE = 0x10;

        public string FileName { get; set; }

        public OpenDocumentWindow(string fileName)
        {
            InitializeComponent();

            FileName = fileName;

            DataContext = this;

            Cursor = Cursors.Wait;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IconHelper.RemoveIcon(this);

            /*if (PresentationSource.FromVisual(this) is HwndSource hwndSource)
                hwndSource.AddHook(new HwndSourceHook(HwndSourceHook));*/
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SHOWWINDOW)
            {
                IntPtr hMenu = GetSystemMenu(hwnd, false);

                if (hMenu != IntPtr.Zero)
                    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
            }
            else if (msg == WM_CLOSE)
                handled = true;

            return IntPtr.Zero;
        }
    }
}
