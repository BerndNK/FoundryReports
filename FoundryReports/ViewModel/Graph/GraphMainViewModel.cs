using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FoundryReports.Core.Source.Prediction;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    public class GraphMainViewModel : BaseViewModel
    {
        private readonly ObservableCollection<ProductViewModel> _products;
        private readonly IEventPredictor _eventPredictor;

        public ObservableCollection<CustomerViewModel> Customer { get; }

        private readonly IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>> _changedProductReports =
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

        public GraphMainViewModel(ObservableCollection<CustomerViewModel> customer,
            ObservableCollection<ProductViewModel> products, IEventPredictor eventPredictor)
        {
            Customer = customer;
            _products = products;
            _eventPredictor = eventPredictor;

            TrendViewModel.MonthlyReportManuallyUpdated += TrendViewModelOnMonthlyReportManuallyUpdated;
            TrendViewModel.PropertyChanged += TrendViewModelOnPropertyChanged;
        }

        private void TrendViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(TrendViewModel.SelectedSegment))
                return;

            EventViewerViewModel.ShowEvents(EventsForSelectedSegment());
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
            RefreshEvents();
        }


        public EventViewerViewModel EventViewerViewModel { get; } = new EventViewerViewModel();

        public TrendViewModel TrendViewModel { get; } = new TrendViewModel();

        public async void LoadTrend()
        {
            if (SelectedCustomer == null)
                return;

            IsBusy = true;

            if (!_changedProductReports.TryGetValue(SelectedCustomer!, out var previouslyChangedReportsForThisCustomer))
                previouslyChangedReportsForThisCustomer = new HashSet<MonthlyProductUsageViewModel>();

            await TrendViewModel.LoadSegments(SelectedCustomer!, _products, previouslyChangedReportsForThisCustomer);

            RefreshEvents();

            IsBusy = false;
        }

        private readonly List<IProductEvent> _productEvents = new List<IProductEvent>();

        private void RefreshEvents()
        {
            var reports = TrendViewModel.AllDisplayedReports();
            var events = _eventPredictor.PredictEvents(reports).ToList();
            TrendViewModel.UpdateEventDisplay(events);
            _productEvents.Clear();
            _productEvents.AddRange(events);
            EventViewerViewModel.ShowEvents(EventsForSelectedSegment());
        }

        private IEnumerable<IProductEvent> EventsForSelectedSegment()
        {
            var selectedSegment = TrendViewModel.SelectedSegment;
            if (selectedSegment == null)
                return Enumerable.Empty<IProductEvent>();

            var reportsInCurrentSegment = selectedSegment.ProductTrends.Select(t => t.CurrentMonth.MonthlyProductReport).ToList();
            return _productEvents.Where(e => reportsInCurrentSegment.Contains(e.ForReport));
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