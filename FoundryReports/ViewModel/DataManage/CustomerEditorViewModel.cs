using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;
using Microsoft.Win32;

namespace FoundryReports.ViewModel.DataManage
{
    public class CustomerEditorViewModel : ListViewModel<CustomerViewModel>
    {
        private readonly IDataSource _dataSource;

        private readonly ObservableCollection<ProductViewModel> _availableProducts;

        public ICommand ImportCommand { get; set; }

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

        public CustomerEditorViewModel(IDataSource dataSource, ObservableCollection<ProductViewModel> availableProducts)
        {
            _dataSource = dataSource;
            _availableProducts = availableProducts;
            ImportCommand = new DelegateCommand(Import);
        }

        private async void Import()
        {
            var fileChooser = new OpenFileDialog {Filter = "Comma delimited (*.csv)|*.csv"};

            if (fileChooser.ShowDialog() == true)
            {
                IsBusy = true;
                var csvImporter = new CsvImporter();
                var customer = await csvImporter.ImportCustomerFromCsv(fileChooser.FileName);

                // use existing or create new item, to allow importer to enrich data
                var newCustomer = Children.FirstOrDefault(p => p.Name == customer.Name) ?? AddItem();
                newCustomer.Name = customer.Name;
                foreach (var monthlyProductReport in customer.MonthlyProductReports)
                {
                    // use existing or create new item, to allow importer to enrich data
                    var newReport =
                        newCustomer.Children.FirstOrDefault(r =>
                            r.SelectedProduct?.Name == monthlyProductReport.ForProduct.Name && r.ForMonth == monthlyProductReport.ForMonth) ?? newCustomer.AddItem();

                    newReport.Value = monthlyProductReport.Value;
                    newReport.ForMonth = monthlyProductReport.ForMonth;

                    var correspondingProduct =
                        _availableProducts.FirstOrDefault(m => m.Name == monthlyProductReport.ForProduct.Name);
                    if (correspondingProduct == null)
                        continue; // ideally log that the referenced product was not found, but due to the scope of ths project this is omitted

                    newReport.SelectedProduct = correspondingProduct;
                }


                IsBusy = false;
            }
        }

        public void Load()
        {
            Children.Clear();
            foreach (var customer in _dataSource.Customer)
            {
                Children.Add(new CustomerViewModel(customer, _availableProducts));
            }
        }

        protected override CustomerViewModel NewViewModel()
        {
            var newCustomer = _dataSource.AddCustomer();
            return new CustomerViewModel(newCustomer, _availableProducts);
        }

        protected override void RemoveFromModel(CustomerViewModel viewModel)
        {
            _dataSource.RemoveCustomer(viewModel.Customer);
        }
    }
}