using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FoundryReports.Core.Source;
using NUnit.Framework;

namespace FoundryReports.Core.Test.Source
{
    public class Tests
    {
        private string TestDataPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Source\\TestData\\");

        private string ProductTestCsvPath => Path.Combine(TestDataPath, "Products.csv");

        private string MoldsTestCsvPath => Path.Combine(TestDataPath, "Molds.csv");

        private string CustomerTestCsvPath => Path.Combine(TestDataPath, "CustomerX.csv");

        private async Task<IEnumerable<T>> EnumerateAsync<T>(IAsyncEnumerable<T> asyncEnumerable)
        {
            var asList = new List<T>();
            await foreach (var item in asyncEnumerable)
            {
                asList.Add(item);
            }

            return asList;
        }

        [Test]
        public async Task ImportingProducts_ShouldResultInCorrectAmount()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var products = await EnumerateAsync(importer.ImportProductsFromCsv(ProductTestCsvPath));

            // Assert
            Assert.That(products.Count(), Is.EqualTo(14));
        }

        [Test]
        public async Task ImportingProducts_ShouldResultInCorrectAmountOfUsedMolds()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var products = await EnumerateAsync(importer.ImportProductsFromCsv(ProductTestCsvPath));

            // Assert
            Assert.That(products.First().MoldRequirements.Count(), Is.EqualTo(18));
        }

        [Test]
        public async Task ImportingProducts_ShouldResultInCorrectMoldNames()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var products = await EnumerateAsync(importer.ImportProductsFromCsv(ProductTestCsvPath));

            // Assert
            Assert.That(products.First().MoldRequirements.Select(x => x.Mold.Name),
                Is.EquivalentTo(new[]
                {
                    "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15",
                    "F16", "F17", "F18"
                }));
        }

        [Test]
        public async Task ImportingProducts_ShouldResultInCorrectMoldUsageAmounts()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var products = await EnumerateAsync(importer.ImportProductsFromCsv(ProductTestCsvPath));

            // Assert
            Assert.That(products.First().MoldRequirements.Select(x => x.UsageAmount),
                Is.EquivalentTo(new[]
                    {3,1.3,0.5,0,0,0,2,2.1,0.1,0.3,2.6,2.2,0.8,2.1,1.2,1.8,1.1,0.7}.Select(d => (decimal)d)));
        }

        [Test]
        public async Task ImportingMolds_ShouldResultInCorrectAmount()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var molds = await EnumerateAsync(importer.ImportMoldsFromCsv(MoldsTestCsvPath));

            // Assert
            Assert.That(molds.Count(), Is.EqualTo(18));
        }

        [Test]
        public async Task ImportingMolds_ShouldResultInCorrectCurrentNames()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var molds = await EnumerateAsync(importer.ImportMoldsFromCsv(MoldsTestCsvPath));

            // Assert
            Assert.That(molds.Select(m => m.Name),
                Is.EquivalentTo(new[]
                {
                    "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15",
                    "F16", "F17", "F18"
                }));
        }

        [Test]
        public async Task ImportingMolds_ShouldResultInCorrectCurrentUsagesAmount()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var molds = await EnumerateAsync(importer.ImportMoldsFromCsv(MoldsTestCsvPath));

            // Assert
            Assert.That(molds.Select(m => m.CurrentUsages),
                Is.EquivalentTo(new[]
                {
                    12873, 2800, 12890, 11290, 25676, 31002, 213452, 97850, 34000, 101000, 17600, 8900, 21456, 12700,
                    103, 12000, 1213, 67345
                }));
        }

        [Test]
        public async Task ImportingMolds_ShouldResultInCorrectMaxUsagesAmount()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var molds = await EnumerateAsync(importer.ImportMoldsFromCsv(MoldsTestCsvPath));

            // Assert
            Assert.That(molds.Select(m => m.MaxUsages),
                Is.EquivalentTo(new[]
                {
                    20000, 20000, 100000, 20000, 60000, 100000, 1000000, 100000, 100000, 150000, 20000, 10000, 25000,
                    30000, 100000, 120000, 20000, 150000
                }));
        }

        [Test]
        public async Task ImportingMolds_ShouldResultInCorrectCastingCellAmounts()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var molds = await EnumerateAsync(importer.ImportMoldsFromCsv(MoldsTestCsvPath));

            // Assert
            Assert.That(molds.Select(m => m.CastingCellAmount),
                Is.EquivalentTo(
                    new [] { 1.2,  0.8,  1.8,  2,  2.2,  1.1,  1.7,  0.5,  0.3,  0.9,  0.8,  1.1,  1,  1.1,  1.2,  0.7,  0.2,  0.7}.Select(d => (decimal)d)));
        }

        [Test]
        public async Task ImportingCustomer_ShouldResultInCustomer()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var customer = await importer.ImportCustomerFromCsv(CustomerTestCsvPath);

            // Assert
            Assert.That(customer, Is.Not.EqualTo(null));
        }

        [Test]
        public async Task ImportingCustomer_ShouldResultInCorrectDates()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var customer = await importer.ImportCustomerFromCsv(CustomerTestCsvPath);

            // Assert
            Assert.That(customer.MonthlyProductReports.Select(m => m.ForMonth).Distinct(), Is.EquivalentTo(new[]
            {
                new DateTime(2019, 1, 1),
                new DateTime(2019, 2, 1),
                new DateTime(2019, 3, 1),
                new DateTime(2019, 4, 1),
                new DateTime(2019, 5, 1),
                new DateTime(2019, 6, 1),
                new DateTime(2019, 7, 1),
                new DateTime(2019, 8, 1),
                new DateTime(2019, 9, 1),
                new DateTime(2019, 10, 1),
                new DateTime(2019, 11, 1),
                new DateTime(2019, 12, 1)
            }));
        }

        [Test]
        public async Task ImportingCustomer_ShouldResultInCorrectProductNames()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var customer = await importer.ImportCustomerFromCsv(CustomerTestCsvPath);

            // Assert
            Assert.That(customer.MonthlyProductReports.Select(m => m.ForProduct.Name).Distinct(), Is.EquivalentTo(new[]
            {
                55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68
            }.Select(n => n.ToString())));
        }

        [Test]
        public async Task ImportingCustomer_ShouldResultInCorrectReportValues()
        {
            // Arrange
            var importer = new CsvImporter();

            // Act
            var customer = await importer.ImportCustomerFromCsv(CustomerTestCsvPath);

            // Assert
            Assert.That(customer.MonthlyProductReports.Where(m => m.ForProduct.Name == "61").Select(m => m.Value), Is.EquivalentTo(new[]
            {
                28481.19164,5065.067807,13450.15981,8385.872121,21831.02789,9927.427329,517.4126124,1296.574768,2373.612425,5105.65897,9503.804379,3870.762881
            }.Select(d => (decimal)d)));
        }
    }
}