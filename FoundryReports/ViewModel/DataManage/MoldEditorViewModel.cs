using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;
using Microsoft.Win32;

namespace FoundryReports.ViewModel.DataManage
{
    public class MoldEditorViewModel : BaseViewModel
    {
        private readonly IToolSource _toolSource;

        public ObservableCollection<MoldViewModel> Molds { get; } = new ObservableCollection<MoldViewModel>();

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand ImportCommand { get; set; }

        public bool IsBusy { get; set; }

        public MoldEditorViewModel(IToolSource toolSource)
        {
            _toolSource = toolSource;
            AddItemCommand = new DelegateCommand(() => AddItem());
            RemoveItemCommand = new DelegateCommand<MoldViewModel>(RemoveItem);
            ImportCommand = new DelegateCommand(Import);
        }

        private async void Import()
        {
            var fileChooser = new OpenFileDialog();
            fileChooser.Filter = "Comma delimited (*.csv)|*.csv";
            if (fileChooser.ShowDialog() == true)
            {
                IsBusy = true;
                var csvImporter = new CsvImporter();
                var importedMolds = csvImporter.ImportMoldsFromCsv(fileChooser.FileName);

                await foreach (var importedMold in importedMolds)
                {
                    var newMold = AddItem();
                    newMold.Name = importedMold.Name;
                    newMold.CurrentUsages = importedMold.CurrentUsages;
                    newMold.MaxUsages = importedMold.MaxUsages;
                    newMold.CastingCellAmount = importedMold.CastingCellAmount;
                }

                IsBusy = false;
            }
        }

        public void Load()
        {
            Molds.Clear();
            foreach (var mold in _toolSource.Molds)
            {
                Molds.Add(new MoldViewModel(mold));
            }
        }

        private MoldViewModel AddItem()
        {
            var newMold = _toolSource.NewMold();
            var moldViewModel = new MoldViewModel(newMold);
            Molds.Add(moldViewModel);

            return moldViewModel;
        }

        private void RemoveItem(MoldViewModel? moldViewModel)
        {
            if (moldViewModel == null)
                return;

            if (Molds.Remove(moldViewModel))
            {
                _toolSource.RemoveMold(moldViewModel.Mold);
            }
        }
    }
}