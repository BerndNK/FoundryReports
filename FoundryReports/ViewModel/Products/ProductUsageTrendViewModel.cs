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

namespace FoundryReports.ViewModel.Products
{
    public class ProductUsageTrendViewModel : BaseProductTrendViewModel<TrendSegmentOfSingleMonthViewModel>
    {
        public event EventHandler<MonthlyReportManuallyUpdatedEventArgs>? MonthlyReportManuallyUpdated;

        public ObservableCollection<MonthlyProductUsageViewModel> UsagesOfSelectedSegment { get; } =
            new ObservableCollection<MonthlyProductUsageViewModel>();
        

        private void UpdateUsageViewModelsOfSelectedSegment()
        {
            foreach (var monthlyProductUsageViewModel in UsagesOfSelectedSegment)
            {
                monthlyProductUsageViewModel.PropertyChanged -= UsageOfProductInSelectedMonthOnPropertyChanged;
            }

            UsagesOfSelectedSegment.Clear();
            if (SelectedProductSegment == null)
                return;

            foreach (var usageOfProductInSelectedMonth in SelectedProductSegment.ProductSegments
                .Select(t => t.CurrentMonth)
                .Where(IsVisible))
            {
                usageOfProductInSelectedMonth.PropertyChanged += UsageOfProductInSelectedMonthOnPropertyChanged;
                UsagesOfSelectedSegment.Add(usageOfProductInSelectedMonth);
            }
        }

        /// <summary>
        /// Occurs when the user manually changes the values of the selected month.
        /// </summary>
        private void UsageOfProductInSelectedMonthOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is MonthlyProductUsageViewModel monthlyProductUsageViewModel))
                return;

            UpdateProductTrend(monthlyProductUsageViewModel);
            UpdatesAfterVisibilityChanged();

            MonthlyReportManuallyUpdated?.Invoke(this,
                new MonthlyReportManuallyUpdatedEventArgs(monthlyProductUsageViewModel));
        }

        private void UpdateProductTrend(MonthlyProductUsageViewModel monthlyProductUsageViewModel)
        {
            var product = monthlyProductUsageViewModel.SelectedProduct;
            if (product == null || _predictor == null)
                return;

            var allSegmentsOfProduct = MonthlyProductSegments
                .Select(s => s.ProductSegments.First(t => t.ForProduct == product))
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

        private readonly ProductUsageTrendViewModelFactory _trendViewModelFactory = new ProductUsageTrendViewModelFactory();

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

            MonthlyProductSegments.Clear();
            ProductSelection.Clear();
            foreach (var segment in segments)
            {
                MonthlyProductSegments.Add(segment);
            }

            var allProducts = MonthlyProductSegments.SelectMany(m => m.Products)
                .Distinct(ProductViewModel.EqualityByReferringToSameProduct);
            foreach (var productViewModel in allProducts)
            {
                var selection = new ProductSelectionViewModel(productViewModel);
                selection.PropertyChanged += ProductSelectionOnPropertyChanged;
                ProductSelection.Add(selection);
            }

            UpdatesAfterVisibilityChanged();
        }

        public IEnumerable<IMonthlyProductReport> AllDisplayedReports()
        {
            var allSegments = MonthlyProductSegments.SelectMany(x => x.ProductSegments);
            var allReports = allSegments.Select(r => r.CurrentMonth.MonthlyProductReport);

            return allReports;
        }

        public void UpdateEventDisplay(IEnumerable<IProductEvent> productEvents)
        {
            var events = productEvents.ToList();
            var allProductSegments = MonthlyProductSegments.SelectMany(x => x.ProductSegments);
            foreach (var segment in allProductSegments)
            {
                var anyEvents = events.Any(e => e.ForReport == segment.CurrentMonth.MonthlyProductReport);
                segment.HasEvent = anyEvents;
            }
        }

        protected override void UpdateSelectedSegmentRelevantProperties() => UpdateUsageViewModelsOfSelectedSegment();

        protected override void UpdatesAfterVisibilityChanged()
        {
            if (!MonthlyProductSegments.Any(s => s.ProductSegments.Any(x => x.IsVisible)))
                return;

            var minUsage = MonthlyProductSegments.Min(s => s.ProductSegments.Where(x => x.IsVisible).Min(p => p.CurrentMonth.Value));
            var maxUsage = MonthlyProductSegments.Max(s => s.ProductSegments.Where(x => x.IsVisible).Max(p => p.CurrentMonth.Value));

            // avoid division by 0
            if (maxUsage - minUsage == 0)
                return;

            foreach (var monthSegment in MonthlyProductSegments)
            {
                foreach (var productSegment in monthSegment.ProductSegments)
                {
                    productSegment.MinUsage = minUsage;
                    productSegment.MaxUsage = maxUsage;
                }
            }

            CreateYAxisDescriptions(minUsage, maxUsage);
        }
    }
}