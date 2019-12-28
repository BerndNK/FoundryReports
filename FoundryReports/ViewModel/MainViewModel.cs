using System.Threading.Tasks;
using System.Windows.Input;
using FoundryReports.Core;
using FoundryReports.Utils;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly CoreSetup _setup;

        public GraphMainViewModel Graph { get; }

        public DataManageMainViewModel DataManage { get; }
        
        public ICommand PersistCommand { get; }

        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            _setup = new CoreSetup();
            DataManage = new DataManageMainViewModel(_setup.DataSource);
            Graph = new GraphMainViewModel(DataManage.CustomerEditor.Children, DataManage.ProductEditor.Children, _setup.EventPredictor);
            PersistCommand = new DelegateCommand(Persist);
        }

        private async void Persist()
        {
            IsBusy = true;
            Graph.PersistChanges();
            await _setup.DataSource.PersistChanges();
            IsBusy = false;
        }

        public async Task Load()
        {
            IsBusy = true;
            await DataManage.Load();
            IsBusy = false;
        }
    }
}
