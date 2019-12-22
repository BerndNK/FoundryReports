using FoundryReports.Core.Products;

namespace FoundryReports.Core
{
    internal class MoldRequirementDummy : IMoldRequirement
    {
        public IMold Mold { get; set; } = new MoldDummy();

        public decimal UsageAmount { get; set; }
    }
}