using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.Graph
{
    public class TrendViewModel : BaseViewModel
    {
        public ObservableCollection<TrendSegmentOfSingleMonthViewModel> MonthlySegments { get; } = new ObservableCollection<TrendSegmentOfSingleMonthViewModel>();

        public ObservableCollection<ProductSelectionViewModel> ProductSelection { get; } = new ObservableCollection<ProductSelectionViewModel>();

        public ObservableCollection<MonthlyProductUsageViewModel> UsagesOfSelectedSegment { get; } = new ObservableCollection<MonthlyProductUsageViewModel>();

        private TrendSegmentOfSingleMonthViewModel? _selectedSegment;

        public TrendSegmentOfSingleMonthViewModel? SelectedSegment
        {
            get => _selectedSegment;
            set
            {
                _selectedSegment = value;
                UpdateUsageViewModelsOfSelectedSegment();
                OnPropertyChanged();
            }
        }

        private void UpdateUsageViewModelsOfSelectedSegment()
        {
            UsagesOfSelectedSegment.Clear();
            if (SelectedSegment == null)
                return;

            foreach (var usageOfProductInSelectedMonth in SelectedSegment.ProductTrends.Select(t => t.CurrentMonth))
            {
                UsagesOfSelectedSegment.Add(usageOfProductInSelectedMonth);
            }
        }

        public async Task LoadSegments(CustomerViewModel customer, ObservableCollection<ProductViewModel> products)
        {
            var factory = new TrendViewModelFactory();
            var segments = await Task.Run(() => factory.ProduceSegments(customer.Customer, new DateTime(2019, 1, 1), new DateTime(2020, 1, 1), products));

            MonthlySegments.Clear();
            ProductSelection.Clear();
            foreach (var segment in segments)
            {
                await Task.Delay(50);
                MonthlySegments.Add(segment);
            }

            var allProducts = MonthlySegments.SelectMany(m => m.Products).Distinct();
            foreach (var productViewModel in allProducts)
            {
                var selection = new ProductSelectionViewModel(productViewModel);
                selection.PropertyChanged += ProductSelectionOnPropertyChanged;
                ProductSelection.Add(selection);
                await Task.Delay(50);
            }
        }

        private void ProductSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(ProductSelectionViewModel.IsSelected))
                return;

            if(sender is ProductSelectionViewModel asProductSelectionViewModel)
            {
                var productViewModel = asProductSelectionViewModel.Product;
                foreach (var monthSegment in MonthlySegments)
                {
                    monthSegment.SetVisibility(productViewModel, asProductSelectionViewModel.IsSelected);
                }
            }

        }
    }
}
