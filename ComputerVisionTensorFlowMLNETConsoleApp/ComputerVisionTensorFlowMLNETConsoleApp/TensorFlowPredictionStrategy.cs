using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;

namespace ComputerVisionTensorFlowMLNETConsoleApp
{
    public enum Approaches
    {
        Default,
        TensorFlowPreTrained,
        TensorFlowCustom
    }

    public interface ITensorFlowPredictionStrategy
    {
        System.Threading.Tasks.Task<IEnumerable<string>> ClassifyImageAsync(byte[] image);
    }

    public class TensorFlowPredictionStrategy : ITensorFlowPredictionStrategy
    {
        private readonly Dictionary<Approaches, IClassifier> models;
        private readonly Approaches defaultModel;

        public TensorFlowPredictionStrategy(IOptionsSnapshot<AppSettings> settings, IHostingEnvironment environment)
        {
            object parseDefaultModel;
            defaultModel =
                (Enum.TryParse(typeof(Approaches), settings.Value.TensorFlowPredictionDefaultModel, ignoreCase: true, result: out parseDefaultModel)) ?
                (Approaches)parseDefaultModel :
                 Approaches.Default;

            if (defaultModel == Approaches.Default)
                defaultModel = Approaches.TensorFlowPreTrained;

            models = new Dictionary<Approaches, IClassifier>
            {
                //{ Approaches.TensorFlowPreTrained, new TensorFlowInceptionPrediction(settings, environment) },
                //{ Approaches.TensorFlowCustom, new TensorFlowModelPrediction(settings, environment) }
            };
        }

        /// <summary>
        /// Classify an image, using a TensorFlow model
        /// </summary>
        /// <param name="image">image (jpeg) bytes to be analyzed</param>
        /// <returns>image related labels</returns>
        /// 
        public async System.Threading.Tasks.Task<IEnumerable<string>> ClassifyImageAsync(byte[] image)
        {
            //var classification = await models[defaultModel].ClassifyImageAsync(image);

            //return classification.OrderByDescending(c => c.Probability)
            //    .Select(c => c.Label);

            return null;
        }
    }
}
