using System;
using System.Collections.Generic;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class Product : IProduct
    {
        public Guid Guid { get; set; }

        public string Name { get; set; } = "New Product";

        public IList<IMoldRequirement> MoldRequirements { get; set; } = new List<IMoldRequirement>();
    }
}