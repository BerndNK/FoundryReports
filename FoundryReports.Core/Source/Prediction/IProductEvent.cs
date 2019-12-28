using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Source.Prediction
{
    public interface IProductEvent
    {
        IMonthlyProductReport ForReport { get; }
    }
}