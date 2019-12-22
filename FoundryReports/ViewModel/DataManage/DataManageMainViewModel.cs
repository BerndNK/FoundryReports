using System.Threading.Tasks;
using FoundryReports.Core.Source;

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

        public DataManageMainViewModel(IDataSource dataSource)
        {
            _dataSource = dataSource;
            MoldEditor = new MoldEditorViewModel(dataSource);
            ProductEditor = new ProductEditorViewModel(dataSource, MoldEditor.Children);
            CustomerEditor = new CustomerEditorViewModel(dataSource, ProductEditor.Children);
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
