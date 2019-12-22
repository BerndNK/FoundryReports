using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    public class GraphMainViewModel
    {
        private readonly ObservableCollection<CustomerViewModel> _customer;
        private readonly ObservableCollection<ProductViewModel> _products;

        public GraphMainViewModel(ObservableCollection<CustomerViewModel> customer,
            ObservableCollection<ProductViewModel> products)
        {
            _customer = customer;
            _products = products;
        }

        public TrendViewModel TrendViewModel { get; } = new TrendViewModel();

        public async Task Load()
        {
            var customer = _customer.FirstOrDefault();
            if (customer == null)
                return;

            await TrendViewModel.LoadSegments(customer, _products);
        }
    }
}