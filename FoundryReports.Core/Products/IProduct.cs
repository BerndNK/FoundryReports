using System;
using System.Collections.Generic;

namespace FoundryReports.Core.Products
{
    public interface IProduct
    {
        Guid Guid { get; }

        string Name { get; set; }

        IList<IMoldRequirement> MoldRequirements { get; }

    }
}