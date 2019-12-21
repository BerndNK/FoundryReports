using System;
using System.Collections.Generic;

namespace FoundryReports.Core.Customer.Visualization
{
    public interface ITrend : ICollection<IMonthlyProductReport>
    {
        DateTime Start { get; }
        DateTime End { get; }


    }
}