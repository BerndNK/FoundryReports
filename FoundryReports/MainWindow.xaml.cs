using System.Windows;
using FoundryReports.Core.Source.Prediction;
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

            var test = new MlProductTrendPredictor();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            await MainViewModel.Load();
        }
    }
}
