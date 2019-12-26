using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;
using FoundryReports.Core.Source;
using FoundryReports.Core.Utils;

namespace FoundryReports.Core.Reports.Visualization
{
    public class MonthlyUsageIndex
    {
        private readonly IProductTrendPredictor _predictor;
        private readonly IDictionary<DateTime, List<IMonthlyProductReport>> _byMonth;

        private readonly IDictionary<IProduct, List<IMonthlyProductReport>> _byProduct;

        public IEnumerable<DateTime> Months => _byMonth.Keys.OrderBy(x => x).ToList();

        public IEnumerable<IProduct> Products => _byProduct.Keys.ToList();

        public decimal MaxUsage { get; }

        public decimal MinUsage { get; }

        public MonthlyUsageIndex(IEnumerable<IMonthlyProductReport> monthlyProductUsages,
            IProductTrendPredictor predictor)
        {
            _predictor = predictor;
            var asList = monthlyProductUsages.ToList();

            _byMonth = asList.GroupBy(u => u.ForMonth)
                .ToDictionary(g => g.Key, g => g.ToList());

            _byProduct = asList.GroupBy(u => u.ForProduct)
                .ToDictionary(g => g.Key!, g => g.ToList());

            MaxUsage = asList.Max(u => u.Value);
            MinUsage = asList.Min(u => u.Value);
        }

        public IMonthlyProductReport PreviousMonthsUsage(IMonthlyProductReport currentMonth)
        {
            var previousMonth = currentMonth.ForMonth.PreviousMonth();
            if (!Months.Contains(previousMonth))
                return currentMonth;

            return UsageOf(previousMonth, currentMonth.ForProduct);
        }

        public IMonthlyProductReport UsageOf(DateTime month, IProduct product)
        {
            var matchingMonth = GetOrCreateValue(_byMonth, month);
            var matchingProduct = GetOrCreateValue(_byProduct, product);

            var matchingReport = matchingMonth.FirstOrDefault(u => matchingProduct.Contains(u));
            if (matchingReport == null)
                return PredictReport(month, product);

            return matchingReport;
        }

        public IMonthlyProductReport NextMonthsUsage(IMonthlyProductReport currentMonth)
        {
            var nextMonth = currentMonth.ForMonth.NextMonth();
            if (!Months.Contains(nextMonth))
                return PredictReport(nextMonth, currentMonth.ForProduct);

            return UsageOf(nextMonth, currentMonth.ForProduct!);
        }

        private IMonthlyProductReport PredictReport(DateTime nextMonth, IProduct forProduct)
        {
            if (!_byProduct.ContainsKey(forProduct))
                return MonthlyProductReport.Zero(forProduct);

            var trend = ProductTrend(forProduct);
            if (trend.Count < 2)
                return MonthlyProductReport.Zero(forProduct);

            var prediction = _predictor.Predict(nextMonth, trend);
            prediction.ForProduct = forProduct;
            Add(prediction);
            return prediction;
        }

        private void Add(IMonthlyProductReport prediction)
        {
            var productIndex = GetOrCreateValue(_byProduct, prediction.ForProduct);
            productIndex.Add(prediction);

            var monthIndex = GetOrCreateValue(_byMonth, prediction.ForMonth);
            monthIndex.Add(prediction);
        }

        private List<IMonthlyProductReport> GetOrCreateValue<T>(IDictionary<T, List<IMonthlyProductReport>> dictionary, T key)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];
            
            var list = new List<IMonthlyProductReport>();
            dictionary.Add(key, list);

            return list;
        }

        private IProductTrend ProductTrend(IProduct ofProduct)
        {
            var reports = _byProduct[ofProduct];
            var productTrend = new ProductTrend(ofProduct, reports);

            return productTrend;
        }
    }
}