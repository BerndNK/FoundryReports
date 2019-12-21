using System;

namespace FoundryReports.Core.Customer.Visualization
{
    public interface IMonthlyProductReport
    {
        bool IsPredicted { get; }

        DateTime ForMonth { get; }

        int Value { get; }
    }
}