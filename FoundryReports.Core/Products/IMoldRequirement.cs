using Newtonsoft.Json;

namespace FoundryReports.Core.Products
{
    public interface IMoldRequirement
    {
        IMold Mold { get; set; }

        decimal UsageAmount { get; set; }
    }
}