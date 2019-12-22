using System;
using System.Collections.ObjectModel;
using FoundryReports.Core.Customer;

namespace FoundryReports.ViewModel.DataManage
{
    public class CustomerViewModel
    {
        public ICustomer Customer { get; }
        
        public CustomerViewModel(ICustomer customer, ObservableCollection<ProductViewModel> availableProducts)
        {
            Customer = customer;
            throw new NotImplementedException();
        }
    }
}
