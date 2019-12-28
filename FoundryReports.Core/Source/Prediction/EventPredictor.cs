using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Source.Prediction
{
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
                        yield return new MoldReachedEndOfLifetimeEvent(moldRequirement, product, report, moldCounter.CurrentUsages);
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
}