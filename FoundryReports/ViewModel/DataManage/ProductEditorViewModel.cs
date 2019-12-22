using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;
using Microsoft.Win32;

namespace FoundryReports.ViewModel.DataManage
{
    public class ProductEditorViewModel : ListViewModel<ProductViewModel>
    {
        private readonly IToolSource _toolSource;

        private readonly ObservableCollection<MoldViewModel> _availableMolds;
        
        public ICommand ImportCommand { get; set; }

        public bool IsBusy { get; set; }

        public ProductEditorViewModel(IToolSource toolSource, ObservableCollection<MoldViewModel> availableMolds)
        {
            _toolSource = toolSource;
            _availableMolds = availableMolds;
            ImportCommand = new DelegateCommand(Import);
        }

        private async void Import()
        {
            var fileChooser = new OpenFileDialog {Filter = "Comma delimited (*.csv)|*.csv"};

            if (fileChooser.ShowDialog() == true)
            {
                IsBusy = true;
                var csvImporter = new CsvImporter();
                var importedProducts = csvImporter.ImportProductsFromCsv(fileChooser.FileName);

                await foreach (var importedProduct in importedProducts)
                {
                    // use existing or create new item, to allow importer to enrich data
                    var newProduct = Children.FirstOrDefault(p => p.Name == importedProduct.Name) ?? AddItem();
                    newProduct.Name = importedProduct.Name;
                    foreach (var productMoldRequirement in importedProduct.MoldRequirements)
                    {
                        // use existing or create new item, to allow importer to enrich data
                        var newMoldRequirement =
                            newProduct.Children.FirstOrDefault(r =>
                                r.SelectedMold?.Name == productMoldRequirement.Mold.Name) ?? newProduct.AddItem();
                        newMoldRequirement.UsageAmount = productMoldRequirement.UsageAmount;

                        var correspondingMold =
                            _availableMolds.FirstOrDefault(m => m.Name == productMoldRequirement.Mold.Name);
                        if (correspondingMold == null)
                            continue; // ideally log that the referenced mold was not found, but due to the scope of ths project this is omitted

                        newMoldRequirement.SelectedMold = correspondingMold;
                    }
                }

                IsBusy = false;
            }
        }

        public void Load()
        {
            Children.Clear();
            foreach (var product in _toolSource.Products)
            {
                Children.Add(new ProductViewModel(product, _availableMolds));
            }
        }

        protected override ProductViewModel NewViewModel()
        {
            var newProduct = _toolSource.NewProduct();
            return new ProductViewModel(newProduct, _availableMolds);
        }

        protected override void RemoveFromModel(ProductViewModel viewModel)
        {
            _toolSource.RemoveProduct(viewModel.Product);
        }
    }
}