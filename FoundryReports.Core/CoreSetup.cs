using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Source;
using FoundryReports.Core.Source.Prediction;

namespace FoundryReports.Core
{
    public class CoreSetup
    {
        public IDataSource DataSource { get; }

        public IEventPredictor EventPredictor { get; }

        public CoreSetup()
        {
            DataSource = new JsonDataSource();
            EventPredictor = new EventPredictor(DataSource);
        }
    }

    public class EventPredictor : IEventPredictor
    {
        private readonly IDataSource _dataSource;

        public EventPredictor(IDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public IEnumerable<IProductEvent> PredictEvents(IEnumerable<IMonthlyProductReport> forReports)
        {
            var molds = _dataSource.Molds.Select(MoldUsageCounter).ToList();

            var relevantReports = forReports.Where(r => r.ForMonth > DateTime.Today);
            foreach (var report in relevantReports)
            {
                var product = report.ForProduct;
                var amount = report.Value;
                foreach (var moldRequirement in product.MoldRequirements)
                {
                    var moldCounter = molds.FirstOrDefault(c => c.ForMold == moldRequirement.Mold);
                    if (moldCounter == null)
                        continue;

                    moldCounter.CurrentUsages += moldRequirement.UsageAmount * amount;
                    if (moldCounter.DidExceedMax)
                    {
                        yield return new MoldReachedEndOfLifetimeEvent(moldRequirement.Mold, product, report);
                        molds.Remove(moldCounter); // remove counter to ensure to only show events once for each mold
                    }
                }
            }
        }

        private MoldUsageCounter MoldUsageCounter(IMold forMold)
        {
            return new MoldUsageCounter(forMold);
        }
    }

    public class MoldReachedEndOfLifetimeEvent : IProductEvent
    {
        public MoldReachedEndOfLifetimeEvent(IMold moldRequirementMold, IProduct product, IMonthlyProductReport report)
        {
            ForReport = report;
        }

        public IMonthlyProductReport ForReport { get; }
    }

    internal class MoldUsageCounter
    {
        public IMold ForMold { get; }

        public decimal MaxUsages { get; }

        public decimal CurrentUsages { get; set; }

        public bool DidExceedMax => CurrentUsages > MaxUsages;

        public MoldUsageCounter(IMold forMold)
        {
            ForMold = forMold;
            MaxUsages = forMold.MaxUsages;
            CurrentUsages = forMold.CurrentUsages;
        }
    }
}