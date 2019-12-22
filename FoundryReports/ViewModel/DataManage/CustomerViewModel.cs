using System.Collections.ObjectModel;
using FoundryReports.Core.Reports;

namespace FoundryReports.ViewModel.DataManage
{
    public class CustomerViewModel : ListViewModel<MonthlyProductUsageViewModel>
    {
        private readonly ObservableCollection<ProductViewModel> _availableProducts;

        public ICustomer Customer { get; }

        public string Name
        {
            get => Customer.Name;
            set
            {
                Customer.Name = value;
                OnPropertyChanged();
            }
        }

        public CustomerViewModel(ICustomer customer, ObservableCollection<ProductViewModel> availableProducts)
        {
            _availableProducts = availableProducts;
            Customer = customer;
            foreach (var customerProductUsage in customer.MonthlyProductReports)
            {
                Children.Add(new MonthlyProductUsageViewModel(customerProductUsage, availableProducts));
            }
        }

        protected override MonthlyProductUsageViewModel NewViewModel()
        {
            var monthlyProductReport = Customer.AddItem();
            return new MonthlyProductUsageViewModel(monthlyProductReport, _availableProducts);
        }

        protected override void RemoveFromModel(MonthlyProductUsageViewModel viewModel)
        {
            Customer.Remove(viewModel.MonthlyProductReport);
        }
    }
}