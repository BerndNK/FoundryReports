using System.Collections.Generic;
using FoundryReports.Core.Customer;

namespace FoundryReports.Core.Source
{
    
    public interface ICustomerSource
    {
        IEnumerable<ICustomer> Customers();
     
        void PersistChanges();
    }
}
