using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Configurator
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length == 0)
                Current.Shutdown(0);
            else
            {
                if (e.Args.Length == 2 && e.Args[0] == "--appName")
                {
                    string appName = e.Args[1];

                    MainWindow mainWindow = new MainWindow(appName);
                    mainWindow.ShowDialog();
                }
            }
        }
    }
}
