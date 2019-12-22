using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Reports;
using FoundryReports.Core.Reports.Visualization;
using Newtonsoft.Json;

namespace FoundryReports.Core
{
    [Serializable]
    internal class Customer : ICustomer
    {
        public IMonthlyProductReport AddItem()
        {
            var monthlyProductReport = new MonthlyProductReport();
            MonthlyProductReportsList.Add(monthlyProductReport);
            return monthlyProductReport;
        }

        public void Remove(IMonthlyProductReport monthlyProductReport)
        {
            var existingElement =
                MonthlyProductReportsList.FirstOrDefault(r => ReferenceEquals(r, monthlyProductReport));

            if (existingElement != null)
            {
                MonthlyProductReportsList.Remove(existingElement);
            }
        }

        public string Name { get; set; } = "New Customer";

        [JsonIgnore] public IEnumerable<IMonthlyProductReport> MonthlyProductReports => MonthlyProductReportsList;

        public IList<MonthlyProductReport> MonthlyProductReportsList { get; } = new List<MonthlyProductReport>();
    }
}