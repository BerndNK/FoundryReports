using System.Collections.ObjectModel;
using System.Windows.Input;
using FoundryReports.Core.Source;
using FoundryReports.Utils;

namespace FoundryReports.ViewModel.DataManage
{
    public class CustomerEditorViewModel : ListViewModel<CustomerViewModel>
    {
        private readonly ICustomerSource _customerSource;
        private readonly ObservableCollection<ProductViewModel> _availableProducts;

        public ICommand ImportCommand { get; set; }

        public CustomerEditorViewModel(ICustomerSource customerSource, ObservableCollection<ProductViewModel> availableProducts)
        {
            _customerSource = customerSource;
            _availableProducts = availableProducts;
            ImportCommand = new DelegateCommand(Import);
        }

        private void Import()
        {
            
        }

        public void Load()
        {
            Children.Clear();
            foreach (var customer in _customerSource.Customers)
            {
                Children.Add(new CustomerViewModel(customer, _availableProducts));
            }
        }

        protected override CustomerViewModel NewViewModel()
        {
            var newCustomer = _customerSource.AddItem();
            return new CustomerViewModel(newCustomer, _availableProducts);
        }

        protected override void RemoveFromModel(CustomerViewModel viewModel)
        {
            _customerSource.Remove(viewModel.Customer);
        }
    }
}
