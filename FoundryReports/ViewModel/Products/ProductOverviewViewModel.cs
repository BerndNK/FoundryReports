using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FoundryReports.Core.Source.Prediction;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Products
{
    public class ProductOverviewViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ProductViewModel> _products;
        private readonly IEventPredictor _eventPredictor;

        public ObservableCollection<CustomerViewModel> Customer { get; }

        public IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>> ChangedProductReports { get; } =
            new Dictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>>();

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

        public ProductOverviewViewModel(ObservableCollection<CustomerViewModel> customer,
            ObservableCollection<ProductViewModel> products, IEventPredictor eventPredictor)
        {
            Customer = customer;
            _products = products;
            _eventPredictor = eventPredictor;

            ProductUsageTrendViewModel.MonthlyReportManuallyUpdated += TrendViewModelOnMonthlyReportManuallyUpdated;
            ProductUsageTrendViewModel.PropertyChanged += TrendViewModelOnPropertyChanged;
        }

        private void TrendViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProductUsageTrendViewModel.SelectedSegment))
                return;

            EventViewerViewModel.ShowEvents(EventsForSelectedSegment());
        }

        private void TrendViewModelOnMonthlyReportManuallyUpdated(object? sender,
            MonthlyReportManuallyUpdatedEventArgs e)
        {
            if (SelectedCustomer == null)
                return;

            if (!ChangedProductReports.TryGetValue(SelectedCustomer, out var hashSet))
            {
                hashSet = new HashSet<MonthlyProductUsageViewModel>();
                ChangedProductReports[SelectedCustomer] = hashSet;
            }

            hashSet.Add(e.MonthlyProductReport);
            RefreshEvents();
        }


        public EventViewerViewModel EventViewerViewModel { get; } = new EventViewerViewModel();

        public ProductUsageTrendViewModel ProductUsageTrendViewModel { get; } = new ProductUsageTrendViewModel();

        public async void LoadTrend()
        {
            if (SelectedCustomer == null)
                return;

            IsBusy = true;

            if (!ChangedProductReports.TryGetValue(SelectedCustomer!, out var previouslyChangedReportsForThisCustomer))
                previouslyChangedReportsForThisCustomer = new HashSet<MonthlyProductUsageViewModel>();

            await ProductUsageTrendViewModel.LoadSegments(SelectedCustomer!, _products,
                previouslyChangedReportsForThisCustomer);

            RefreshEvents();

            IsBusy = false;
        }

        private readonly List<IProductEvent> _productEvents = new List<IProductEvent>();

        private void RefreshEvents()
        {
            var reports = ProductUsageTrendViewModel.AllDisplayedReports();
            var events = _eventPredictor.PredictEvents(reports).ToList();
            ProductUsageTrendViewModel.UpdateEventDisplay(events);
            _productEvents.Clear();
            _productEvents.AddRange(events);
            EventViewerViewModel.ShowEvents(EventsForSelectedSegment());
        }

        private IEnumerable<IProductEvent> EventsForSelectedSegment()
        {
            var selectedSegment = ProductUsageTrendViewModel.SelectedProductSegment;
            if (selectedSegment == null)
                return Enumerable.Empty<IProductEvent>();

            var reportsInCurrentSegment =
                selectedSegment.ProductSegments.Select(t => t.CurrentMonth.MonthlyProductReport).ToList();
            return _productEvents.Where(e => reportsInCurrentSegment.Contains(e.ForReport));
        }

        public void PersistChanges()
        {
            foreach (var changedProductReport in ChangedProductReports)
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

            ChangedProductReports.Clear();
        }
    }
}