using System.Collections.Generic;

namespace FoundryReports.Core.Products
{
    public interface IProduct
    {
        string Name { get; set; }

        IEnumerable<IMoldRequirement> MoldRequirements { get; }

        IMoldRequirement AddMoldRequirement();

        void Remove(IMoldRequirement moldRequirement);
    }
}