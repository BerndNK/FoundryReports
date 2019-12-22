using System.Collections.Generic;
using System.Threading.Tasks;
using FoundryReports.Core.Customer;

namespace FoundryReports.Core.Source
{
    public interface ICustomerSource : IAddAndRemove<ICustomer>
    {
        IEnumerable<ICustomer> Customers { get; }

        void PersistChanges();

        Task Load();
    }
}