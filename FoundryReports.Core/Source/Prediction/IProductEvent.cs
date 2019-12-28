using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Source.Prediction
{
    public interface IProductEvent
    {
        IMonthlyProductReport ForReport { get; }

        string DisplayName { get; }

        string Description { get; }
    }
}