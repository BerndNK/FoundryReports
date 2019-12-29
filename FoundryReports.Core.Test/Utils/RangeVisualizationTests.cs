using System.Linq;
using FoundryReports.Core.Utils;
using NUnit.Framework;

namespace FoundryReports.Core.Test.Utils
{
    class RangeVisualizationTests
    {

        [TestCase(0, 10, 4, new []{"0", "2", "4", "6", "8", "10"})]
        [TestCase(0, 9, 4, new []{"0", "2", "4", "6", "8", "10"})]
        [TestCase(-7, 9, 4, new []{"-10", "-6", "-2", "2", "6", "10"})]
        [TestCase(-7, 9, 5, new []{"-10", "-6", "-3", "0", "3", "6", "10"})]
        public void RangeShouldResultInExpectedStrings(double min, double max, int steps, string[] expectedStrings)
        {
            // act
            var result = NumberVisualization.DisplayRange(min, max, steps).ToList();

            // assert
            Assert.That(result, Is.EqualTo(expectedStrings));
        }

    }
}
