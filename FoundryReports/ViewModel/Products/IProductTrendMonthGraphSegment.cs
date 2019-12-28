using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel.Products
{
    public interface IProductTrendMonthGraphSegment : IMonthGraphSegment
    {
        void SetVisibility(ProductViewModel ofProduct, bool isVisible);
    }
}