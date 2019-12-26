using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Source;
using FoundryReports.Core.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    internal class TrendViewModelFactory
    {
        private IDictionary<IMonthlyProductReport, MonthlyProductUsageViewModel> _viewModelUsages =
            new Dictionary<IMonthlyProductReport, MonthlyProductUsageViewModel>();

        public IEnumerable<TrendSegmentOfSingleMonthViewModel> ProduceSegments(CustomerViewModel forCustomer,
            DateTime from,
            DateTime to, ObservableCollection<ProductViewModel> availableProducts, IProductTrendPredictor predictor, IEnumerable<MonthlyProductUsageViewModel> manuallyChangedReports)
        {
            var relevantUsages = forCustomer.Children.Concat(manuallyChangedReports).Where(r => r.ForMonth >= from && r.ForMonth <= to).ToList();
            var relevantMonths = AllMonthsFromTo(from, to).ToList();

            _viewModelUsages = relevantUsages.ToDictionary(r => r.MonthlyProductReport, r => r);

            if (!relevantUsages.Any())
                yield break;

            var index = CreateIndex(relevantUsages, predictor);

            foreach (var month in relevantMonths)
            {
                var monthTrendSegment = new TrendSegmentOfSingleMonthViewModel(month);

                foreach (var product in index.Products)
                {
                    var thisMonth = index.UsageOf(month, product);
                    var previousMonth = index.PreviousMonthsUsage(thisMonth);
                    var nextMonth = index.NextMonthsUsage(thisMonth);

                    var singleProductTrendSegment =
                        new TrendSegmentOfSingleMonthOfOneProductViewModel(
                            AsViewModel(previousMonth, availableProducts),
                            AsViewModel(thisMonth, availableProducts),
                            AsViewModel(nextMonth, availableProducts), index.MinUsage, index.MaxUsage);

                    monthTrendSegment.ProductTrends.Add(singleProductTrendSegment);
                }

                yield return monthTrendSegment;
            }
        }

        private MonthlyProductUsageViewModel AsViewModel(IMonthlyProductReport report,
            ObservableCollection<ProductViewModel> availableProducts)
        {
            MonthlyProductUsageViewModel monthlyProductUsageViewModel;

            if (_viewModelUsages.ContainsKey(report))
            {
                monthlyProductUsageViewModel = _viewModelUsages[report];
            }
            else
            {
                monthlyProductUsageViewModel = new MonthlyProductUsageViewModel(report, availableProducts);
                _viewModelUsages.Add(report, monthlyProductUsageViewModel);
            }

            // if the product does not exist (is a dummy), availableProducts will not include it. Which means that the SelectedProduct property will be null.
            // However this factory is supposed to ensure non null properties. 
            // All operations on the view model will be ignored, which is intended, as the user should not be able to write data for a product which doesn't exist
            if (monthlyProductUsageViewModel.SelectedProduct == null)
                monthlyProductUsageViewModel.SelectedProduct =
                    new ProductViewModel(report.ForProduct, new ObservableCollection<MoldViewModel>());

            return monthlyProductUsageViewModel;
        }

        private MonthlyUsageIndex CreateIndex(IEnumerable<MonthlyProductUsageViewModel> viewModels,
            IProductTrendPredictor predictor)
        {
            var index = new MonthlyUsageIndex(viewModels.Select(v => v.MonthlyProductReport), predictor);
            return index;
        }

        private IEnumerable<DateTime> AllMonthsFromTo(DateTime fromMonth, DateTime toMonth)
        {
            var currentMonth = fromMonth;
            while (currentMonth <= toMonth)
            {
                yield return currentMonth;
                currentMonth = currentMonth.NextMonth();
            }
        }
    }
}