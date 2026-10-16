
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Simulation
{
    public class Uav
    {
        public int Id { get; }

        public double Weight { get; private set; }

        public double Bias { get; private set; }

        public int SampleCount => _trainingData.Count;

        private readonly List<TrainingSample> _trainingData;

        public Uav(int id, List<TrainingSample> trainingData)
        {
            Id = id;
            _trainingData = trainingData;

            Weight = 0.0;
            Bias = 0.0;
        }

        private static double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        public void Train(int epochs, double learningRate)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                double weightGradient = 0.0;
                double biasGradient = 0.0;

                foreach (var sample in _trainingData)
                {
                    double prediction = Sigmoid(
                        Weight * sample.Temperature + Bias);

                    double error =
                        prediction - sample.FireDetected;

                    weightGradient +=
                        error * sample.Temperature;

                    biasGradient += error;
                }

                Weight -= learningRate *
                    weightGradient / SampleCount;

                Bias -= learningRate *
                    biasGradient / SampleCount;
            }
        }

        public ModelUpdate GenerateUpdate(int round)
        {
            return new ModelUpdate
            {
                UavId = Id,
                Round = round,
                Weight = Weight,
                Bias = Bias,
                SampleCount = SampleCount,
                IsMalicious = false
            };
        }
    }// end of class Uav
}// end of namespace FederatedPoisoningDetector.Simulation
