using FoundryReports.Core.Source.Prediction;

namespace FoundryReports.ViewModel.Products
{
    public class ProductEventViewModel : BaseViewModel
    {
        private readonly IProductEvent _productEvent;
        public string Name => _productEvent.GetType().Name;

        public ProductEventViewModel(IProductEvent productEvent)
        {
            _productEvent = productEvent;
        }
    }
}