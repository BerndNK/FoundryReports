using System.Collections.ObjectModel;

namespace FoundryReports.ViewModel.Graph
{
    public interface IGraphViewModel
    {
        ObservableCollection<IMonthGraphSegment> MonthlySegments { get; }

        IMonthGraphSegment? SelectedSegment { get; set; }

        ObservableCollection<string> YAxisDescriptions { get; }
    }
}