using FoundryReports.Core.Source;
using FoundryReports.Core.Source.Prediction;

namespace FoundryReports.Core
{
    public class CoreSetup
    {
        public IDataSource DataSource { get; }

        public IEventPredictor EventPredictor { get; }

        public CoreSetup()
        {
            DataSource = new JsonDataSource();
            EventPredictor = new EventPredictor(DataSource);
        }
    }
}