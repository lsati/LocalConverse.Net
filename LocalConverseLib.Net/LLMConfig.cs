using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalConverseLib.Net
{
    public class LLMConfig
    {
        public LLMConfig() { }

        public string ModelPath { get; set; }


        public static LLMConfig Default()
        {
            string modelPath = @"D:\models\lmstudio\QuantFactory\Meta-Llama-3-8B-Instruct-GGUF\Meta-Llama-3-8B-Instruct.Q4_0.gguf";

            return new LLMConfig()
            {
                ModelPath = modelPath
            };
        }
    }
}
