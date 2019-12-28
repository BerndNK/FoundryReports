using System.ComponentModel;
using System.Windows.Media;
using FoundryReports.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Products
{
    /// <summary>
    /// Describes the usage trend of a single month for one specific product
    /// </summary>
    public class TrendSegmentOfSingleMonthOfOneProductViewModel : BaseTrendGraphSegment
    {
        public ProductViewModel ForProduct { get; set; }

        public MonthlyProductUsageViewModel PreviousMonth { get; set; }

        public MonthlyProductUsageViewModel CurrentMonth { get; set; }

        public MonthlyProductUsageViewModel NextMonth { get; set; }

        protected override double PreviousMonthYPosInPercent() => MiddleValueBetween(PreviousMonth.Value, CurrentMonth.Value);

        protected override double CurrentMonthYPosInPercent() => UsageValueInPercent(CurrentMonth.Value);

        protected override double NextMonthYPosInPercent() => MiddleValueBetween(CurrentMonth.Value, NextMonth.Value);

        public override bool IsSegmentBeforeDotted => CurrentMonth.MonthlyProductReport.IsPredicted ||
                                                      PreviousMonth == CurrentMonth;

        public override bool IsSegmentAfterDotted => CurrentMonth.MonthlyProductReport.IsPredicted ||
                                                     NextMonth.MonthlyProductReport.IsPredicted ||
                                                     NextMonth == CurrentMonth;

        public override Color StrokeColor
        {
            get
            {
                var productNameHash =
                    ForProduct.Name
                        .StableHashCode(); // use stable hash, so the color remains the same after application restart
                return Color.FromRgb((byte) productNameHash, (byte) (productNameHash >> 8),
                    (byte) (productNameHash >> 16));
            }
        }

        public override decimal Value => CurrentMonth.Value;

        public override string DisplayValue => CurrentMonth.Value.ToString("0.##");

        public TrendSegmentOfSingleMonthOfOneProductViewModel(MonthlyProductUsageViewModel previousMonth,
            MonthlyProductUsageViewModel currentMonth, MonthlyProductUsageViewModel nextMonth, decimal minUsage,
            decimal maxUsage)
        {
            // instance will not be null, when produced by factory as the factory ensures to only use instances with valid products
            PreviousMonth = previousMonth;
            CurrentMonth = currentMonth;
            NextMonth = nextMonth;
            ForProduct = CurrentMonth.SelectedProduct!;
            MinUsage = minUsage;
            MaxUsage = maxUsage;

            previousMonth.PropertyChanged += MonthlyProductUsageViewModelOnPropertyChanged;
            currentMonth.PropertyChanged += MonthlyProductUsageViewModelOnPropertyChanged;
            nextMonth.PropertyChanged += MonthlyProductUsageViewModelOnPropertyChanged;
        }

        private void MonthlyProductUsageViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(MonthlyProductUsageViewModel.Value):
                    OnPropertyChanged(nameof(LineSegmentStartY));
                    OnPropertyChanged(nameof(LineSegmentCenterY));
                    OnPropertyChanged(nameof(LineSegmentEndY));
                    OnPropertyChanged(nameof(DisplayValue));
                    break;
                case nameof(MonthlyProductUsageViewModel.IsPredicted):
                    OnPropertyChanged(nameof(IsSegmentBeforeDotted));
                    OnPropertyChanged(nameof(IsSegmentAfterDotted));
                    break;
            }
        }
    }
}