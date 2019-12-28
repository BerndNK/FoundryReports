using System.Collections.Generic;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Source.Prediction
{
    public interface IEventPredictor
    {
        IEnumerable<IProductEvent> PredictEvents(IEnumerable<IMonthlyProductReport> forReports);

    }
}
