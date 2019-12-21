using System.Collections.Generic;
using FoundryReports.Core.Customer.Visualization;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Customer
{
    public interface ICustomerOverview
    {
        IList<IProductTrend> ProductTrends { get; }

        IList<IProduct> Products { get; }

    }
}