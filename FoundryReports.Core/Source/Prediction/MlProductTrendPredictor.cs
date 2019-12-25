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

        private string ModelPath =>
            Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                "Source\\Prediction\\");

        // these are the generated names from the keras model. See FoundryReports.ipynb
        private const string InputColumnName = "lstm_1_input";
        private const string OutputColumnName = "dense_1/BiasAdd";

        /// <summary>
        /// The model has been trained with a look back value of 12. Meaning in order to predict the next value, it has to know the last 12
        /// </summary>
        internal const int InputNodesCount = 12;

        public MlProductTrendPredictor()
        {
            _context = new MLContext();

            var tensorFlowModel = _context.Model.LoadTensorFlowModel(ModelPath);

            _pipeline = tensorFlowModel.ScoreTensorFlowModel(
                new[] {OutputColumnName},
                new[] {InputColumnName}, addBatchDimensionInput: false);
        }

        public IMonthlyProductReport Predict(DateTime forMonth, IProductTrend forTrend)
        {
            var previousValues = ResolveValuesBeforeMonthToPredict(forTrend, forMonth);
            var prediction = PredictNextValue(previousValues.Select(d => (float)d).ToArray());
            return new MonthlyProductReport(forTrend.Product, (decimal) prediction)
            {
                IsPredicted = true,
                ForMonth = forMonth
            };
        }

        private IEnumerable<decimal> ResolveValuesBeforeMonthToPredict(IProductTrend forTrend, DateTime monthToPredict)
        {
            var reportsBeforeDesiredMonth = forTrend.Where(r => r.ForMonth < monthToPredict).ToList();
            var latestMonthNeeded = monthToPredict.PreviousMonths(InputNodesCount);

            var currentMonth = latestMonthNeeded;
            for (var i = 0; i < InputNodesCount; i++)
            {
                yield return GetOrFakeValueForMonth(currentMonth, reportsBeforeDesiredMonth);

                currentMonth = currentMonth.NextMonth();
            }
        }

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
            if (previousValues.Length != 12)
                return 0.0;

            var inputValues = _context.Data.LoadFromEnumerable(new TensorData(previousValues).Yield());
            var estimator = _pipeline.Fit(inputValues);
            var transformedValues = estimator.Transform(inputValues);
            var outScores = _context.Data.CreateEnumerable<OutputScores>(transformedValues, reuseRowObject: false);

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
            [ColumnName(OutputColumnName)]
            [VectorType(1)]
            public float[]? Prediction { get; set; }
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