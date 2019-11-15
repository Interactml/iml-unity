using System;

namespace InteractML
{
    /// <summary>
    /// Holds a training example ready to be used with Rapidlib
    /// </summary>
    [Serializable]
    public struct RapidlibTrainingExample
    {
        public double[] Input;
        public double[] Output;

        public RapidlibTrainingExample(double[] inputs, double[] outputs)
        {
            Input = new double[inputs.Length];
            Output = new double[outputs.Length];
            Array.Copy(inputs, Input, inputs.Length);
            Array.Copy(outputs, Output, outputs.Length);

        }

        public void AddExample (double input, double output)
        {
            Array.Resize(ref Input, Input.Length + 1);
            Input[Input.GetUpperBound(0)] = input;

            Array.Resize(ref Output, Output.Length + 1);
            Output[Output.GetUpperBound(0)] = output;

        }

        public void AddExample (double[] inputs, double[] outputs)
        {
            int originalSize = Input.Length;
            Array.Resize(ref Input, Input.Length + inputs.Length);
            for (int i = originalSize - 1; i < Input.Length; i++)
            {
                Input[i] = inputs[i];
            }

            originalSize = Output.Length;
            Array.Resize(ref Output, Output.Length + outputs.Length);
            for (int i = originalSize - 1; i < Output.Length; i++)
            {
                Output[i] = outputs[i];
            }
        }
    }

}

