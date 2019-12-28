namespace FoundryReports.Core.Products
{
    internal class MoldDummy : IMold
    {
        public decimal MaxUsages { get; set; }
        public decimal CurrentUsages { get; set; }
        public decimal CastingCellAmount { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}