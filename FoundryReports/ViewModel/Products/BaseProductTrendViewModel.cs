using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using FoundryReports.Core.Utils;
using FoundryReports.ViewModel.DataManage;
using FoundryReports.ViewModel.Graph;

namespace FoundryReports.ViewModel.Products
{
    public abstract class BaseProductTrendViewModel<T> : BaseViewModel, IGraphViewModel
        where T : class, IProductTrendMonthGraphSegment
    {
        public ObservableCollection<T> MonthlyProductSegments { get; }

        public ObservableCollection<IMonthGraphSegment> MonthlySegments { get; }

        public ObservableCollection<ProductSelectionViewModel> ProductSelection { get; } =
            new ObservableCollection<ProductSelectionViewModel>();

        public ObservableCollection<string> YAxisDescriptions { get; } = new ObservableCollection<string>();

        public T? SelectedProductSegment { get; private set; }

        public IMonthGraphSegment? SelectedSegment
        {
            get => SelectedProductSegment;
            set
            {
                SelectedProductSegment = value as T;
                UpdateSelectedSegmentRelevantProperties();
                OnPropertyChanged();
            }
        }

        protected BaseProductTrendViewModel()
        {
            MonthlyProductSegments = new ObservableCollection<T>();
            MonthlySegments = new ObservableCollectionWrapper<T, IMonthGraphSegment>(MonthlyProductSegments);
        }

        protected virtual void UpdateSelectedSegmentRelevantProperties()
        {
        }

        protected bool IsVisible(MonthlyProductUsageViewModel monthlyProductUsageViewModel)
        {
            var correspondingSelection =
                ProductSelection.FirstOrDefault(s => s.Product == monthlyProductUsageViewModel.SelectedProduct);
            if (correspondingSelection == null)
                return true;

            return correspondingSelection.IsSelected;
        }

        protected void ProductSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProductSelectionViewModel.IsSelected))
                return;

            if (sender is ProductSelectionViewModel asProductSelectionViewModel)
            {
                var productViewModel = asProductSelectionViewModel.Product;
                foreach (var monthSegment in MonthlyProductSegments)
                {
                    monthSegment.SetVisibility(productViewModel, asProductSelectionViewModel.IsSelected);
                }

                UpdatesAfterVisibilityChanged();
                UpdateSelectedSegmentRelevantProperties();
            }
        }

        protected abstract void UpdatesAfterVisibilityChanged();

        protected void CreateYAxisDescriptions(in decimal minUsage, in decimal maxUsage)
        {
            YAxisDescriptions.Clear();
            var intervals = NumberVisualization.DisplayRange((double) minUsage, (double) maxUsage, 4);
            foreach (var interval in intervals.Reverse()
            ) // the numbers go from top to bottom, while in the interval list its reversed.
            {
                YAxisDescriptions.Add(interval);
            }
        }
    }
}