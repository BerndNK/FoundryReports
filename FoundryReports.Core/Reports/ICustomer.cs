using System.Collections.Generic;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Source;

namespace FoundryReports.Core.Reports
{
    public interface ICustomer : IAddAndRemove<IMonthlyProductReport>
    {
        string Name { get; set; }

        IEnumerable<IMonthlyProductReport> MonthlyProductReports { get; }
    }
}