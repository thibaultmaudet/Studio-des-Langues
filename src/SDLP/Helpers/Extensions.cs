using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SDLP.Helpers.Extensions
{
    public static class Extensions
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        public static async Task WaitForExitAsync(this Process process, CancellationToken cancellationToken = default(CancellationToken))
        {
            var tcs = new TaskCompletionSource<bool>();

            void Process_Exited(object sender, EventArgs e)
            {
                tcs.TrySetResult(true);
            }

            process.EnableRaisingEvents = true;
            process.Exited += Process_Exited;

            try
            {
                if (process.HasExited)
                {
                    return;
                }

                using (cancellationToken.Register(() => tcs.TrySetCanceled()))
                {
                    await tcs.Task;
                }
            }
            finally
            {
                process.Exited -= Process_Exited;
            }
        }

        public static ImageSource ToImageSource(this Icon icon) => Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    }
}
