using System.Threading.Tasks;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;

namespace FoundryReports.ViewModel.DataManage
{
    public class DataManageMainViewModel : BaseViewModel
    {
        private readonly IDataSource _dataSource;
        private bool _isBusy;

        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                _isBusy = value; 
                OnPropertyChanged();
            }
        }

        public MoldEditorViewModel MoldEditor { get; }

        public ProductEditorViewModel ProductEditor { get; }

        public CustomerEditorViewModel CustomerEditor { get; }

        public ICommand PersistCommand { get; }

        public ICommand ImportCommand { get; }

        public DataManageMainViewModel(IDataSource dataSource)
        {
            _dataSource = dataSource;
            MoldEditor = new MoldEditorViewModel(dataSource);
            ProductEditor = new ProductEditorViewModel(dataSource, MoldEditor.Children);
            CustomerEditor = new CustomerEditorViewModel(dataSource, ProductEditor.Children);

            PersistCommand = new DelegateCommand(Persist);
            ImportCommand = new DelegateCommand(Import);
        }

        private void Import()
        {
            throw new System.NotImplementedException();
        }

        private async void Persist()
        {
            IsBusy = true;
            await _dataSource.PersistChanges();
            IsBusy = false;
        }

        public async Task Load()
        {
            IsBusy = true;
            await _dataSource.Load();
            MoldEditor.Load();
            ProductEditor.Load();
            CustomerEditor.Load();
            IsBusy = false;
        }
    }
}
