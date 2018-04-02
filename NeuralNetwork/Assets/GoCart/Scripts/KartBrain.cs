using System.Collections;
using System.Collections.Generic;
using System.IO;
using TemplateNetwork;
using UnityEngine;

public class KartBrain : MonoBehaviour
{
    public float Translation;
    public float Rotation;

    public bool LoadFromFile = false;
    public float VisibleDistance = 200f;
    public int Epochs = 10000;
    public float Speed = 50f;
    public float RotationSpeed = 100.0f;

    private NeuralNet _neuralNetwork;
    private bool _trainingDone;
    private float _trainingProgress = 0;
    private double _squaredSummedError = 0;
    private double _finalSqError = 1;


	// Use this for initialization
	void Start ()
	{
	    _neuralNetwork = new NeuralNet(5, 2, 1, 10, 0.05);

	    if (LoadFromFile)
	    {
            LoadWeightsFromFile();
            _trainingDone = true;
        }
        else
    	    StartCoroutine(LoadTrainingSet());
	}

    void OnGUI()
    {
        GUI.Label(new Rect(25, 25, 250, 30), "SSE " + _finalSqError);
        GUI.Label(new Rect(25, 40, 250, 30), "Alpha " + _neuralNetwork.Alpha);
        GUI.Label(new Rect(25, 55, 250, 30), "Trained " + _trainingProgress);
    }

    
    private IEnumerator LoadTrainingSet()
    {
        string path = Application.dataPath + "/trainingData.txt";
        string line;

        if (File.Exists(path))
        {
            int lineCount = File.ReadAllLines(path).Length;
            StreamReader trainingFile = File.OpenText(path);

            List<double> calcOutput = new List<double>();
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            for (int i = 0; i < Epochs; i++)
            {
                _squaredSummedError = 0;
                trainingFile.BaseStream.Position = 0;

                string currentWeights = _neuralNetwork.PrintWeights();
                while ((line = trainingFile.ReadLine()) != null)
                {
                    string[] dataSplit = line.Split(',');
                    float thisError = 0;
                    /*
                    if (!Mathf.Approximately((float) System.Convert.ToDouble(dataSplit[5]), 0) &&
                        !Mathf.Approximately((float) System.Convert.ToDouble(dataSplit[6]), 0))
                        */
                    if(System.Convert.ToDouble(dataSplit[5])!=0 && System.Convert.ToDouble(dataSplit[6])!=0)
                    {
                        inputs.Clear();
                        outputs.Clear();
                        inputs.Add(System.Convert.ToDouble(dataSplit[0]));
                        inputs.Add(System.Convert.ToDouble(dataSplit[1]));
                        inputs.Add(System.Convert.ToDouble(dataSplit[2]));
                        inputs.Add(System.Convert.ToDouble(dataSplit[3]));
                        inputs.Add(System.Convert.ToDouble(dataSplit[4]));

                        double output0 = Map(0, 1, -1, 1, System.Convert.ToSingle(dataSplit[5]));
                        outputs.Add(output0);

                        double output1 = Map(0, 1, -1, 1, System.Convert.ToSingle(dataSplit[6]));
                        outputs.Add(output1);

                        calcOutput = _neuralNetwork.TrainS(inputs, outputs);
                        thisError = ((Mathf.Pow((float)(outputs[0] - calcOutput[0]), 2) +
                            Mathf.Pow((float)(outputs[1] - calcOutput[1]), 2))) / 2.0f;
                    }
                    _squaredSummedError += thisError;
                }
                _trainingProgress = (float) i/(float) Epochs;
                _squaredSummedError /= lineCount;
                //_finalSqError = _squaredSummedError;

                if (_finalSqError < _squaredSummedError)
                {
                    _neuralNetwork.LoadWeights(currentWeights);
                    _neuralNetwork.Alpha = Mathf.Clamp((float) _neuralNetwork.Alpha - 0.001f, 0.01f, 0.9f);
                }
                else
                {
                    _neuralNetwork.Alpha = Mathf.Clamp((float) _neuralNetwork.Alpha + 0.001f, 0.01f, 0.9f);
                    _finalSqError = _squaredSummedError;
                }

                yield return null;
            }


        }
        SaveWeightsToFile();
        _trainingDone = true;
    }
    

    // Update is called once per frame
	void Update () {
        if (!_trainingDone) return;

	    List<double> calcOutputs = new List<double>();
	    List<double> inputs = new List<double>();
	    List<double> outputs = new List<double>();

	    RaycastHit hit;
        float frontDistance = 0,
            rightDistance = 0,
            leftDistance = 0,
            halfRightDistance = 0,
            halfLeftDistance = 0;

        Utils.PerformRayCasts(out frontDistance, out rightDistance, out leftDistance, out halfRightDistance, out halfLeftDistance, this.transform, VisibleDistance);

        inputs.Add(frontDistance);
	    inputs.Add(rightDistance);
	    inputs.Add(leftDistance);
	    inputs.Add(halfRightDistance);
	    inputs.Add(halfLeftDistance);
	    outputs.Add(0);
	    outputs.Add(0);

	    calcOutputs = _neuralNetwork.CalcOutput(inputs, outputs);
	    float translationInput = Map(-1f, 1f, 0f, 1f, (float)calcOutputs[0]);
	    float rotationInput = Map(-1, 1, 0, 1, (float) calcOutputs[1]);

	    Translation = translationInput*Speed*Time.deltaTime;
	    Rotation = rotationInput*RotationSpeed*Time.deltaTime;

	    transform.Translate(0, 0, Translation);
	    transform.Rotate(0, Rotation, 0);
        
	}

    void SaveWeightsToFile()
    {
        string path = Application.dataPath + "/weights.txt";
        StreamWriter wf = File.CreateText(path);
        wf.WriteLine(_neuralNetwork.PrintWeights());
        wf.Close();
    }

    void LoadWeightsFromFile()
    {
        string path = Application.dataPath + "/weights.txt";
        StreamReader wf = File.OpenText(path);

        if (File.Exists(path))
        {
            string line = wf.ReadLine();
            _neuralNetwork.LoadWeights(line);
        }
    }

    float Map(float newfrom, float newto, float origfrom, float origto, float value)
    {
        if (value <= origfrom)
            return newfrom;
        if (value >= origto)
            return newto;
        return (newto - newfrom) * ((value - origfrom) / (origto - origfrom)) + newfrom;
    }

}
