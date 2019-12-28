using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Source;
using FoundryReports.Core.Source.Prediction;
using FoundryReports.Core.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    public class TrendViewModel : BaseViewModel
    {
        public event EventHandler<MonthlyReportManuallyUpdatedEventArgs>? MonthlyReportManuallyUpdated;

        public ObservableCollection<TrendSegmentOfSingleMonthViewModel> MonthlySegments { get; } =
            new ObservableCollection<TrendSegmentOfSingleMonthViewModel>();

        public ObservableCollection<ProductSelectionViewModel> ProductSelection { get; } =
            new ObservableCollection<ProductSelectionViewModel>();

        public ObservableCollection<MonthlyProductUsageViewModel> UsagesOfSelectedSegment { get; } =
            new ObservableCollection<MonthlyProductUsageViewModel>();

        private TrendSegmentOfSingleMonthViewModel? _selectedSegment;

        public TrendSegmentOfSingleMonthViewModel? SelectedSegment
        {
            get => _selectedSegment;
            set
            {
                _selectedSegment = value;
                UpdateUsageViewModelsOfSelectedSegment();
                OnPropertyChanged();
            }
        }

        private void UpdateUsageViewModelsOfSelectedSegment()
        {
            foreach (var monthlyProductUsageViewModel in UsagesOfSelectedSegment)
            {
                monthlyProductUsageViewModel.PropertyChanged -= UsageOfProductInSelectedMonthOnPropertyChanged;
            }

            UsagesOfSelectedSegment.Clear();
            if (SelectedSegment == null)
                return;

            foreach (var usageOfProductInSelectedMonth in SelectedSegment.ProductTrends.Select(t => t.CurrentMonth)
                .Where(IsVisible))
            {
                usageOfProductInSelectedMonth.PropertyChanged += UsageOfProductInSelectedMonthOnPropertyChanged;
                UsagesOfSelectedSegment.Add(usageOfProductInSelectedMonth);
            }
        }

        private bool IsVisible(MonthlyProductUsageViewModel monthlyProductUsageViewModel)
        {
            var correspondingSelection =
                ProductSelection.FirstOrDefault(s => s.Product == monthlyProductUsageViewModel.SelectedProduct);
            if (correspondingSelection == null)
                return true;

            return correspondingSelection.IsSelected;
        }

        /// <summary>
        /// Occurs when the user manually changes the values of the selected month.
        /// </summary>
        private void UsageOfProductInSelectedMonthOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is MonthlyProductUsageViewModel monthlyProductUsageViewModel))
                return;

            UpdateProductTrend(monthlyProductUsageViewModel);

            MonthlyReportManuallyUpdated?.Invoke(this,
                new MonthlyReportManuallyUpdatedEventArgs(monthlyProductUsageViewModel));
        }

        private void UpdateProductTrend(MonthlyProductUsageViewModel monthlyProductUsageViewModel)
        {
            var product = monthlyProductUsageViewModel.SelectedProduct;
            if (product == null || _predictor == null)
                return;

            var allSegmentsOfProduct = MonthlySegments.Select(s => s.ProductTrends.First(t => t.ForProduct == product))
                .ToList();
            foreach (var segment in allSegmentsOfProduct.Where(s => s.CurrentMonth.IsPredicted))
            {
                var asProductTrend = new ProductTrend(product.Product,
                    allSegmentsOfProduct.Where(s => s.CurrentMonth.ForMonth < segment.CurrentMonth.ForMonth)
                        .Select(s => s.CurrentMonth.MonthlyProductReport));
                var newPrediction = _predictor.Predict(segment.CurrentMonth.ForMonth, asProductTrend);
                segment.CurrentMonth.Value = newPrediction.Value;
                segment.CurrentMonth.IsPredicted = true;
            }
        }

        private readonly TrendViewModelFactory _trendViewModelFactory = new TrendViewModelFactory();

        private IProductTrendPredictor? _predictor;

        public async Task LoadSegments(CustomerViewModel customer, ObservableCollection<ProductViewModel> products,
            IEnumerable<MonthlyProductUsageViewModel> manuallyChangedReports)
        {
            var segments = await Task.Run(() =>
            {
                var fromMonth = new DateTime(2019, 1, 1);
                var toMonth = fromMonth.NextMonths(16);
                if (_predictor == null)
                    _predictor = new MlProductTrendPredictor();

                var producedSegments = _trendViewModelFactory
                    .ProduceSegments(customer, fromMonth, toMonth, products, _predictor, manuallyChangedReports)
                    .ToList();
                return producedSegments;
            });

            MonthlySegments.Clear();
            ProductSelection.Clear();
            foreach (var segment in segments)
            {
                MonthlySegments.Add(segment);
            }

            var allProducts = MonthlySegments.SelectMany(m => m.Products)
                .Distinct(ProductViewModel.EqualityByReferringToSameProduct);
            foreach (var productViewModel in allProducts)
            {
                var selection = new ProductSelectionViewModel(productViewModel);
                selection.PropertyChanged += ProductSelectionOnPropertyChanged;
                ProductSelection.Add(selection);
            }
        }

        private void ProductSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProductSelectionViewModel.IsSelected))
                return;

            if (sender is ProductSelectionViewModel asProductSelectionViewModel)
            {
                var productViewModel = asProductSelectionViewModel.Product;
                foreach (var monthSegment in MonthlySegments)
                {
                    monthSegment.SetVisibility(productViewModel, asProductSelectionViewModel.IsSelected);
                }

                UpdateUsageViewModelsOfSelectedSegment();
            }
        }

        public IEnumerable<IMonthlyProductReport> AllDisplayedReports()
        {
            var allSegments = MonthlySegments.SelectMany(x => x.ProductTrends);
            var allReports = allSegments.Select(r => r.CurrentMonth.MonthlyProductReport);

            return allReports;
        }

        public void UpdateEventDisplay(IEnumerable<IProductEvent> productEvents)
        {
            var events = productEvents.ToList();
            var allProductSegments = MonthlySegments.SelectMany(x => x.ProductTrends);
            foreach (var segment in allProductSegments)
            {
                var anyEvents = events.Any(e => e.ForReport == segment.CurrentMonth.MonthlyProductReport);
                segment.HasEvent = anyEvents;
            }
        }
    }
}