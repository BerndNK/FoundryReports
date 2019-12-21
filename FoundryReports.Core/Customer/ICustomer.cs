using System.Collections.Generic;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Customer
{
    public interface ICustomer
    {
        IList<IProduct> Products { get; }
    }
}