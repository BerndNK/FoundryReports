using System;
using System.Collections.Generic;
using System.Linq;

namespace FoundryReports.Core.Source.Prediction
{
    internal class DeltaNormalization
    {
        private readonly decimal _maxDelta;
        private readonly decimal _minDelta;

        public IEnumerable<decimal> Deltas { get; }

        /// <summary>
        /// Normalizes the given values to relative deltas between [0, 1].
        /// </summary>
        /// <param name="denormalized">The values that shall be normalized.</param>
        /// <param name="context">Context of values to normalize, to use for min / max values. E.g. values [1,3,7] normalized -> [0,1] 1 = +4, 0 = +2. With context [10,18,34], it`d be [0, 0.14]. With 1 = +16, 0 = +2</param>
        public DeltaNormalization(IEnumerable<decimal> denormalized, IEnumerable<decimal> context)
        {
            var (normalized, minDelta, maxDelta) = ResolveNormalized(denormalized, context);
            Deltas = normalized;
            _minDelta = minDelta;
            _maxDelta = maxDelta;
        }

        public decimal DenormalizeDelta(decimal normalizedDelta)
        {
            return DenormalizeScalarMinMax(normalizedDelta, _minDelta, _maxDelta);
        }

        private (IEnumerable<decimal> normalized, decimal minDelta, decimal maxDelta) ResolveNormalized(IEnumerable<decimal> denormalized, IEnumerable<decimal> context)
        {
            var toNormalizeDeltas= AsDeltas(denormalized);
            var contextDeltas = AsDeltas(context);

            var min = Math.Min(toNormalizeDeltas.Min(), contextDeltas.Min());
            var max = Math.Max(toNormalizeDeltas.Max(), contextDeltas.Max());

            return (toNormalizeDeltas.Select(d => NormalizeScalarMinMax(d, min, max)).ToList(), min, max);
        }

        private static List<decimal> AsDeltas(IEnumerable<decimal> denormalized)
        {
            var deltas = new List<decimal>();
            // convert values into deltas
            decimal? lastValue = null;
            foreach (var currentValue in denormalized)
            {
                if (lastValue == null)
                {
                    lastValue = currentValue;
                }
                else
                {
                    deltas.Add(currentValue - lastValue!.Value);
                    lastValue = currentValue;
                }
            }

            return deltas;
        }

        private decimal NormalizeScalarMinMax(decimal value, decimal min, decimal max)
        {
            // avoid division by zero
            if (max - min == 0)
                return value;

            return (value - min) / (max - min);
        }

        private decimal DenormalizeScalarMinMax(decimal normalized, decimal min, decimal max)
        {
            return normalized * (max - min) + min;
        }
    }
}