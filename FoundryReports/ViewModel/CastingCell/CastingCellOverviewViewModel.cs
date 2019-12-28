using System.Collections.Generic;
using System.Collections.ObjectModel;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.CastingCell
{
    public class CastingCellOverviewViewModel : BaseViewModel
    {
        private readonly IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>> _manuallyChangedProductUsages;
        public ObservableCollection<CustomerViewModel> Customer { get; }
        private CustomerViewModel? _selectedCustomer;

        public CustomerViewModel? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                LoadTrend();
            }
        }

        private bool _isBusy;
        private readonly ObservableCollection<ProductViewModel> _products;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public CastingCellUsageTrendViewModel CastingCellUsageTrendViewModel { get; } = new CastingCellUsageTrendViewModel();

        public CastingCellOverviewViewModel(ObservableCollection<CustomerViewModel> customer, ObservableCollection<ProductViewModel> products, IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>> manuallyChangedProductUsages)
        {
            Customer = customer;
            _products = products;
            _manuallyChangedProductUsages = manuallyChangedProductUsages;
        }
        
        public async void LoadTrend()
        {
            if (SelectedCustomer == null)
                return;

            IsBusy = true;
            
            if (!_manuallyChangedProductUsages.TryGetValue(SelectedCustomer!, out var previouslyChangedReportsForThisCustomer))
                previouslyChangedReportsForThisCustomer = new HashSet<MonthlyProductUsageViewModel>();

            await CastingCellUsageTrendViewModel.LoadSegments(SelectedCustomer!, _products, previouslyChangedReportsForThisCustomer);


            IsBusy = false;
        }
    }
}
