using System;
using System.Collections.Generic;

namespace FoundryReports.Core.Products
{
    public interface IProduct
    {
        Guid Guid { get; }

        IEnumerable<IMoldRequirement> MoldRequirements { get; }

    }
}