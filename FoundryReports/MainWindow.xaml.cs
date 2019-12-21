using System.Windows;
using FoundryReports.ViewModel;

namespace FoundryReports
{
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; }

        public MainWindow()
        {
            MainViewModel = new MainViewModel();
            DataContext = this;
            InitializeComponent();
        }
    }
}
