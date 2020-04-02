using Dev2Be.AssemblyInformationCalling;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour ConfiguratorWindow.xaml
    /// </summary>
    public partial class ConfiguratorWindow : Window
    {
        public ConfiguratorWindow()
        {
            InitializeComponent();

            ApplicationContainer.ApplicationPath = Path.Combine(Directory.GetCurrentDirectory(), "Configurator.exe");
            ApplicationContainer.Arguments = "--appName \"" + new AssemblyInformation(Assembly.GetExecutingAssembly().GetName().Name).Product + "\"";
            ApplicationContainer.PropertyChanged += ApplicationContainer_PropertyChanged;

            Unloaded += new RoutedEventHandler((s, e) => { ApplicationContainer.Dispose(); });
        }

        public void ShowDialog(Window owner)
        {
            Owner = owner;

            ShowDialog();
        }

        private void ApplicationContainer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDisposed")
                Dispatcher.Invoke(() => Close());
        }
    }
}
