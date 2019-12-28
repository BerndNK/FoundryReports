using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.Core.Source.Prediction;
using FoundryReports.Core.Utils;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Products;

namespace FoundryReports.ViewModel.CastingCell
{
    public class CastingCellUsageTrendViewModel : BaseProductTrendViewModel<CastingCellUsageTrendSegmentViewModel>
    {
        private MlProductTrendPredictor? _predictor;

        private readonly ProductUsageTrendViewModelFactory _trendViewModelFactory =
            new ProductUsageTrendViewModelFactory();

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
                MonthlyProductSegments.Add(new CastingCellUsageTrendSegmentViewModel(segment, products));
            }

            UpdatesAfterVisibilityChanged();
            var allProducts = segments.SelectMany(m => m.Products).Distinct(ProductViewModel.EqualityByReferringToSameProduct);
            foreach (var productViewModel in allProducts)
            {
                var selection = new ProductSelectionViewModel(productViewModel);
                selection.PropertyChanged += ProductSelectionOnPropertyChanged;
                ProductSelection.Add(selection);
            }
        }

        protected override void UpdatesAfterVisibilityChanged()
        {
            
            var minUsage = MonthlyProductSegments.Min(s => s.TrendSegments.Min(p => p.Value));
            var maxUsage = MonthlyProductSegments.Max(s => s.TrendSegments.Max(p => p.Value));

            foreach (var monthSegment in MonthlyProductSegments)
            {
                monthSegment.MaxUsage = maxUsage;
                monthSegment.MinUsage = minUsage;
            }
        }
    }
}