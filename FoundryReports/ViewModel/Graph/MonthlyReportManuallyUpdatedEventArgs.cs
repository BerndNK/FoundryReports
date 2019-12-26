using System;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
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