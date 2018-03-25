using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNet
{
    /// <summary>
    /// Impact of each one learning set on the shape of the network
    /// </summary>
    public double Alpha;

    public int InputNum;
    public int OutputNum;
    public int HiddenLayerNum;
    public int NeuronPerHiddenLayerNum;

    public List<Layer> Layers = new List<Layer>();

    public NeuralNet(int numIn, int numOut, int numHid, int numPerHid, double alpha)
    {
        InputNum = numIn;
        OutputNum = numOut;
        HiddenLayerNum = numHid;
        NeuronPerHiddenLayerNum = numPerHid;
        Alpha = alpha;

        if(numHid > 0)
        {
            Layers.Add(new Layer(NeuronPerHiddenLayerNum, InputNum));

            for(int i=0; i< HiddenLayerNum-1; i++)
            {
                Layers.Add(new Layer(NeuronPerHiddenLayerNum, NeuronPerHiddenLayerNum));
            }

            Layers.Add(new Layer(OutputNum, NeuronPerHiddenLayerNum));
        }
        else
        {
            Layers.Add(new Layer(OutputNum, InputNum));
        }
    }

    public List<double> Train(List<double> inputVals, List<double> desiredOutput)
    {
        List<double> inputs = new List<double>(inputVals);
        List<double> outputs = new List<double>();

        if(inputs.Count!=InputNum)
        {
            Debug.LogError("Invalid  number of inputs");
            return null;
        }

        // each layer
        for(int i=0; i<HiddenLayerNum+1; i++)
        {
            if(i>0)
            {
                // Output of i becomes input of i+1
                inputs = new List<double>(outputs);
            }

            outputs.Clear();

            // each neuron on each layer
            for(int j=0; j<Layers[i].NeuronNum; j++)
            {
                double rawNeuronValue = 0;
                Layers[i].Neurons[j].Inputs.Clear();

                // each input slot for each neuron on each layer
                for(int k=0; k<Layers[i].Neurons[j].InputsNum; k++)
                {
                    Layers[i].Neurons[j].Inputs.Add(inputs[k]);
                    rawNeuronValue += Layers[i].Neurons[j].Weights[k] * inputs[k];
                }

                rawNeuronValue -= Layers[i].Neurons[j].Bias;

                if(i == HiddenLayerNum)
                    Layers[i].Neurons[j].Output = ActivationFunctionOut(rawNeuronValue);
                else
                    Layers[i].Neurons[j].Output = ActivationFunction(rawNeuronValue);
                outputs.Add(Layers[i].Neurons[j].Output);
            }
        }

        UpdateWeights(outputs, desiredOutput);
        return outputs;
        
    }

    private void UpdateWeights(List<double> outputs, List<double> desiredOutput)
    {
        double error;

        for (int i = HiddenLayerNum; i >= 0; i--)
        {
            for (int j = 0; j < Layers[i].NeuronNum; j++)
            {
                if (i == HiddenLayerNum)
                {
                    error = desiredOutput[j] - outputs[j];
                    // Simplified error gradient (delta rule)
                    Layers[i].Neurons[j].ErrorGradient = outputs[j] * (1 - outputs[j]) * error;
                }
                else
                {
                    Layers[i].Neurons[j].ErrorGradient = Layers[i].Neurons[j].Output * (1 - Layers[i].Neurons[j].Output);

                    double higherLayerErrorGradSum = 0;
                    for (int m = 0; m < Layers[i + 1].NeuronNum; m++)
                    {
                        higherLayerErrorGradSum += Layers[i + 1].Neurons[m].ErrorGradient * Layers[i + 1].Neurons[m].Weights[j];
                    }
                    Layers[i].Neurons[j].ErrorGradient *= higherLayerErrorGradSum;
                }

                // each input weight correction
                for(int k = 0; k<Layers[i].Neurons[j].InputsNum; k++)
                {
                    // only last corrected by full error
                    if (i == HiddenLayerNum)
                    {
                        error = desiredOutput[j] - outputs[j];
                        Layers[i].Neurons[j].Weights[k] += Alpha * Layers[i].Neurons[j].Inputs[k] * error;
                    }
                    else
                    {
                        Layers[i].Neurons[j].Weights[k] += Alpha * Layers[i].Neurons[j].Inputs[k] * Layers[i].Neurons[j].ErrorGradient;
                    }
                }

                Layers[i].Neurons[j].Bias += Alpha * -1 * Layers[i].Neurons[j].ErrorGradient;
            }
        }
    }

    private double ActivationFunction(double value)
    {
        return Sigmoid(value);
    }


    private double ActivationFunctionOut(double value)
    {
        return Sigmoid(value);
    }


    private double Sigmoid (double value)
    {
        double k = (double)System.Math.Exp(value);
        return k / (1.0f + k);
	}

    private double Step(double value)
    {
        if (value < 0) return 0;
        return 1;
    }

    private double TanH(double value)
    {
        return (2*(Sigmoid(2*value) - 1));
    }

    /// <summary>
    /// rectified linear unit
    /// </summary>
    private double ReLu(double value)
    {
        if(value > 0)
            return value;
        return 0;
    }

    private double LeakyReLu(double value)
    {
        if (value < 0) return 0.01 * value;
        return value;
    }
	
}
