
using System;
using System.Collections.Generic;
using FederatedPoisoningDetector.Models;
using FederatedPoisoningDetector.Simulation;
using FederatedPoisoningDetector.Federation;
using FederatedPoisoningDetector.Attacks;

namespace FederatedPoisoningDetector
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Federated Poisoning Detector");
            Console.WriteLine("----------------------------");

            // Create five UAVs with different local datasets.
            var uavs = new List<Uav>
            {
                new Uav(1, new List<TrainingSample>
                {
                    new TrainingSample(0.10, 0),
                    new TrainingSample(0.20, 0),
                    new TrainingSample(0.30, 0),
                    new TrainingSample(0.70, 1)
                }),

                new Uav(2, new List<TrainingSample>
                {
                    new TrainingSample(0.15, 0),
                    new TrainingSample(0.25, 0),
                    new TrainingSample(0.75, 1),
                    new TrainingSample(0.85, 1)
                }),

                new Uav(3, new List<TrainingSample>
                {
                    new TrainingSample(0.40, 0),
                    new TrainingSample(0.60, 1),
                    new TrainingSample(0.80, 1),
                    new TrainingSample(0.90, 1)
                }),

                new Uav(4, new List<TrainingSample>
                {
                    new TrainingSample(0.05, 0),
                    new TrainingSample(0.15, 0),
                    new TrainingSample(0.25, 0),
                    new TrainingSample(0.35, 0)
                }),

                new Uav(5, new List<TrainingSample>
                {
                    new TrainingSample(0.65, 1),
                    new TrainingSample(0.75, 1),
                    new TrainingSample(0.85, 1),
                    new TrainingSample(0.95, 1)
                })
            };

            FederatedServer server = new FederatedServer();

            int rounds = 10;
            int localEpochs = 100;
            double learningRate = 0.1;

            for (int round = 1; round <= rounds; round++)
            {
                var updates = new List<ModelUpdate>();

                foreach (var uav in uavs)
                {
                    // Synchronise with the global model.
                    uav.SetModel(
                        server.GlobalWeight,
                        server.GlobalBias);

                    // Perform local training.
                    uav.Train(localEpochs, learningRate);


                    // Collect the updated parameters.
                    ModelUpdate update = uav.GenerateUpdate(round);

                    // UAV 3 is compromised.
                    if (uav.Id == 3)
                    {
                        update = SignFlippingAttack.Apply(
                            update,
                            server.GlobalWeight,
                            server.GlobalBias,
                            strength: 5.0);
                    }

                    updates.Add(update);

                }

                // Aggregate all local models.
                server.Aggregate(updates);

                Console.WriteLine(
                    $"Round {round,2}: " +
                    $"Weight = {server.GlobalWeight:F4}, " +
                    $"Bias = {server.GlobalBias:F4}");
            }

            Console.WriteLine();
            Console.WriteLine("Federated training completed.");


            var testData = new List<TrainingSample>
{
    new TrainingSample(0.08, 0),
    new TrainingSample(0.12, 0),
    new TrainingSample(0.18, 0),
    new TrainingSample(0.22, 0),
    new TrainingSample(0.28, 0),
    new TrainingSample(0.32, 0),
    new TrainingSample(0.38, 0),
    new TrainingSample(0.42, 0),
    new TrainingSample(0.48, 0),
    new TrainingSample(0.52, 1),
    new TrainingSample(0.58, 1),
    new TrainingSample(0.62, 1),
    new TrainingSample(0.68, 1),
    new TrainingSample(0.72, 1),
    new TrainingSample(0.78, 1),
    new TrainingSample(0.82, 1),
    new TrainingSample(0.88, 1),
    new TrainingSample(0.92, 1)
};

            ModelEvaluator evaluator = new ModelEvaluator();

            evaluator.Evaluate(
                server.GlobalWeight,
                server.GlobalBias,
                testData);

        }// end of Main


    }// end of class Program

}// end of namespace FederatedPoisoningDetector
