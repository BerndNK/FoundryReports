using System.Collections.Generic;
using FoundryReports.Core.Source;

namespace FoundryReports.Core.Products
{
    public interface IProduct : IAddAndRemove<IMoldRequirement>
    {
        string Name { get; set; }

        IEnumerable<IMoldRequirement> MoldRequirements { get; }
    }
}