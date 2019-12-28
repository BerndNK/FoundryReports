using System;
using System.ComponentModel;
using System.Windows.Media;
using FoundryReports.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    /// <summary>
    /// Describes the usage trend of a single month for one specific product
    /// </summary>
    public class TrendSegmentOfSingleMonthOfOneProductViewModel : BaseViewModel
    {
        public ProductViewModel ForProduct { get; set; }

        public MonthlyProductUsageViewModel PreviousMonth { get; set; }

        public MonthlyProductUsageViewModel CurrentMonth { get; set; }

        public MonthlyProductUsageViewModel NextMonth { get; set; }

        public decimal MaxUsage { get; set; }

        public decimal MinUsage { get; set; }

        public double DisplayHeight { get; set; } = 200;

        public double DisplayWidth { get; set; } = 50;

        public double PreviousMonthYPosInPercent => MiddleValueBetween(PreviousMonth, CurrentMonth);

        public double CurrentMonthYPosInPercent => UsageValueInPercent(CurrentMonth.Value);

        public double NextMonthYPosInPercent => MiddleValueBetween(CurrentMonth, NextMonth);

        public double LineSegmentStartX => 0;
        public double LineSegmentStartY => Math.Ceiling(DisplayHeight * (1.0 - PreviousMonthYPosInPercent));

        public double LineSegmentCenterX => DisplayWidth / 2.0;
        public double LineSegmentCenterY => DisplayHeight * (1.0 - CurrentMonthYPosInPercent);

        public double LineSegmentEndX => DisplayWidth;
        public double LineSegmentEndY => Math.Ceiling(DisplayHeight * (1.0 - NextMonthYPosInPercent));

        public bool IsSegmentBeforeDotted => CurrentMonth.MonthlyProductReport.IsPredicted ||
                                             PreviousMonth == CurrentMonth;

        public bool IsSegmentAfterDotted => CurrentMonth.MonthlyProductReport.IsPredicted ||
                                            NextMonth.MonthlyProductReport.IsPredicted || 
                                            NextMonth == CurrentMonth;

        public Color StrokeColor
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

        private bool _isVisible = true;

        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged();
            }
        }

        private bool _hasEvent;

        public bool HasEvent
        {
            get => _hasEvent;
            set
            {
                _hasEvent = value;
                OnPropertyChanged();
            }
        }

        private double MiddleValueBetween(MonthlyProductUsageViewModel monthA, MonthlyProductUsageViewModel monthB)
        {
            var aInPercent = UsageValueInPercent(monthA.Value);
            var bInPercent = UsageValueInPercent(monthB.Value);

            return (aInPercent + bInPercent) / 2;
        }

        private double UsageValueInPercent(decimal usageInPercent)
        {
            return ((double) usageInPercent - (double) MinUsage) / (double) (MaxUsage - MinUsage);
        }

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
                    break;
                case nameof(MonthlyProductUsageViewModel.IsPredicted):
                    OnPropertyChanged(nameof(IsSegmentBeforeDotted));
                    OnPropertyChanged(nameof(IsSegmentAfterDotted));
                    break;
            }
        }
    }
}