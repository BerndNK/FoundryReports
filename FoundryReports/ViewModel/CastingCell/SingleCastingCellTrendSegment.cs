using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using FoundryReports.Core.Utils;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Products;

namespace FoundryReports.ViewModel.CastingCell
{
    public class SingleCastingCellTrendSegment : BaseTrendGraphSegment
    {
        private readonly TrendSegmentOfSingleMonthViewModel _wrappedSegment;
        private decimal _previousMonthUsage;
        private decimal _currentMonthUsage;
        private decimal _nextMonthUsage;

        public SingleCastingCellTrendSegment(TrendSegmentOfSingleMonthViewModel wrappedSegment)
        {
            _wrappedSegment = wrappedSegment;
            CalculateCastingCellUsages();
        }

        private decimal CalculateCastingCellUsage(IEnumerable<MonthlyProductUsageViewModel> productUsages)
        {
            decimal castingCellUsage = 0;
            foreach (var monthlyProductUsageViewModel in productUsages)
            {
                var productViewModel = monthlyProductUsageViewModel.SelectedProduct;
                if (productViewModel == null)
                    continue;

                foreach (var moldRequirement in productViewModel.Product.MoldRequirements)
                {
                    castingCellUsage += moldRequirement.Mold.CastingCellAmount * moldRequirement.UsageAmount *
                                        monthlyProductUsageViewModel.Value;
                }
            }

            return castingCellUsage;
        }

        public void CalculateCastingCellUsages()
        {
            _previousMonthUsage = CalculateCastingCellUsage(_wrappedSegment.ProductSegments.Where(p => p.IsVisible)
                .Select(p => p.PreviousMonth));
            _currentMonthUsage = CalculateCastingCellUsage(_wrappedSegment.ProductSegments.Where(p => p.IsVisible)
                .Select(p => p.CurrentMonth));
            _nextMonthUsage =
                CalculateCastingCellUsage(_wrappedSegment.ProductSegments.Where(p => p.IsVisible)
                    .Select(p => p.NextMonth));
        }

        protected override double PreviousMonthYPosInPercent() =>
            MiddleValueBetween(_previousMonthUsage, _currentMonthUsage);

        protected override double CurrentMonthYPosInPercent() => UsageValueInPercent(_currentMonthUsage);

        protected override double NextMonthYPosInPercent() => MiddleValueBetween(_currentMonthUsage, _nextMonthUsage);

        public override bool IsSegmentBeforeDotted => false;

        public override bool IsSegmentAfterDotted => false;

        public override Color StrokeColor => Color.FromArgb(255, 125, 8, 0);

        public override decimal Value => _currentMonthUsage;

        public override string DisplayValue => _currentMonthUsage.ToString(",0.##");

        /// <summary>
        /// Returns a description of how the calculation of the current value was executed.
        /// </summary>
        public string CalculationDescription()
        {
            var sb = new StringBuilder();
            var productUsages = _wrappedSegment.ProductSegments.Where(p => p.IsVisible).Select(p => p.CurrentMonth);

            var shortCalculationSb = new StringBuilder();

            decimal castingCellUsage = 0;
            foreach (var monthlyProductUsageViewModel in productUsages)
            {
                decimal productOnlyUsage = 0;
                var productViewModel = monthlyProductUsageViewModel.SelectedProduct;
                if (productViewModel == null)
                    continue;

                sb.AppendLine();

                if (monthlyProductUsageViewModel.Value > 0)
                    sb.AppendLine(
                        $"Product {productViewModel.Product.Name}, being produced {((int) monthlyProductUsageViewModel.Value).AsFormattedString()} times:");
                else
                    sb.AppendLine($"Product {productViewModel.Product.Name}, is not produced. (0 usages)");

                var productSb = new StringBuilder();
                productSb.Append($"Product {productViewModel.Product.Name}: (");

                var isFirstMold = true;
                foreach (var moldRequirement in productViewModel.Product.MoldRequirements)
                {
                    var moldRequirementUsageAmount = moldRequirement.Mold.CastingCellAmount *
                                                     moldRequirement.UsageAmount * monthlyProductUsageViewModel.Value;
                    if (moldRequirementUsageAmount > 0)
                    {
                        sb.AppendLine(
                            $"Mold {moldRequirement.Mold.Name}, being used {moldRequirement.UsageAmount} times, needing {moldRequirement.Mold.CastingCellAmount} casting cell each time.");
                        productSb.Append((isFirstMold ? "" : " + ") +
                                         $"{moldRequirement.UsageAmount * moldRequirement.Mold.CastingCellAmount}");
                        isFirstMold = false;
                    }

                    productOnlyUsage += moldRequirementUsageAmount;
                    castingCellUsage += moldRequirementUsageAmount;
                }

                productSb.Append(
                    $") * {((int) monthlyProductUsageViewModel.Value).AsFormattedString()}  | = {((int) productOnlyUsage).AsFormattedString()}");

                if (productOnlyUsage > 0)
                {
                    shortCalculationSb.AppendLine(productSb.ToString());
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine($"Resulting in a usage of {castingCellUsage:0.##} casting cell(s).");
            sb.AppendLine();
            sb.AppendLine($"In short:");
            sb.AppendLine(shortCalculationSb.ToString());
            sb.AppendLine($"------------------");
            sb.AppendLine($"= {((int) castingCellUsage).AsFormattedString()}");

            return sb.ToString();
        }
    }
}