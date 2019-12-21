using System.Threading.Tasks;
using FoundryReports.Core;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public GraphMainViewModel Graph { get; } = new GraphMainViewModel();

        public DataManageMainViewModel DataManage { get; }

        public MainViewModel()
        {
            var setup = new CoreSetup();
            DataManage = new DataManageMainViewModel(setup.ToolSource);
        }
        
        public async Task Load()
        {
            await DataManage.Load();
        }
    }
}
