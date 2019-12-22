using System;
using System.Collections.Generic;

namespace FoundryReports.Core.Reports.Visualization
{
    public interface ITrend : ICollection<IMonthlyProductReport>
    {
        DateTime Start { get; }
        DateTime End { get; }


    }
}