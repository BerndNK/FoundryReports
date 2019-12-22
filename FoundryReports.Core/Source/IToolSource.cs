using System.Collections.Generic;
using System.Threading.Tasks;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Source
{
    public interface IToolSource
    {
        Task Load();

        IEnumerable<IProduct> Products { get; }

        IEnumerable<IMold> Molds { get; }

        Task PersistChanges();

        IMold NewMold();

        void RemoveMold(IMold mold);

        IProduct NewProduct();

        void RemoveProduct(IProduct product);
    }
}