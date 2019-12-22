using FoundryReports.Core.Source;

namespace FoundryReports.Core
{
    public class CoreSetup
    {
        public IDataSource DataSource { get; }

        public CoreSetup()
        {
            DataSource = new JsonDataSource();
        }
    }
}