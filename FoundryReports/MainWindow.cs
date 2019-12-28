using System.Windows;
using FoundryReports.ViewModel;

namespace FoundryReports
{
    public partial class MainWindow
    {
        public MainViewModel MainViewModel { get; }

        public MainWindow()
        {
            MainViewModel = new MainViewModel();
            DataContext = this;
            InitializeComponent();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await MainViewModel.Load();
        }
    }
}
