namespace FoundryReports.Core.Products
{
    internal class MoldRequirementDummy : IMoldRequirement
    {
        public IMold Mold { get; set; } = new MoldDummy();

        public decimal UsageAmount { get; set; }
    }
}