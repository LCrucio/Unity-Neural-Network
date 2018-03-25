using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Equivalent to the perceptron model
/// </summary>
public class Neuron
{

    public double Bias;
    public double Output;
    public double ErrorGradient;

    public int InputsNum;

    public List<double> Weights = new List<double>();
    public List<double> Inputs = new List<double>();

    public Neuron(int inputsNum)
    {
        Bias = UnityEngine.Random.Range(-1.0f, 1.0f);
        InputsNum = inputsNum;
        for(int i = 0; i< inputsNum; i++)
        {
            Weights.Add(Random.Range(-1.0f, 1.0f));
        }
    }

}
