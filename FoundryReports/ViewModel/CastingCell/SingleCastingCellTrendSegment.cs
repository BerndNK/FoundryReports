using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
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
                    castingCellUsage += moldRequirement.UsageAmount * monthlyProductUsageViewModel.Value;
                }
            }

            return castingCellUsage;
        }

        public void CalculateCastingCellUsages()
        {
            _previousMonthUsage = CalculateCastingCellUsage(_wrappedSegment.ProductSegments.Where(p => p.IsVisible).Select(p => p.PreviousMonth));
            _currentMonthUsage = CalculateCastingCellUsage(_wrappedSegment.ProductSegments.Where(p => p.IsVisible).Select(p => p.CurrentMonth));
            _nextMonthUsage = CalculateCastingCellUsage(_wrappedSegment.ProductSegments.Where(p => p.IsVisible).Select(p => p.NextMonth));
        }

        protected override double PreviousMonthYPosInPercent() => MiddleValueBetween(_previousMonthUsage, _currentMonthUsage);

        protected override double CurrentMonthYPosInPercent() => UsageValueInPercent(_currentMonthUsage);

        protected override double NextMonthYPosInPercent() => MiddleValueBetween(_currentMonthUsage, _nextMonthUsage);

        public override bool IsSegmentBeforeDotted => false;

        public override bool IsSegmentAfterDotted => false;

        public override Color StrokeColor => Color.FromArgb(255, 125, 8, 0);

        public override decimal Value => _currentMonthUsage;

        public override string DisplayValue => _currentMonthUsage.ToString("0.##");
    }
}