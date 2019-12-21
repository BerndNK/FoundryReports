using FoundryReports.Core.Source;

namespace FoundryReports.Core
{
    public class CoreSetup
    {
        public IToolSource ToolSource { get; }

        public CoreSetup()
        {
            ToolSource = new JsonToolSource();
        }
    }
}