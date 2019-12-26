using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;

namespace FoundryReports.Core.Reports.Visualization
{
    internal class ProductTrend : List<IMonthlyProductReport>, IProductTrend
    {
        public DateTime Start => this.Any() ? this.Min(r => r.ForMonth) : DateTime.MaxValue;

        public DateTime End => this.Any() ? this.Max(r => r.ForMonth) : DateTime.MinValue;

        public IProduct Product { get; }

        public decimal MinUsage=> this.Any() ? this.Min(r => r.Value) : decimal.MaxValue;

        public decimal MaxUsage=> this.Any() ? this.Max(r => r.Value) : decimal.MinValue;

        public ProductTrend(IProduct product, IEnumerable<IMonthlyProductReport> reports) : base(reports)
        {
            Product = product;
        }

        public ProductTrend(IProduct product) : this(product, Enumerable.Empty<IMonthlyProductReport>())
        {
        }
    }
}