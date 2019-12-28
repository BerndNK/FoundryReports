namespace FoundryReports.Core.Products
{
    public interface IMold
    {
        decimal MaxUsages { get; set; }

        decimal CurrentUsages { get; set; }
        
        public decimal CastingCellAmount { get; set; }

        string Name { get; set; }
    }
}