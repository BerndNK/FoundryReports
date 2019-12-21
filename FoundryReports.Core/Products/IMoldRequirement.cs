namespace FoundryReports.Core.Products
{
    public interface IMoldRequirement
    {
        IMold Mold { get; }

        decimal CastingCellAmount { get; }
    }
}