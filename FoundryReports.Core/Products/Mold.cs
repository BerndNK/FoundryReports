using System;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class Mold : IMold
    {
        public int MaxUsages { get; set;  }

        public int CurrentUsages { get; set; }
    }
}