using System;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Products
{
    public class MonthlyReportManuallyUpdatedEventArgs : EventArgs
    {
        public MonthlyProductUsageViewModel MonthlyProductReport { get; }

        public MonthlyReportManuallyUpdatedEventArgs(MonthlyProductUsageViewModel monthlyProductReport)
        {
            MonthlyProductReport = monthlyProductReport;
        }
    }
}