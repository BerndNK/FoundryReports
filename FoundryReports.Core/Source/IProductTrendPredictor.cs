using System;
using FoundryReports.Core.Customer;
using FoundryReports.Core.Customer.Visualization;

namespace FoundryReports.Core.Source
{
    public interface IProductTrendPredictor
    {
        IMonthlyProductReport Predict(DateTime forMonth, IProductTrend forTrend, ICustomerOverview overview);
    }
}