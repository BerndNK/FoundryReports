using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.Core.Reports;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    internal class TrendViewModelFactory
    {
        public IEnumerable<TrendSegmentOfSingleMonthViewModel> ProduceSegments(ICustomer forCustomer, DateTime from,
            DateTime to, ObservableCollection<ProductViewModel> availableProducts)
        {
            var relevantUsages = forCustomer.MonthlyProductReports.Where(r => r.ForMonth >= from && r.ForMonth <= to);
            var monthlyProductUsageViewModels =
                relevantUsages.Select(u => new MonthlyProductUsageViewModel(u, availableProducts)).ToList();

            if (!monthlyProductUsageViewModels.Any())
                yield break;

            var index = new MonthlyUsageIndex(monthlyProductUsageViewModels);

            foreach (var month in index.Months)
            {
                var monthTrendSegment = new TrendSegmentOfSingleMonthViewModel(month);

                foreach (var product in index.Products)
                {
                    var thisMonth = index.UsageOf(month, product);
                    var previousMonth = index.PreviousMonthsUsage(thisMonth);
                    var nextMonth = index.NextMonthsUsage(thisMonth);

                    var singleProductTrendSegment =
                        new TrendSegmentOfSingleMonthOfOneProductViewModel(previousMonth, thisMonth, nextMonth, index.MinUsage, index.MaxUsage);
                    monthTrendSegment.ProductTrends.Add(singleProductTrendSegment);
                }

                yield return monthTrendSegment;
            }
        }
    }

    internal class MonthlyUsageIndex
    {
        private readonly IDictionary<DateTime, IEnumerable<MonthlyProductUsageViewModel>> _byMonth;

        private readonly IDictionary<ProductViewModel, IEnumerable<MonthlyProductUsageViewModel>> _byProduct;

        public IEnumerable<DateTime> Months => _byMonth.Keys.OrderBy(x => x);

        public IEnumerable<ProductViewModel> Products => _byProduct.Keys;

        public decimal MaxUsage { get; }

        public decimal MinUsage { get; }

        public MonthlyUsageIndex(IEnumerable<MonthlyProductUsageViewModel> monthlyProductUsages)
        {
            var asList = monthlyProductUsages.Where(u => u.SelectedProduct != null).ToList();

            _byMonth = asList.GroupBy(u => u.ForMonth)
                .ToDictionary(g => g.Key, g => g as IEnumerable<MonthlyProductUsageViewModel>);
            _byProduct = asList.GroupBy(u => u.SelectedProduct)
                .ToDictionary(g => g.Key!, g => g as IEnumerable<MonthlyProductUsageViewModel>);

            MaxUsage = asList.Max(u => u.Value);
            MinUsage = asList.Min(u => u.Value);
        }

        public MonthlyProductUsageViewModel PreviousMonthsUsage(MonthlyProductUsageViewModel currentMonth)
        {
            var previousMonth = currentMonth.ForMonth.AddMonths(-1);
            if (!Months.Contains(previousMonth))
                return currentMonth;

            return UsageOf(previousMonth, currentMonth.SelectedProduct!);
        }

        public MonthlyProductUsageViewModel UsageOf(DateTime month, ProductViewModel productName)
        {
            var matchingMonth = _byMonth[month];
            var matchingProduct = _byProduct[productName];

            return matchingMonth.First(u => matchingProduct.Contains(u));
        }

        public MonthlyProductUsageViewModel NextMonthsUsage(MonthlyProductUsageViewModel currentMonth)
        {
            var nextMonth = currentMonth.ForMonth.AddMonths(1);
            if (!Months.Contains(nextMonth))
                return currentMonth;

            return UsageOf(nextMonth, currentMonth.SelectedProduct!);
        }
    }
}