using System.Text;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.Core.Source.Prediction
{
    public class MoldReachedEndOfLifetimeEvent : IProductEvent
    {
        private readonly IMoldRequirement _moldRequirement;
        private readonly IProduct _product;
        private readonly decimal _moldUsagesWhenExceeded;

        public MoldReachedEndOfLifetimeEvent(IMoldRequirement moldRequirement, IProduct product, IMonthlyProductReport report, in decimal moldCounterCurrentUsages)
        {
            _moldRequirement = moldRequirement;
            _product = product;
            ForReport = report;
            _moldUsagesWhenExceeded = moldCounterCurrentUsages;
        }

        public IMonthlyProductReport ForReport { get; }

        public string DisplayName => $"Mold \"{_moldRequirement.Mold.Name}\" expired";

        public string Description
        {
            get
            {
                var sb = new StringBuilder($"{ForReport.ForMonth:M}: ");
                if (ForReport.IsPredicted)
                    sb.Append("With the predicted monthly usage of ");
                else
                    sb.Append("With the monthly usage of ");

                sb.Append($"{ForReport.Value:0.##} units, ");
                sb.AppendLine();
                sb.AppendLine($"the product \"{_product.Name}\" will expire the mold \"{_moldRequirement.Mold.Name}\"");
                sb.AppendLine($"Exceeding the capacity by {(_moldUsagesWhenExceeded - _moldRequirement.Mold.MaxUsages):0} usages.");
                sb.AppendLine();
                sb.AppendLine($"Mold \"{_moldRequirement.Mold.Name}\" usages as of today: {_moldRequirement.Mold.CurrentUsages}/{_moldRequirement.Mold.MaxUsages}");
                sb.AppendLine($"Product \"{_product.Name}\" uses mold \"{_moldRequirement.Mold.Name}\" {_moldRequirement.UsageAmount} time(s).");

                return sb.ToString();
            }
        }
    }
}