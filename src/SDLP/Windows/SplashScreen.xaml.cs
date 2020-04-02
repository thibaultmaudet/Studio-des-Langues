using Dev2Be.Toolkit;
using Meziantou.Framework.Win32;
using Microsoft.Win32;
using SDLP.Helpers.Extensions;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window
    {
        private Process process;

        public SplashScreen()
        {
            InitializeComponent();

            Title = "Ouverture - " + new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name).Product;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        public async Task CheckConfig()
        {
            AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\" + assemblyInformation.Company + "\\" + assemblyInformation.Product);

            registryKey.CreateSubKey("Account");

            if (registryKey.OpenSubKey("Account").GetValue("Username") != null)
            {
                Credential credential = CredentialManager.ReadCredential("SDLPAccount:user=" + registryKey.OpenSubKey("Account").GetValue("Username").ToString());

                if (credential == null)
                {
                    registryKey.OpenSubKey("Account", true).DeleteValue("Username");
                    registryKey.OpenSubKey("Account", true).SetValue("IsLogged", false);
                }
            }

            bool configuratorLauch = false;

            while ((string)registryKey.GetValue("FirstLaunch") == "True" || registryKey.GetValue("FirstLaunch") == null)
            {
                if ((string)registryKey.GetValue("ConnexionMode") == null)
                {
                    if (configuratorLauch)
                    {
                        Application.Current.Shutdown(0);
                        return;
                    }

                    process = new Process();
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                    process.StartInfo.FileName = "Configurator.exe";
                    process.StartInfo.Arguments = "--appName \"" + assemblyInformation.Product + "\"";
                    process.Start();

                    configuratorLauch = true;

                    await process.WaitForExitAsync();
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown(0);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (process != null)
                if(!process.HasExited)
                    process.Kill();
        }
    }
}
