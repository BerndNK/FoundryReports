using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel.Products
{
    /// <summary>
    /// Describes the usage trend of a single month for various products.
    /// </summary>
    public class TrendSegmentOfSingleMonthViewModel : IProductTrendMonthGraphSegment
    {
        public ObservableCollection<TrendSegmentOfSingleMonthOfOneProductViewModel> ProductSegments { get; }

        public Month Month { get; }

        public ObservableCollection<ITrendGraphSegment> TrendSegments { get; }

        public string MonthDisplay => string.Join(Environment.NewLine, Month.ToString().Substring(0, 3), Year);

        public int Year { get; }

        public bool IsThisMonth => DateTime.Today.Month == (int) Month;

        public IEnumerable<ProductViewModel> Products => ProductSegments.Select(t => t.ForProduct);

        public TrendSegmentOfSingleMonthViewModel(DateTime forMonth)
        {
            Month = (Month) forMonth.Month;
            Year = forMonth.Year;
            ProductSegments = new ObservableCollection<TrendSegmentOfSingleMonthOfOneProductViewModel>();
            TrendSegments = new ObservableCollectionWrapper<TrendSegmentOfSingleMonthOfOneProductViewModel, ITrendGraphSegment>(ProductSegments);
        }

        public void SetVisibility(ProductViewModel ofProduct, bool isVisible)
        {
            var trend = ProductSegments.FirstOrDefault(t => t.ForProduct == ofProduct);
            if (trend == null)
                return;

            trend.IsVisible = isVisible;
        }
    }
}