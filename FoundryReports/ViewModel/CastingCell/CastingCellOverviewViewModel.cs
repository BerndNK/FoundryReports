using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using FoundryReports.Core.Utils;
using FoundryReports.ViewModel.DataManage;

namespace FoundryReports.ViewModel.CastingCell
{
    public class CastingCellOverviewViewModel : BaseViewModel
    {
        private readonly IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>>
            _manuallyChangedProductUsages;

        public ObservableCollection<CustomerViewModel> Customer { get; }
        private CustomerViewModel? _selectedCustomer;

        public CustomerViewModel? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                LoadTrend();
                UpdateSelectedTrendDescription();
            }
        }

        private bool _isBusy;

        private readonly ObservableCollection<ProductViewModel> _products;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        private string _selectedSegmentDescription = string.Empty;

        public string SelectedSegmentDescription
        {
            get => _selectedSegmentDescription;
            set
            {
                _selectedSegmentDescription = value;
                OnPropertyChanged();
            }
        }

        public CastingCellUsageTrendViewModel CastingCellUsageTrendViewModel { get; } =
            new CastingCellUsageTrendViewModel();

        public CastingCellOverviewViewModel(ObservableCollection<CustomerViewModel> customer,
            ObservableCollection<ProductViewModel> products,
            IDictionary<CustomerViewModel, HashSet<MonthlyProductUsageViewModel>> manuallyChangedProductUsages)
        {
            Customer = customer;
            _products = products;
            _manuallyChangedProductUsages = manuallyChangedProductUsages;
            CastingCellUsageTrendViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(CastingCellUsageTrendViewModel.SelectedSegment))
                {
                    UpdateSelectedTrendDescription();
                }
            };

            CastingCellUsageTrendViewModel.ValuesChanged += (sender, args) =>
            {
                UpdateSelectedTrendDescription();
            };
        }

        public async void LoadTrend()
        {
            if (SelectedCustomer == null)
                return;

            IsBusy = true;

            if (!_manuallyChangedProductUsages.TryGetValue(SelectedCustomer!,
                out var previouslyChangedReportsForThisCustomer))
                previouslyChangedReportsForThisCustomer = new HashSet<MonthlyProductUsageViewModel>();

            await CastingCellUsageTrendViewModel.LoadSegments(SelectedCustomer!, _products,
                previouslyChangedReportsForThisCustomer);


            IsBusy = false;
        }

        private void UpdateSelectedTrendDescription()
        {
            var sb = new StringBuilder();
            var selectedSegment = CastingCellUsageTrendViewModel.SelectedProductSegment;
            if (selectedSegment == null)
            {
                SelectedSegmentDescription = string.Empty;
                return;
            }

            sb.AppendLine($"In {selectedSegment.MonthDisplay.Replace(Environment.NewLine, "")}");
            sb.AppendLine($"The casting cell usage of {((int)selectedSegment.TrendSegments.First().Value).AsFormattedString()} is calculated as follows:");

            var calculation = selectedSegment.CalculationDescription();
            sb.AppendLine(calculation);

            SelectedSegmentDescription = sb.ToString();
        }
    }
}