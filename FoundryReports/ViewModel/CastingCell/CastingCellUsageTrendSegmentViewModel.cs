using System.Collections.ObjectModel;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;
using FoundryReports.ViewModel.Products;

namespace FoundryReports.ViewModel.CastingCell
{
    public class CastingCellUsageTrendSegmentViewModel : IProductTrendMonthGraphSegment
    {
        private readonly TrendSegmentOfSingleMonthViewModel _wrappedProductSegment;

        public CastingCellUsageTrendSegmentViewModel(TrendSegmentOfSingleMonthViewModel segment,
            ObservableCollection<ProductViewModel> products)
        {
            _wrappedProductSegment = segment;
            TrendSegments = new ObservableCollection<ITrendGraphSegment>();

            // this type of graph only displayed one line, which is added and created here.
            _castingCellUsageSegment = new SingleCastingCellTrendSegment(segment);
            TrendSegments.Add(_castingCellUsageSegment);
        }

        public ObservableCollection<ITrendGraphSegment> TrendSegments { get; }

        private readonly SingleCastingCellTrendSegment _castingCellUsageSegment;

        public decimal MinUsage
        {
            get => _castingCellUsageSegment.MinUsage;
            set => _castingCellUsageSegment.MinUsage = value;
        }

        public decimal MaxUsage
        {
            get => _castingCellUsageSegment.MaxUsage;
            set => _castingCellUsageSegment.MaxUsage = value;
        }

        public string MonthDisplay => _wrappedProductSegment.MonthDisplay;

        public bool IsThisMonth => _wrappedProductSegment.IsThisMonth;

        public void SetVisibility(ProductViewModel ofProduct, bool isVisible)
        {
            _wrappedProductSegment.SetVisibility(ofProduct, isVisible);
            CalculateCastingCellUsage();
        }

        private void CalculateCastingCellUsage()
        {
            _castingCellUsageSegment.CalculateCastingCellUsages();
        }

        public string CalculationDescription()
        {
            return _castingCellUsageSegment.CalculationDescription();
        }
    }
}