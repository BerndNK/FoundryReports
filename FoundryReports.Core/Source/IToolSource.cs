using System.Collections.Generic;
using System.Threading.Tasks;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Source
{
    public interface IToolSource
    {
        IAsyncEnumerable<IProduct> LoadProducts();

        IAsyncEnumerable<IMold> LoadMolds();

        Task PersistChanges();

        IMold NewMold();

        void RemoveMold(IMold mold);

        IProduct NewProduct();

        void RemoveProduct(IProduct product);
    }
}