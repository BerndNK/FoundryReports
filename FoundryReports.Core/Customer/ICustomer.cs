using System.Collections.Generic;
using FoundryReports.Core.Customer.Visualization;
using FoundryReports.Core.Source;

namespace FoundryReports.Core.Customer
{
    public interface ICustomer : IAddAndRemove<IMonthlyProductReport>

    {
    string Name { get; set; }

    IList<IMonthlyProductReport> ProductUsages { get; }

    }
}