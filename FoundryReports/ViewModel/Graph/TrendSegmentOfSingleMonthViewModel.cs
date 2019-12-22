using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    /// <summary>
    /// Describes the usage trend of a single month for various products.
    /// </summary>
    public class TrendSegmentOfSingleMonthViewModel
    {
        public ObservableCollection<TrendSegmentOfSingleMonthOfOneProductViewModel> ProductTrends { get; } = new ObservableCollection<TrendSegmentOfSingleMonthOfOneProductViewModel>();

        public Month Month { get; }

        public string MonthDisplay => string.Join(Environment.NewLine, Month.ToString().Substring(0, 3), Year);

        public int Year { get; }

        public bool IsThisMonth => DateTime.Today.Month == (int) Month;

        public IEnumerable<ProductViewModel> Products => ProductTrends.Select(t => t.ForProduct);

        public TrendSegmentOfSingleMonthViewModel(DateTime forMonth)
        {
            Month = (Month) forMonth.Month;
            Year = forMonth.Year;
        }

        public void SetVisibility(ProductViewModel ofProduct, bool isVisible)
        {
            var trend = ProductTrends.FirstOrDefault(t => t.ForProduct == ofProduct);
            if (trend == null)
                return;

            trend.IsVisible = isVisible;
        }
    }
}