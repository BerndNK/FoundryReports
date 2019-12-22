using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.Core.Products;

namespace FoundryReports.ViewModel.DataManage
{
    public class MoldRequirementViewModel : BaseViewModel
    {
        public IMoldRequirement MoldRequirement { get; }

        public ObservableCollection<MoldViewModel> AvailableMolds { get; }
        
        private MoldViewModel? _selectedMold;

        public MoldViewModel? SelectedMold
        {
            get => _selectedMold;
            set
            {
                _selectedMold = value;
                if (value != null)
                {
                    MoldRequirement.Mold = value.Mold;
                }

                OnPropertyChanged();
            }
        }

        public decimal UsageAmount
        {
            get => MoldRequirement.UsageAmount;
            set
            {
                MoldRequirement.UsageAmount = value;
                OnPropertyChanged();
            }
        }

        public MoldRequirementViewModel(IMoldRequirement moldRequirement,
            ObservableCollection<MoldViewModel> availableMolds)
        {
            MoldRequirement = moldRequirement;
            AvailableMolds = availableMolds;
            SelectedMold = AvailableMolds.FirstOrDefault(m => m.Name == MoldRequirement.Mold.Name);
        }
    }
}