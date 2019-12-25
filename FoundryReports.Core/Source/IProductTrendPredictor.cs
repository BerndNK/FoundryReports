using System;
using FoundryReports.Core.Reports;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Source
{
    public interface IProductTrendPredictor
    {
        IMonthlyProductReport Predict(DateTime forMonth, IProductTrend forTrend);
    }
}