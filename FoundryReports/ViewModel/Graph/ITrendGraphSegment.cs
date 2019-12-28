using System.ComponentModel;
using System.Windows.Media;

namespace FoundryReports.ViewModel.Graph
{
    public interface ITrendGraphSegment : INotifyPropertyChanged
    {
        double LineSegmentStartX { get; }
        double LineSegmentStartY { get; }
        double LineSegmentCenterX { get; }
        double LineSegmentCenterY { get; }
        double LineSegmentEndX { get; }
        double LineSegmentEndY { get; }
        bool IsSegmentBeforeDotted { get; }
        bool IsSegmentAfterDotted { get; }
        Color StrokeColor { get; }
        bool IsVisible { get; set; }
        bool HasEvent { get; set; }

        decimal Value { get; }

        string DisplayValue { get; }
    }
}