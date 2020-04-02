using System.Windows;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected async override void OnStartup(StartupEventArgs e)
        {
            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            base.OnStartup(e);

            await splashScreen.CheckConfig();

            MainWindow window = new MainWindow();
            window.Show();

            splashScreen.Close();

            window.Focus();
        }
    }
}
