using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FoundryReports.Core.Reports.Visualization;
using FoundryReports.Core.Utils;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using NumSharp.Extensions;

namespace FoundryReports.Core.Source.Prediction
{
    public class MlProductTrendPredictor : IProductTrendPredictor
    {
        private readonly MLContext _context;
        private readonly TensorFlowEstimator _pipeline;
        private TensorFlowModel _tensorFlowModel;

        private string ModelPath =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                "Source\\Prediction\\");

        // these are the generated names from the keras model. See FoundryReports.ipynb
        private const string InputColumnName = "lstm_1_input";
        private const string OutputColumnName = "dense_1/BiasAdd";

        /// <summary>
        /// The look back value the model has been trained with. Meaning e.g. for look back = 12: in order to predict the next value, it has to know the last 12 values.
        /// </summary>
        internal const int InputNodesCount = 5;

        public MlProductTrendPredictor()
        {
            _context = new MLContext();

            var tensorFlowModel = _context.Model.LoadTensorFlowModel(ModelPath);
            _tensorFlowModel = tensorFlowModel;
            _pipeline = tensorFlowModel.ScoreTensorFlowModel(OutputColumnName, InputColumnName);
        }

        public IMonthlyProductReport Predict(DateTime forMonth, IProductTrend forTrend)
        {
            var previousValues = ResolveValuesBeforeMonthToPredict(forTrend, forMonth).ToList();
            var normalization = Normalize(previousValues, forTrend);
            var normalizedPrediction = PredictNextValue(normalization.Deltas.Select(d => (float) d).ToArray());
            var predictedDelta = normalization.DenormalizeDelta((decimal) normalizedPrediction);
            var lastValue = previousValues.Last();

            
            // the network might predict deltas that would rise below zero, however this doesn't make sense in the context of product usages, therefore minimum = 0
            var prediction = Math.Max(lastValue + predictedDelta, 0);

            return new MonthlyProductReport(forTrend.Product, prediction)
            {
                IsPredicted = true,
                ForMonth = forMonth
            };
        }

        private IEnumerable<decimal> ResolveValuesBeforeMonthToPredict(IProductTrend forTrend, DateTime monthToPredict)
        {
            var reportsBeforeDesiredMonth = forTrend.Where(r => r.ForMonth < monthToPredict).ToList();
            // during normalization you loose one value, therefore + 1 is required
            var latestMonthNeeded = monthToPredict.PreviousMonths(InputNodesCount + 1);

            var currentMonth = latestMonthNeeded;
            var values = new List<decimal>();
            for (var i = 0; i < InputNodesCount + 1; i++)
            {
                var value = GetOrFakeValueForMonth(currentMonth, reportsBeforeDesiredMonth);
                values.Add(value);

                currentMonth = currentMonth.NextMonth();
            }

            return values;
        }

        private DeltaNormalization Normalize(IEnumerable<decimal> values, IProductTrend forTrend) => new DeltaNormalization(values, forTrend.Select(r => r.Value));

        /// <summary>
        /// Tries to retrieve the product report value for a specific month. If it doesn't exist, it tries to somehow guess it. Either by taking the value before or after the current month, or by interpolating.
        /// </summary>
        private decimal GetOrFakeValueForMonth(DateTime targetMonth, IList<IMonthlyProductReport> reportsToTakeFrom)
        {
            var reportForThisMonth = reportsToTakeFrom.FirstOrDefault(r => r.ForMonth == targetMonth);
            if (reportForThisMonth != null)
                return reportForThisMonth.Value;

            var reportBefore = reportsToTakeFrom.OrderBy(r => r.ForMonth).LastOrDefault(r => r.ForMonth < targetMonth);
            var reportAfter = reportsToTakeFrom.OrderBy(r => r.ForMonth).FirstOrDefault(r => r.ForMonth > targetMonth);

            if (reportBefore != null && reportAfter != null)
                return (reportBefore.Value + reportAfter.Value) / 2;

            if (reportAfter != null)
                return reportAfter.Value;

            if (reportBefore != null)
                return reportBefore.Value;

            return 0;
        }

        private double PredictNextValue(float[] previousValues)
        {
            if (previousValues.Length != InputNodesCount)
                return 0.0;

            var inputValues = _context.Data.LoadFromEnumerable(new TensorData(previousValues).Yield());
            var estimator = _pipeline.Fit(inputValues);
            var transformedValues = estimator.Transform(inputValues);
            var outScores = _context.Data.CreateEnumerable<OutputScores>(transformedValues, reuseRowObject: false)
                .ToList();


            try
            {
                var prediction = outScores.First().Prediction.First();
                return prediction;
            }
            catch (Exception)
            {
                return 0;
            }
        }


        private sealed class OutputScores
        {
            [ColumnName(OutputColumnName)] public float[]? Prediction { get; set; }
        }

        private sealed class TensorData
        {
            public TensorData(params float[] features)
            {
                Features = features;
            }

            [VectorType(InputNodesCount)]
            [ColumnName(InputColumnName)]
            public float[] Features { get; set; }
        }
    }
}