using System;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class Mold : IMold
    {
        public decimal MaxUsages { get; set;  }

        public decimal CurrentUsages { get; set; }

        public decimal CastingCellAmount { get; set; }

        public string Name { get; set; } = "New Mold";
    }
}