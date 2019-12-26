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

        private string _productName = "[Unknown]";

        public string ProductName
        {
            get => _productName;
            set
            {
                _productName = value;
                _productDummy.Name = _productName;
            }
        }

        private ProductDummy _productDummy = new ProductDummy();

        [JsonIgnore]
        public IProduct ForProduct
        {
            get => _productInstance ?? _productDummy;
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

        public static IMonthlyProductReport Zero(IProduct forProduct) => new MonthlyProductReport(forProduct, 0);
    }
}