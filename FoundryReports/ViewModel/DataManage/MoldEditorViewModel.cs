using System.Linq;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;
using Microsoft.Win32;

namespace FoundryReports.ViewModel.DataManage
{
    public class MoldEditorViewModel : ListViewModel<MoldViewModel>
    {
        private readonly IDataSource _dataSource;

        public ICommand ImportCommand { get; set; }

        public bool IsBusy { get; set; }

        public MoldEditorViewModel(IDataSource dataSource)
        {
            _dataSource = dataSource;
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
                    // use existing or create new item, to allow importer to enrich data
                    var newMold = Children.FirstOrDefault(m => m.Name == importedMold.Name) ?? AddItem();
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
            Children.Clear();
            foreach (var mold in _dataSource.Molds)
            {
                Children.Add(new MoldViewModel(mold));
            }
        }

        protected override MoldViewModel NewViewModel()
        {
            var newMold = _dataSource.NewMold();
            return new MoldViewModel(newMold);
        }

        protected override void RemoveFromModel(MoldViewModel viewModel)
        {
            _dataSource.RemoveMold(viewModel.Mold);
        }
    }
}