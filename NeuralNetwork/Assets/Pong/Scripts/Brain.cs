using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PongNetwork
{
    public class Brain : MonoBehaviour
    {
        public GameObject Paddle;
        public GameObject Ball;
        private Rigidbody2D _ballRB;

        public float YVall;
        public float PaddleMinY = 8.8f;
        public float PaddleMaxY = 17.4f;
        public float PaddleMaxSpeed = 15;

        public bool Learn = true;
        public float LearningRate = 0.02f;

        public float ScoreSaved = 0;
        public float ScoreMissed = 0;

        private TemplateNetwork.NeuralNet _neuralNet;

        // Use this for initialization
        void Start()
        {
            _neuralNet = new TemplateNetwork.NeuralNet(6, 1, 1, 12, LearningRate);
            _ballRB = Ball.GetComponent<Rigidbody2D>();
        }

        List<double> Run(double ballX, double ballY, double ballVelocityX, double ballVelocityY, double paddleX, double paddleY, double  paddleVelocityY, bool train)
        {
            List<double> inputs = new List<double>();
            List<double> outputs = new List<double>();

            inputs.Add(ballX);
            inputs.Add(ballY);
            inputs.Add(ballVelocityX);
            inputs.Add(ballVelocityY);
            inputs.Add(paddleX);
            inputs.Add(paddleY);

            outputs.Add(paddleVelocityY);

            if(train)
            {
                return _neuralNet.TrainS(inputs, outputs);
            }
            else
            {
                return _neuralNet.CalcOutput(inputs, outputs);
            }
        }

        // Update is called once per frame
        void Update()
        {
            float posY = Mathf.Clamp(Paddle.transform.position.y + (YVall*Time.deltaTime*PaddleMaxSpeed), PaddleMinY, PaddleMaxY);

            Paddle.transform.position = new Vector3(Paddle.transform.position.x, posY, Paddle.transform.position.z);

            List<double> output = new List<double>();
            int layerMask = 1 << 9;

            RaycastHit2D hit = Physics2D.Raycast(Ball.transform.position, _ballRB.velocity, 100, layerMask);

            if (hit.collider != null && hit.collider.tag == "tops") {
                Vector3 reflection = Vector3.Reflect(_ballRB.velocity, hit.normal);
                hit = Physics2D.Raycast(hit.point, reflection, 1000, layerMask);
            }
            if (hit.collider != null && hit.collider.tag == "backwall")
            {
                float y = hit.point.y - Paddle.transform.position.y;

                output = Run(Ball.transform.position.x, Ball.transform.position.y, _ballRB.velocity.x, _ballRB.velocity.y, Paddle.transform.position.x, Paddle.transform.position.y, y, Learn);
                YVall = (float)output[0];
            }
            else
            {
                YVall = 0;
            }
        }
    }

}