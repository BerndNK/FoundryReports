using System;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Test.Source.Prediction
{
    internal class MonthlyProductReportDummy : IMonthlyProductReport
    {
        public bool IsPredicted { get; set; }
        public DateTime ForMonth { get; set; }
        public decimal Value { get; set; }
        public IProduct ForProduct { get; set; }
    }
}