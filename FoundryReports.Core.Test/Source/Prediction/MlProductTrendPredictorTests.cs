using System;
using System.Collections.Generic;
using System.Text;
using FoundryReports.Core.Source.Prediction;
using NUnit.Framework;

namespace FoundryReports.Core.Test.Source.Prediction
{
    class MlProductTrendPredictorTests
    {

        [Test]
        public void LoadShouldSucceed()
        {

            var predictor = new MlProductTrendPredictor();
        }
    }
}
