using System;
using System.Collections.Generic;
using System.Text;
using FoundryReports.Core.Customer;

namespace FoundryReports.Core.Products
{
    [Serializable]
    internal class Product : IProduct
    {
        public Guid Guid { get; set; }

        public IEnumerable<IMoldRequirement> MoldRequirements => MoldRequirementsList;

        public IList<IMoldRequirement> MoldRequirementsList = new List<IMoldRequirement>();
    }
}
