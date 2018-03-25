using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{

    private NeuralNet _neuralNet;
    private double _sumSquareError = 0;
    
	void Start ()
	{
	    _neuralNet = new NeuralNet(2, 1, 1, 2, 0.9);

	    List<double> result;

	    for (int i = 0; i < 1000000; i++)
	    {
	        _sumSquareError = 0;
	        result = Train(1, 1, 0);
	        _sumSquareError += Mathf.Pow((float) result[0] - 0, 2);
            result = Train(1, 0, 1);
            _sumSquareError += Mathf.Pow((float)result[0] - 1, 2);
            result = Train(0, 1, 1);
            _sumSquareError += Mathf.Pow((float)result[0] - 1, 2);
            result = Train(0, 0, 0);
            _sumSquareError += Mathf.Pow((float)result[0] - 0, 2);
        }
	    Debug.Log("SSE: " + _sumSquareError);

	    result = Train(1, 1, 0);
	    Debug.Log(" 1 1 " + result[0]);
        result = Train(1, 0, 1);
        Debug.Log(" 1 0 " + result[0]);
        result = Train(0, 1, 1);
        Debug.Log(" 0 1 " + result[0]);
        result = Train(0, 0, 0);
        Debug.Log(" 0 0 " + result[0]);

	    Debug.Log(_neuralNet.Layers[0].Neurons[0].Weights[0]);
        Debug.Log(_neuralNet.Layers[0].Neurons[0].Weights[1]);
        Debug.Log(_neuralNet.Layers[0].Neurons[1].Weights[0]);
        Debug.Log(_neuralNet.Layers[0].Neurons[1].Weights[1]);
        Debug.Log(_neuralNet.Layers[1].Neurons[0].Weights[0]);
        Debug.Log(_neuralNet.Layers[1].Neurons[0].Weights[1]);
        

    }

    List<double> Train(double input1, double input2, double output)
    {
        List<double> inputs = new List<double>();
        List<double> outputs = new List<double>();

        inputs.Add(input1);
        inputs.Add(input2);
        outputs.Add(output);

        return (_neuralNet.Train(inputs, outputs));
    }
	
}
