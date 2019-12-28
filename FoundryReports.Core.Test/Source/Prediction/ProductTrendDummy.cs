using System;
using System.Collections.Generic;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Test.Source.Prediction
{
    internal class ProductTrendDummy : List<IMonthlyProductReport>, IProductTrend
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public IProduct Product { get; set; }
        public decimal MinUsage { get; set; }
        public decimal MaxUsage { get; set; }
    }
}