using System.Collections.Generic;
using System.Linq;

namespace FoundryReports.Core.Products
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