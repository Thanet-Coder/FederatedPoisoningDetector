
using System;
using System.Collections.Generic;
using FederatedPoisoningDetector.Models;
using FederatedPoisoningDetector.Simulation;

namespace FederatedPoisoningDetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Federated Poisoning Detector");
            Console.WriteLine("----------------------------");

            var trainingData = new List<TrainingSample>
            {
                new TrainingSample(0.10, 0),
                new TrainingSample(0.20, 0),
                new TrainingSample(0.30, 0),
                new TrainingSample(0.70, 1),
                new TrainingSample(0.80, 1),
                new TrainingSample(0.90, 1)
            };

            Uav uav1 = new Uav(1, trainingData);

            Console.WriteLine("Training UAV 1...");

            uav1.Train(epochs: 1000, learningRate: 0.1);

            var update = uav1.GenerateUpdate(1);

            Console.WriteLine($"UAV ID: {update.UavId}");
            Console.WriteLine($"Round: {update.Round}");
            Console.WriteLine($"Weight: {update.Weight:F4}");
            Console.WriteLine($"Bias: {update.Bias:F4}");
            Console.WriteLine($"Samples: {update.SampleCount}");
            Console.WriteLine($"Malicious: {update.IsMalicious}");
        }
    }
}// end of namespace FederatedPoisoningDetector
