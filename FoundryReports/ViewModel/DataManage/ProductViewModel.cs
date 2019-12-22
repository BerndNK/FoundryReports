using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using FoundryReports.Core.Products;
using FoundryReports.Utils;

namespace FoundryReports.ViewModel.DataManage
{
    public class ProductViewModel : BaseViewModel
    {
        private readonly ObservableCollection<MoldViewModel> _availableMolds;

        public IProduct Product { get; }

        public ObservableCollection<MoldRequirementViewModel> MoldRequirements { get; set; }

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }
        
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
            AddItemCommand = new DelegateCommand(() => AddItem());
            RemoveItemCommand = new DelegateCommand<MoldRequirementViewModel>(RemoveItem);
            Product = product;

            MoldRequirements = new ObservableCollection<MoldRequirementViewModel>(product.MoldRequirements.Select(r => new MoldRequirementViewModel(r, availableMolds)));
        }
        
        public MoldRequirementViewModel AddItem()
        {
            var newRequirement = Product.AddMoldRequirement();
            var moldRequirementViewModel = new MoldRequirementViewModel(newRequirement, _availableMolds);
            MoldRequirements.Add(moldRequirementViewModel);

            return moldRequirementViewModel;
        }

        private void RemoveItem(MoldRequirementViewModel? moldRequirementViewModel)
        {
            if (moldRequirementViewModel == null)
                return;

            if(MoldRequirements.Remove(moldRequirementViewModel))
            {
                Product.Remove(moldRequirementViewModel.MoldRequirement);
            }
        }
    }
}