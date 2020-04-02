using Dev2Be.Toolkit;
using Microsoft.Win32;
using SDLL.Configuration;
using System;
using System.Reflection;
using System.Windows;

namespace Configurator
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string appName;
        
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string appName)
        {
            this.appName = appName;

            InitializeComponent();

            DataContext = this;

            Title = "Configuration - " + appName;

            WelcomeWizardPage.Title = "Bienvenue dans l'assistant de configuration du " + appName;
            WelcomeWizardPage.Description = "Cet assistant va vous guider tout au long de la procédure de configuration du " + appName + "." + Environment.NewLine + Environment.NewLine + Environment.NewLine + "Cliquez sur Suivant pour commencer la configuration.";
        }

        private void Wizard_Finish(object sender, Xceed.Wpf.Toolkit.Core.CancelRoutedEventArgs e)
        {
            AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);

            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\" + assemblyInformation.Company + "\\" + appName);

            registryKey.SetValue("FirstLaunch", false);

            if((bool)OfflineRadioButton.IsChecked)
                registryKey.SetValue("ConnexionMode", "Offline");
            else
            {
                registryKey.SetValue("ConnexionMode", "Online");

                new BDDConfiguration() { BDDName = DBNameTextBox.Text, IPAddress = IPAdressDBTextBox.Text, Password = PasswordTextBox.Text, User = UsernameTextBox.Text }.Save();
                new DomainServerConfiguration() { DomainName = DomainServerNameTextBox.Text, IPAddress = IPAdressDomainServerTextBox.Text }.Sauvegarder();
            }
        }
    }
}
