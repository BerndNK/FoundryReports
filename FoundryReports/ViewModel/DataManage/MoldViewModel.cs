using FoundryReports.Core.Products;

namespace FoundryReports.ViewModel.DataManage
{
    public class MoldViewModel : BaseViewModel
    {
        public IMold Mold { get; }

        public string Name
        {
            get => Mold.Name;
            set
            {
                Mold.Name = value;
                OnPropertyChanged();
            }
        }

        public int MaxUsages
        {
            get => Mold.MaxUsages;
            set
            {
                Mold.MaxUsages = value;
                OnPropertyChanged();
            }
        }

        public int CurrentUsages
        {
            get => Mold.CurrentUsages;
            set
            {
                Mold.CurrentUsages = value;
                OnPropertyChanged();
            }
        }

        public decimal CastingCellAmount
        {
            get => Mold.CastingCellAmount;
            set
            {
                Mold.CastingCellAmount = value;
                OnPropertyChanged();
            }
        }

        public MoldViewModel(IMold mold)
        {
            Mold = mold;
        }
    }
}