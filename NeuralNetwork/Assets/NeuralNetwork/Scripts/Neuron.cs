using System.Collections.Generic;
using UnityEngine;

namespace TemplateNetwork
{
    /// <summary>
    ///     Equivalent to the perceptron model
    /// </summary>
    public class Neuron
    {
        public double Bias;
        public double ErrorGradient;
        public List<double> Inputs = new List<double>();

        public int InputsNum;
        public double Output;

        public List<double> Weights = new List<double>();

        public Neuron(int inputsNum)
        {
            Bias = Random.Range(-1.0f, 1.0f);
            InputsNum = inputsNum;
            for (int i = 0; i < inputsNum; i++)
            {
                Weights.Add(Random.Range(-1.0f, 1.0f));
            }
        }
    }
}