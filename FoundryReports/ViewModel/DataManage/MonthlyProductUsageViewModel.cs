using System;
using System.Collections.ObjectModel;
using System.Linq;
using FoundryReports.Core.Reports.Visualization;

namespace FoundryReports.ViewModel.DataManage
{
    public class MonthlyProductUsageViewModel : BaseViewModel
    {
        public IMonthlyProductReport MonthlyProductReport { get; }

        public ObservableCollection<ProductViewModel> AvailableProducts { get; }

        private ProductViewModel? _selectedProduct;

        public ProductViewModel? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                if (value != null)
                {
                    MonthlyProductReport.ForProduct = value.Product;
                }

                OnPropertyChanged();
            }
        }

        public bool IsPredicted
        {
            get => MonthlyProductReport.IsPredicted;
            set
            {
                MonthlyProductReport.IsPredicted = value;
                OnPropertyChanged();
            }
        }

        public decimal Value
        {
            get => MonthlyProductReport.Value;
            set
            {
                MonthlyProductReport.Value = value;
                OnPropertyChanged();
            }
        }

        public DateTime ForMonth
        {
            get => MonthlyProductReport.ForMonth;
            set
            {
                MonthlyProductReport.ForMonth = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Year));
                OnPropertyChanged(nameof(Month));
            }
        }

        public int Year
        {
            get => ForMonth.Year;
            set
            {
                MonthlyProductReport.ForMonth = new DateTime(value, ForMonth.Month, 1);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ForMonth));
            }
        }

        public Month Month
        {
            get => (Month) ForMonth.Month;
            set
            {
                MonthlyProductReport.ForMonth = new DateTime(ForMonth.Year, (int) value, 1);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ForMonth));
            }
        }

        public MonthlyProductUsageViewModel(IMonthlyProductReport monthlyProductReport,
            ObservableCollection<ProductViewModel> availableProducts)
        {
            MonthlyProductReport = monthlyProductReport;
            AvailableProducts = availableProducts;
            SelectedProduct = AvailableProducts.FirstOrDefault(p => p.Name == MonthlyProductReport.ForProduct.Name);
        }
    }
}