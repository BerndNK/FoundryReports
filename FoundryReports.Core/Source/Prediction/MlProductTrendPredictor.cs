using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using FoundryReports.Core.Reports;
using FoundryReports.Core.Reports.Visualization;
using Microsoft.ML;
using Microsoft.ML.Data;
using Tensorflow.Framework.Models;

namespace FoundryReports.Core.Source.Prediction
{
    public class MlProductTrendPredictor : IProductTrendPredictor
    {
        private string ModelPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,"Source\\Prediction\\");

        // these are the generated names from the keras model. See FoundryReports.ipynb
        private const string InputSchemaName = "lstm_135_input";
        private const string OutputSchemaName = "dense_132/BiasAdd";

        public MlProductTrendPredictor()
        {
            var context = new MLContext();
            var graph = new Tensorflow.Graph();
            var tensorFlowModel = context.Model.LoadTensorFlowModel(ModelPath);
            var schema = tensorFlowModel.GetModelSchema();
            var featuresType = (VectorDataViewType)schema[InputSchemaName].Type;
            Debug.WriteLine("Name: {0}, Type: {1}, Shape: (-1, {2})", "Features",
                featuresType.ItemType.RawType, featuresType.Dimensions[0]);

            var predictionType = (VectorDataViewType)schema[OutputSchemaName]
                .Type;
            Debug.WriteLine("Name: {0}, Type: {1}, Shape: (-1, {2})",
                "Prediction/Softmax", predictionType.ItemType.RawType,
                predictionType.Dimensions[0]);
        }

        public IMonthlyProductReport Predict(DateTime forMonth, IProductTrend forTrend, ICustomerOverview overview)
        {
            throw new NotImplementedException();
        }
    }
}
