using System.Windows.Input;
using System.Windows.Media;
using FoundryReports.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Products
{
    public class ProductSelectionViewModel : BaseViewModel
    {
        public string ProductName => $"Product {Product.Name}";

        private bool _isSelected = true;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public ProductViewModel Product { get; }

        public ICommand ToggleCommand { get; }

        public ProductSelectionViewModel(ProductViewModel product)
        {
            Product = product;
            Product.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(ProductViewModel.Name))
                    OnPropertyChanged(nameof(ProductName));
            };

            ToggleCommand = new DelegateCommand(ToggleIsSelected);
        }

        private void ToggleIsSelected()
        {
            IsSelected = !IsSelected;
        }

        public Color FontColor
        {
            get
            {
                var productNameHash = Product.Name.StableHashCode(); // use stable hash, so the color remains the same after application restart
                return Color.FromRgb((byte) productNameHash, (byte) (productNameHash>>8), (byte) (productNameHash>>16));
            }
        }
    }
}
