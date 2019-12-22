using System.Collections.Generic;
using System.Threading.Tasks;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports;

namespace FoundryReports.Core.Source
{
    public interface IDataSource
    {
        Task Load();

        Task PersistChanges();

        IEnumerable<IProduct> Products { get; }

        IEnumerable<IMold> Molds { get; }
        
        IEnumerable<ICustomer> Customer { get; }

        IMold NewMold();

        void RemoveMold(IMold mold);

        IProduct NewProduct();

        void RemoveProduct(IProduct product);

        ICustomer AddCustomer();

        void RemoveCustomer(ICustomer item);
    }
}