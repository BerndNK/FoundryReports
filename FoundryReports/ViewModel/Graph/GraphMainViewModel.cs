using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    public class GraphMainViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ProductViewModel> _products;

        public ObservableCollection<CustomerViewModel> Customer { get; }

        private readonly IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>> _changedProductReports =
            new Dictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>>();

        public ICommand RefreshTrendCommand { get; }

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

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public GraphMainViewModel(ObservableCollection<CustomerViewModel> customer,
            ObservableCollection<ProductViewModel> products)
        {
            Customer = customer;
            _products = products;

            TrendViewModel.MonthlyReportManuallyUpdated += TrendViewModelOnMonthlyReportManuallyUpdated;
            RefreshTrendCommand = new DelegateCommand(LoadTrend);
        }

        private void TrendViewModelOnMonthlyReportManuallyUpdated(object? sender,
            MonthlyReportManuallyUpdatedEventArgs e)
        {
            if (SelectedCustomer == null)
                return;

            if (!_changedProductReports.TryGetValue(SelectedCustomer, out var hashSet))
            {
                hashSet = new HashSet<MonthlyProductUsageViewModel>();
                _changedProductReports[SelectedCustomer] = hashSet;
            }

            hashSet.Add(e.MonthlyProductReport);
        }

        public TrendViewModel TrendViewModel { get; } = new TrendViewModel();

        public async void LoadTrend()
        {
            if (SelectedCustomer == null)
                return;

            IsBusy = true;

            if (!_changedProductReports.TryGetValue(SelectedCustomer!, out var previouslyChangedReportsForThisCustomer))
                previouslyChangedReportsForThisCustomer = new HashSet<MonthlyProductUsageViewModel>();

            await TrendViewModel.LoadSegments(SelectedCustomer!, _products, previouslyChangedReportsForThisCustomer);

            IsBusy = false;
        }

        public void PersistChanges()
        {
            foreach (var changedProductReport in _changedProductReports)
            {
                var customer = changedProductReport.Key;
                var reports = changedProductReport.Value;

                foreach (var changedReport in reports.Where(r => !r.IsPredicted))
                {
                    var newReport = customer.AddItem();
                    newReport.Value = changedReport.Value;
                    newReport.ForMonth = changedReport.ForMonth;
                    newReport.SelectedProduct = changedReport.SelectedProduct;
                }
            }

            _changedProductReports.Clear();
        }
    }
}