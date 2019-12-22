using System;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Reports.Visualization
{
    public interface IMonthlyProductReport
    {
        bool IsPredicted { get; set; }

        DateTime ForMonth { get; set; }

        int Value { get; set; }

        IProduct ForProduct { get; set; }
    }
}