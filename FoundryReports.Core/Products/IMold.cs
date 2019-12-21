namespace FoundryReports.Core.Products
{
    public interface IMold
    {
        int MaxUsages { get; }

        int CurrentUsages { get; set; }
    }
}