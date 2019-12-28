using System.Threading.Tasks;
using System.Windows.Input;
using FoundryReports.Core;
using FoundryReports.Utils;
using FoundryReports.ViewModel.CastingCell;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Products;

namespace FoundryReports.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly CoreSetup _setup;

        public CastingCellOverviewViewModel CastingCellOverview { get; }

        public ProductOverviewViewModel ProductOverview { get; }

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
            ProductOverview = new ProductOverviewViewModel(DataManage.CustomerEditor.Children, DataManage.ProductEditor.Children, _setup.EventPredictor);
            CastingCellOverview = new CastingCellOverviewViewModel(DataManage.CustomerEditor.Children, DataManage.ProductEditor.Children, ProductOverview.ChangedProductReports);
            PersistCommand = new DelegateCommand(Persist);
        }

        private async void Persist()
        {
            IsBusy = true;
            ProductOverview.PersistChanges();
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
