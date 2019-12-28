using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Source;
using FoundryReports.Core.Source.Prediction;
using FoundryReports.Core.Utils;
using NSubstitute;
using NUnit.Framework;

namespace FoundryReports.Core.Test.Source.Prediction
{
    class EventPredictorTests
    {
        private IDataSource _dataSource;
        private IMold MoldA { get; }
        private IMold MoldB { get; }

        private IProduct MoldAUsingProduct { get; }
        private IProduct MoldBTwiceUsingProduct { get; }
        private IProduct MoldAAndBUsingProduct { get; }

        private readonly DateTime _currentMonth = DateTime.Today;

        private IEventPredictor? _eventPredictor;

        private IList<IProduct>? _products;

        public EventPredictorTests()
        {
            MoldA = MoldSubstitute(6, 10);
            MoldB = MoldSubstitute(9, 10);

            MoldAUsingProduct = Product(nameof(MoldAUsingProduct), MoldRequirement(MoldA, 1));
            MoldBTwiceUsingProduct = Product(nameof(MoldBTwiceUsingProduct), MoldRequirement(MoldA, 2));
            MoldAAndBUsingProduct = Product(nameof(MoldAAndBUsingProduct), MoldRequirement(MoldA, 1),
                MoldRequirement(MoldB, 1));
        }

        private static IMold MoldSubstitute(decimal currentUsages, decimal maxUsages)
        {
            var mold = Substitute.For<IMold>();
            mold.CurrentUsages.Returns(currentUsages);
            mold.MaxUsages.Returns(maxUsages);
            return mold;
        }

        private IProduct Product(string productName, params IMoldRequirement[] requirements)
        {
            var product = Substitute.For<IProduct>();
            product.MoldRequirements.Returns(requirements);
            product.Name.Returns(productName);
            return product;
        }

        private IMoldRequirement MoldRequirement(IMold forMold, decimal usage)
        {
            var requirement = Substitute.For<IMoldRequirement>();
            requirement.Mold.Returns(forMold);
            requirement.UsageAmount.Returns(usage);
            return requirement;
        }

        [SetUp]
        public void Setup()
        {
            _dataSource = Substitute.For<IDataSource>();
            _products = new List<IProduct> {MoldAUsingProduct, MoldBTwiceUsingProduct, MoldAAndBUsingProduct};
            _dataSource.Products.Returns(_products);
            _dataSource.Molds.Returns(new[] {MoldA, MoldB});

            _eventPredictor = new EventPredictor(_dataSource);
        }

        private IProductTrend ProductTrend(IProduct forProduct, DateTime startMonth, params decimal[] usages)
        {
            var trend = new ProductTrendDummy {Product = forProduct, MinUsage = usages.Min(), MaxUsage = usages.Max()};

            var currentMonth = startMonth;
            foreach (var usage in usages)
            {
                var report = Substitute.For<IMonthlyProductReport>();
                report.Value.Returns(usage);
                report.ForProduct.Returns(forProduct);
                report.ForMonth.Returns(currentMonth);
                currentMonth = currentMonth.NextMonth();

                trend.Add(report);
            }

            return trend;
        }

        private ICustomer Customer(params IProductTrend[] productTrends)
        {
            var customer = Substitute.For<ICustomer>();
            var monthlyReports = productTrends.SelectMany(t => t).ToList();
            customer.MonthlyProductReports.Returns(monthlyReports);

            return customer;
        }

        [Test]
        public void ProductUsage_WhichExceedsMoldCapacity_ResultsInEvent()
        {
            // arrange
            var customer = Customer(ProductTrend(MoldAUsingProduct, _currentMonth.NextMonth(), 1, 2, 1, 1));

            // act
            var events = _eventPredictor!.PredictEvents(customer.MonthlyProductReports).ToList();

            // assert
            Assert.That(events.Count(), Is.EqualTo(1));
            var predictedEvent = events.First() as MoldReachedEndOfLifetimeEvent;
            Assert.That(predictedEvent, Is.Not.Null);
        }

        [Test]
        public void ProductUsage_WhichDoesNotExceedMoldCapacity_ResultsInNoEvents()
        {
            // arrange
            var customer = Customer(ProductTrend(MoldAUsingProduct, _currentMonth.NextMonth(), 1, 2, 1));

            // act
            var events = _eventPredictor!.PredictEvents(customer.MonthlyProductReports);

            // assert
            Assert.That(events, Is.Empty);
        }

        [Test]
        public void MultipleProductUsages_WhichDoesNotExceedMoldCapacity_ResultsInNoEvents()
        {
            // arrange
            var customer = Customer(
                ProductTrend(MoldAUsingProduct, _currentMonth.NextMonth(), 1, 2),
                ProductTrend(MoldAAndBUsingProduct, _currentMonth.NextMonth(), 1)
            );

            // act
            var events = _eventPredictor!.PredictEvents(customer.MonthlyProductReports);

            // assert
            Assert.That(events, Is.Empty);
        }

        [Test]
        public void MultipleProductUsages_WithOneExceedingMoldCapacity_ResultsInEventTellingAboutThatProduct()
        {
            // arrange
            var customer = Customer(
                ProductTrend(MoldAUsingProduct, _currentMonth.NextMonth(), 1, 2, 1),
                ProductTrend(MoldBTwiceUsingProduct, _currentMonth.NextMonth(), 3)
            );

            // act
            var events = _eventPredictor!.PredictEvents(customer.MonthlyProductReports);

            // assert
            var moldEvent = events.OfType<MoldReachedEndOfLifetimeEvent>().First();
            Assert.That(moldEvent.ForReport.ForProduct, Is.EqualTo(MoldBTwiceUsingProduct));
        }

        [Test]
        public void ProductUsage_NotExceedingMoldCapacityAfterThisMonth_ResultsInNoEvents()
        {
            // arrange
            var customer = Customer(
                ProductTrend(MoldAUsingProduct, _currentMonth, 100, 2)
            );

            // act
            var events = _eventPredictor!.PredictEvents(customer.MonthlyProductReports);

            // assert
            Assert.That(events, Is.Empty);
        }
    }
}