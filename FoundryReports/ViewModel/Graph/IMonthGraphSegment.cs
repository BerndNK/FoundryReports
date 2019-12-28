using System.Collections.ObjectModel;

namespace FoundryReports.ViewModel.Graph
{
    public interface IMonthGraphSegment
    {
        ObservableCollection<ITrendGraphSegment> TrendSegments { get; }
        string MonthDisplay { get; }
        bool IsThisMonth { get; }
    }
}