using System;
using System.Windows.Media;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel.Products
{
    public abstract class BaseTrendGraphSegment : BaseViewModel, ITrendGraphSegment
    {
        public double DisplayHeight { get; set; } = 200;

        public double DisplayWidth { get; set; } = 50;

        public double LineSegmentStartX => 0;

        public double LineSegmentStartY => Math.Ceiling(DisplayHeight * (1.0 - PreviousMonthYPosInPercent()));
        protected abstract double PreviousMonthYPosInPercent();

        public double LineSegmentCenterX => DisplayWidth / 2.0;

        public double LineSegmentCenterY => DisplayHeight * (1.0 - CurrentMonthYPosInPercent());
        protected abstract double CurrentMonthYPosInPercent();

        public double LineSegmentEndX => DisplayWidth;

        public double LineSegmentEndY => Math.Ceiling(DisplayHeight * (1.0 - NextMonthYPosInPercent()));

        public abstract bool IsSegmentBeforeDotted { get; }

        public abstract bool IsSegmentAfterDotted { get; }

        public abstract Color StrokeColor { get; }

        protected abstract double NextMonthYPosInPercent();

        private decimal _maxUsage;

        public decimal MaxUsage
        {
            get => _maxUsage;
            set
            {
                _maxUsage = value;
                OnPropertyChanged(nameof(LineSegmentStartY));
                OnPropertyChanged(nameof(LineSegmentCenterY));
                OnPropertyChanged(nameof(LineSegmentEndY));
            }
        }

        private decimal _minUsage;

        public decimal MinUsage
        {
            get => _minUsage;
            set
            {
                _minUsage = value;
                OnPropertyChanged(nameof(LineSegmentStartY));
                OnPropertyChanged(nameof(LineSegmentCenterY));
                OnPropertyChanged(nameof(LineSegmentEndY));
            }

        }

        public abstract decimal Value { get; }

        public abstract string DisplayValue { get; }

        protected double MiddleValueBetween(decimal monthA, decimal monthB)
        {
            var aInPercent = UsageValueInPercent(monthA);
            var bInPercent = UsageValueInPercent(monthB);

            return (aInPercent + bInPercent) / 2;
        }

        protected double UsageValueInPercent(decimal usageInPercent)
        {
            return ((double) usageInPercent - (double) MinUsage) / (double) (MaxUsage - MinUsage);
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
    }
}