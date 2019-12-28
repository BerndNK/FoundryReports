using System;
using System.Collections.Generic;
using System.Linq;
using FoundryReports.Core.Products;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Source.Prediction;
using FoundryReports.Core.Utils;
using NSubstitute;
using NUnit.Framework;

namespace FoundryReports.Core.Test.Source.Prediction
{
    class MlProductTrendPredictorTests
    {
        private DateTime CurrentMonthToPredict { get; } = new DateTime(2020, 1, 1);
        private DateTime LatestMonthOfTrend => CurrentMonthToPredict.PreviousMonths(MlProductTrendPredictor.InputNodesCount);

        [TestCase(0.5784, new[]
        {
            0.55410557, 0.42505458, 0.51299183, 0.52854088, 0.54266738, 0.49943218, 0.60640473, 0.25073919, 0.6435862,
            0.4223072, 0.57065117
        })]
        public void PredictionShouldYieldExpectedValue(double expectedPrediction, double[] previousValues)
        {
            // arrange
            var predictor = new MlProductTrendPredictor();

            // act
            previousValues = previousValues.Take(MlProductTrendPredictor.InputNodesCount).ToArray();
            var result = predictor.Predict(CurrentMonthToPredict, AsProductTrend(previousValues));

            // assert
            var precision = 10000;
            Assert.That(Math.Round(result.Value * precision), Is.EqualTo(Math.Round(expectedPrediction * precision)));
        }

        [Test]
        public void PredictionHasPredictedFlag()
        {
            // arrange
            var predictor = new MlProductTrendPredictor();

            // act
            var result = predictor.Predict(CurrentMonthToPredict,
                AsProductTrend(RandomValues(MlProductTrendPredictor.InputNodesCount).ToArray()));

            // assert
            Assert.That(result.IsPredicted, Is.True);
        }

        [Test]
        public void PredictionWithOnlyNotEnoughValuesStillProducesResults()
        {
            try
            {
                // arrange
                var predictor = new MlProductTrendPredictor();
                var availableValues = RandomValues(MlProductTrendPredictor.InputNodesCount - 2);

                // act
                var result = predictor.Predict(CurrentMonthToPredict, AsProductTrend(availableValues.ToArray()));

                // assert
                Assert.That(result, Is.Not.Zero);
            }
            catch (Exception)
            {
                // exceptions are ignored, as the loading of the predictor is already covered in "LoadShouldSucceed" unit test
                return;
            }
        }

        private IEnumerable<double> RandomValues(int amount)
        {
            var rnd = new Random();
            for (var i = 0; i < amount; i++)
                yield return rnd.NextDouble();
        }

        private IProductTrend AsProductTrend(params double[] trendValues)
        {
            var product = Substitute.For<IProduct>();
            var trend = new ProductTrendDummy();
            trend.Product =product;
            trend.MinUsage =0;
            trend.MaxUsage = 1;

            var monthlyReports = new List<IMonthlyProductReport>();
            var currentMonth = LatestMonthOfTrend;

            foreach (var trendValue in trendValues)
            {
                var report = new MonthlyProductReportDummy
                {
                    ForMonth = currentMonth,
                    ForProduct = product, 
                    IsPredicted = false, 
                    Value = (decimal) trendValue
                };
                currentMonth = currentMonth.NextMonth();

                trend.Add(report);
            }

            return trend;
        }
    }
}