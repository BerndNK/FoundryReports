using FoundryReports.Core.Products;

namespace FoundryReports.Core.Reports.Visualization
{
    public interface IProductTrend : ITrend
    {
        IProduct Product { get; }
        decimal MinUsage { get; }
        decimal MaxUsage { get; }
    }
}