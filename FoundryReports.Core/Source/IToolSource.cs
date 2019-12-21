using System.Collections.Generic;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Source
{
    public interface IToolSource
    {
        IEnumerable<IProduct> AllProducts();

        IEnumerable<IMold> AllMolds();

        void PersistChanges();
    }
}