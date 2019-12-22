using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.Core.Products;

namespace FoundryReports.ViewModel.DataManage
{
    public class ProductViewModel : ListViewModel<MoldRequirementViewModel>
    {
        private readonly ObservableCollection<MoldViewModel> _availableMolds;

        public IProduct Product { get; }
        
        public string Name
        {
            get => Product.Name;
            set
            {
                Product.Name = value;
                OnPropertyChanged();
            }
        }

        public ProductViewModel(IProduct product, ObservableCollection<MoldViewModel> availableMolds)
        {
            _availableMolds = availableMolds;
            Product = product;

            foreach (var moldRequirementViewModel in product.MoldRequirements.Select(r => new MoldRequirementViewModel(r, availableMolds)))
            {
                Children.Add(moldRequirementViewModel);
            }
        }
        
        protected override MoldRequirementViewModel NewViewModel()
        {
            var newRequirement = Product.AddItem();
            return new MoldRequirementViewModel(newRequirement, _availableMolds);
        }

        protected override void RemoveFromModel(MoldRequirementViewModel viewModel)
        {
            Product.Remove(viewModel.MoldRequirement);
        }
    }
}