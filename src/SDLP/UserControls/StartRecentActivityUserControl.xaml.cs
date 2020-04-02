using MRULib.MRU.Interfaces;
using System.Windows.Controls;

namespace SDLP
{
    /// <summary>
    /// Logique d'interaction pour StartRecentActivity.xaml
    /// </summary>
    public partial class StartRecentActivityUserControl : UserControl
    {
        private static IMRUListViewModel mruListViewModel = null;

        private static readonly string mruFilePath;

        public static IMRUListViewModel MRUListViewModel { get { return mruListViewModel; } }

        static StartRecentActivityUserControl()
        {
            mruListViewModel = RecentFiles.MRUListViewModel;
        }
        
        public StartRecentActivityUserControl()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
