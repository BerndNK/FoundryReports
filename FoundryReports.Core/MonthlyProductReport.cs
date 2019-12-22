using System;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;
using Newtonsoft.Json;

namespace FoundryReports.Core
{
    [Serializable]
    internal class MonthlyProductReport : IMonthlyProductReport
    {
        public bool IsPredicted { get; set; }

        public DateTime ForMonth { get; set; } = DateTime.Today;

        public decimal Value { get; set; }

        private IProduct? _productInstance;

        public string ProductName { get; set; } = string.Empty;

        [JsonIgnore]
        public IProduct ForProduct
        {
            get => _productInstance ?? new ProductDummy {Name = ProductName};
            set
            {
                _productInstance = value;
                ProductName = value.Name;
            }
        }

        public MonthlyProductReport()
        {
        }

        public MonthlyProductReport(IProduct forProduct, decimal value)
        {
            ForProduct = forProduct;
            Value = value;
        }
    }
}