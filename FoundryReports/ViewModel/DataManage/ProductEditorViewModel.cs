using System.Threading.Tasks;
using FoundryReports.Core.Source;

namespace FoundryReports.ViewModel.DataManage
{
    public class ProductEditorViewModel
    {
        private readonly IToolSource _toolSource;

        public ProductEditorViewModel(IToolSource toolSource)
        {
            _toolSource = toolSource;
        }

        public Task Load()
        {
            return Task.CompletedTask;
        }
    }
}
