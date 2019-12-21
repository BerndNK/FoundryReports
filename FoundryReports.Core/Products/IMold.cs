namespace FoundryReports.Core.Products
{
    public interface IMold
    {
        int MaxUsages { get; set; }

        int CurrentUsages { get; set; }
        
        public decimal CastingCellAmount { get; set; }

        string Name { get; set; }
    }
}