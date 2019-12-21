using System;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class MoldRequirement : IMoldRequirement
    {
        public IMold Mold { get; set; }

        public decimal CastingCellAmount { get; set; }
    }
}