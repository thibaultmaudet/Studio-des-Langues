using Dev2Be.Toolkit;
using Meziantou.Framework.Win32;
using Microsoft.Win32;
using System.Reflection;
using System.Windows;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour AuthentificationWindow.xaml
    /// </summary>
    public partial class AuthentificationWindow : Fluent.RibbonWindow
    {
        public AuthentificationWindow()
        {
            InitializeComponent();
        }

        public void ShowDialog(Window owner)
        {
            Owner = owner;

            ShowDialog();
        }

        private void ConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UsernameWatermarkTextBox.Text) && string.IsNullOrEmpty(PasswordWatermarkTextBox.Text))
                ErrorTextBox.Text = "Veuillez indiquer un nom d'utilisateur et un mot de passe.";
            else if (string.IsNullOrEmpty(UsernameWatermarkTextBox.Text))
                ErrorTextBox.Text = "Veuillez indiquer un nom d'utilisateur.";
            else if (string.IsNullOrEmpty(PasswordWatermarkTextBox.Text))
                ErrorTextBox.Text = "Veuillez indiquer un mot de passe.";
            else
            {
                AssemblyInformations assemblyInformation = new AssemblyInformations(Assembly.GetExecutingAssembly().GetName().Name);

                RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\" + assemblyInformation.Company + "\\" + assemblyInformation.Product + "\\Account");
                registryKey.SetValue("IsLogged", true);
                registryKey.SetValue("Username", UsernameWatermarkTextBox.Text);

                CredentialManager.WriteCredential("SDLPAccount:user=" + UsernameWatermarkTextBox.Text, UsernameWatermarkTextBox.Text, PasswordWatermarkTextBox.Password, CredentialPersistence.Session);

                Close();
            }
        }
    }
}
