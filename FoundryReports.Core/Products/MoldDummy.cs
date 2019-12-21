namespace FoundryReports.Core.Products
{
    internal class MoldDummy : IMold
    {
        public int MaxUsages { get; set; }
        public int CurrentUsages { get; set; }
        public decimal CastingCellAmount { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}