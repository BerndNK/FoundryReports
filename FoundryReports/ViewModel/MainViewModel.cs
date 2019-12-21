using FoundryReports.Core;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public GraphMainViewModel Graph { get; } = new GraphMainViewModel();

        public DataManageMainViewModel DataManage { get; } = new DataManageMainViewModel();

        public MainViewModel()
        {
            var setup = new CoreSetup();

        }

    }
}
