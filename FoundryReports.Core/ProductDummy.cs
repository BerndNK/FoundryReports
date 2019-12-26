using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;

namespace FoundryReports.Core
{
    internal class ProductDummy : IProduct
    {
        public IMoldRequirement AddItem() => new MoldRequirementDummy();

        public void Remove(IMoldRequirement item)
        {
        }

        public string Name { get; set; } = "[Unknown]";

        public IEnumerable<IMoldRequirement> MoldRequirements => Enumerable.Empty<IMoldRequirement>();
    }
}