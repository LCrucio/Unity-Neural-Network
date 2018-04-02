using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace kot
{
    public class Drive : MonoBehaviour
    {

        public float Speed = 50.0F;
        public float RotationSpeed = 100.0F;
        public float VisibleDistance = 200f;

        private List<string> _collectedTrainingData = new List<string>();
        private StreamWriter _trainingFile;

        void Start()
        {

            string path = Application.dataPath + "/trainingData.txt";
            _trainingFile = File.CreateText(path);
        }

        void OnApplicationQuit()
        {
            foreach (var data in _collectedTrainingData)
            {
                _trainingFile.WriteLine(data);
            }
            _trainingFile.Close();
        }

        void Update()
        {
            float translationInput = Input.GetAxis("Vertical");
            float rotationInput = Input.GetAxis("Horizontal");
            float translation = Time.deltaTime*Speed*translationInput;
            float rotation = Time.deltaTime*RotationSpeed*rotationInput;
            transform.Translate(0, 0, translation);
            transform.Rotate(0, rotation, 0);

            Debug.DrawRay(transform.position, transform.forward*VisibleDistance, Color.red);
            Debug.DrawRay(transform.position, (transform.forward - transform.right).normalized*VisibleDistance,
                Color.blue);
            Debug.DrawRay(transform.position, Quaternion.AngleAxis(45, Vector3.up)*transform.right*VisibleDistance,
                Color.yellow);

            RaycastHit hit;
            float frontDistance = 0,
                rightDistance = 0,
                leftDistance = 0,
                halfRightDistance = 0,
                halfLeftDistance = 0;

            Utils.PerformRayCasts(out frontDistance, out rightDistance, out leftDistance, out halfRightDistance, out halfLeftDistance, this.transform, VisibleDistance);

            string trainingData = frontDistance + "," + rightDistance + "," + leftDistance + "," + halfRightDistance +
                                  "," +
                                  halfLeftDistance + "," + Round(translationInput) + "," + Round(rotationInput);

            if (!_collectedTrainingData.Contains(trainingData))
                _collectedTrainingData.Add(trainingData);
        }

        float Round(float x)
        {
            return (float)System.Math.Round(x, System.MidpointRounding.AwayFromZero) / 2.0f;
        }
    }
}