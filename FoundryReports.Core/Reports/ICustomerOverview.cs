using System.Collections.Generic;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Reports
{
    public interface ICustomerOverview
    {
        IList<IProductTrend> ProductTrends { get; }

        IList<IProduct> Products { get; }

    }
}