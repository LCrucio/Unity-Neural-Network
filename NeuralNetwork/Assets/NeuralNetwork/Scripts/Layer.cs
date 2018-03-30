using System.Collections.Generic;

namespace TemplateNetwork
{
    public class Layer
    {
        public int NeuronNum;
        public List<Neuron> Neurons = new List<Neuron>();

        public Layer(int neuronNum, int neuronInputsNum)
        {
            NeuronNum = neuronNum;
            for (int i = 0; i < neuronNum; i++)
            {
                Neurons.Add(new Neuron(neuronInputsNum));
            }
        }
    }
}