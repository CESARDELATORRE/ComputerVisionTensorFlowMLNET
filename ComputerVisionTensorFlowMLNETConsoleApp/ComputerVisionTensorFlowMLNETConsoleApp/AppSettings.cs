using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerVisionTensorFlowMLNETConsoleApp
{
    public class AppSettings
    {
        public string AIModelsPath { get; set; }
        public string TensorFlowPredictionDefaultModel { get; set; } //TensorFlowPreTrained|TensorFlowCustom
    }
}
