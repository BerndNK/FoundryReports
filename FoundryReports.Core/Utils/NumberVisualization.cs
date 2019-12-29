using System.Collections.Generic;

namespace FoundryReports.Core.Utils
{
    public static class NumberVisualization
    {
        public static IEnumerable<string> DisplayRange(double min, double max, int intervalSteps)
        {
            var minMagnitude = NumberMagnitude.FromNumbers(min);
            var maxMagnitude = NumberMagnitude.FromNumbers(max);

            var minValue = minMagnitude.NextBiggestMagnitude;
            var maxValue = maxMagnitude.NextBiggestMagnitude;

            var wholeRange = maxValue - minValue;
            var stepSize = wholeRange / (double) (intervalSteps + 1);

            for (var i = 0; i <= intervalSteps + 1; i++)
            {
                var number = (int) (minValue + stepSize * i);
                yield return number.AsFormattedString();
            }
        }


        /// <summary>
        /// Displays large numbers with letters. E.g. 1300 -> 1.3k
        /// Source https://stackoverflow.com/questions/2134161/format-number-like-stack-overflow-rounded-to-thousands-with-k-suffix/3850141
        /// </summary>
        public static string AsFormattedString(this int number)
        {
            if (number >= 100000000)
            {
                return (number / 1000000D).ToString("0.#M");
            }

            if (number >= 1000000)
            {
                return (number / 1000000D).ToString("0.##M");
            }

            if (number >= 100000)
            {
                return (number / 1000D).ToString("0.#k");
            }

            if (number >= 10000)
            {
                return (number / 1000D).ToString("0.##k");
            }

            return number.ToString("#,0");
        }
    }
}