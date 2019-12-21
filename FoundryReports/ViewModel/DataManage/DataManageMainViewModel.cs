using System.Threading.Tasks;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;

namespace FoundryReports.ViewModel.DataManage
{
    public class DataManageMainViewModel : BaseViewModel
    {
        private readonly IToolSource _toolSource;
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

        public ICommand PersistCommand { get; }

        public ICommand ImportCommand { get; }

        public DataManageMainViewModel(IToolSource toolSource)
        {
            _toolSource = toolSource;
            MoldEditor = new MoldEditorViewModel(toolSource);
            ProductEditor = new ProductEditorViewModel(toolSource);
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
            await _toolSource.PersistChanges();
            IsBusy = false;
        }

        public async Task Load()
        {
            IsBusy = true;
            await Task.WhenAll(MoldEditor.Load(), ProductEditor.Load());
            IsBusy = false;
        }
    }
}
