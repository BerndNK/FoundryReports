using FoundryReports.Core.Products;

namespace FoundryReports.Core.Customer.Visualization
{
    public interface IProductTrend : ITrend
    {
        IProduct Product { get; }

    }
}