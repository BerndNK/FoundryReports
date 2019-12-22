using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;
using Microsoft.Win32;

namespace FoundryReports.ViewModel.DataManage
{
    public class ProductEditorViewModel : BaseViewModel
    {
        private readonly IToolSource _toolSource;

        private readonly ObservableCollection<MoldViewModel> _availableMolds;

        public ObservableCollection<ProductViewModel> Products { get; } = new ObservableCollection<ProductViewModel>();

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        public ICommand ImportCommand { get; set; }

        public bool IsBusy { get; set; }

        public ProductEditorViewModel(IToolSource toolSource, ObservableCollection<MoldViewModel> availableMolds)
        {
            _toolSource = toolSource;
            _availableMolds = availableMolds;
            AddItemCommand = new DelegateCommand(() => AddItem());
            RemoveItemCommand = new DelegateCommand<ProductViewModel>(RemoveItem);
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
                var importedProducts = csvImporter.ImportProductsFromCsv(fileChooser.FileName);

                await foreach (var importedProduct in importedProducts)
                {
                    var newProduct = AddItem();
                    newProduct.Name = importedProduct.Name;
                    foreach (var productMoldRequirement in importedProduct.MoldRequirements)
                    {
                        var newMoldRequirement = newProduct.AddItem();
                        newMoldRequirement.UsageAmount = productMoldRequirement.UsageAmount;

                        var correspondingMold = _availableMolds.FirstOrDefault(m => m.Name == productMoldRequirement.Mold.Name);
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
            Products.Clear();
            foreach (var product in _toolSource.Products)
            {
                Products.Add(new ProductViewModel(product, _availableMolds));
            }
        }

        private ProductViewModel AddItem()
        {
            var newProduct = _toolSource.NewProduct();
            var productViewModel = new ProductViewModel(newProduct, _availableMolds);
            Products.Add(productViewModel);

            return productViewModel;
        }

        private void RemoveItem(ProductViewModel? productViewModel)
        {
            if (productViewModel == null)
                return;

            if (Products.Remove(productViewModel))
            {
                _toolSource.RemoveProduct(productViewModel.Product);
            }
        }
    }
}