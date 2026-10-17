
using System;
using System.Collections.Generic;
using FederatedPoisoningDetector.Models;
using FederatedPoisoningDetector.Simulation;
using FederatedPoisoningDetector.Federation;
using FederatedPoisoningDetector.Attacks;
using FederatedPoisoningDetector.Data;

namespace FederatedPoisoningDetector
{
    internal class Program
    {
        // Federated learning configuration.
        private const int TrainingRounds = 10;
        private const int LocalEpochs = 100;
        private const double LearningRate = 0.1;

        static void Main(string[] args)
        {
            DisplayWelcomeMessage();

            int experimentChoice = SelectExperiment();

            List<Uav> uavs = CreateFiveUavs();

            FederatedServer server = CreateFederatedServer();

            RunFederatedTraining(
                uavs,
                server,
                experimentChoice);

            EvaluateGlobalModel(server);

        }// end of Main method

        // Display the application heading.
        private static void DisplayWelcomeMessage()
        {
            Console.WriteLine("Federated Poisoning Detector");
            Console.WriteLine("----------------------------");
            Console.WriteLine();

        }// end of DisplayWelcomeMessage method

        // Allow the user to select an experiment.
        private static int SelectExperiment()
        {
            Console.WriteLine("Select experiment:");
            Console.WriteLine();
            Console.WriteLine("1. Clean FedAvg");
            Console.WriteLine("2. Sign-Flipping Attack");
            Console.WriteLine("3. Gaussian-Noise Attack");
            Console.WriteLine("4. Model-Scaling Attack");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Enter choice (1-4): ");

                if (int.TryParse(Console.ReadLine(), out int choice)
                    && choice >= 1 && choice <= 4)
                {
                    Console.WriteLine();
                    return choice;
                }

                Console.WriteLine("Invalid selection. Try again.");
            }
        }// end of SelectExperiment method

        // Create five UAVs with different local datasets.
        private static List<Uav> CreateFiveUavs()
        {
            return new List<Uav>
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
        }// end of CreateFiveUavs method

        // Create the federated learning server.
        private static FederatedServer CreateFederatedServer()
        {
            return new FederatedServer();
        }

        // Execute the configured federated training rounds.
        private static void RunFederatedTraining(
            List<Uav> uavs,
            FederatedServer server,
            int experimentChoice)
        {
            for (int round = 1; round <= TrainingRounds; round++)
            {
                List<ModelUpdate> updates =
                    CollectModelUpdates(
                        uavs,
                        server,
                        round,
                        experimentChoice);



                string datasetDirectory = "datasets";

                System.IO.Directory.CreateDirectory(datasetDirectory);

                string csvPath = System.IO.Path.Combine(
                    datasetDirectory,
                    "updates.csv");

                ModelUpdateCsvWriter.Write(
                    csvPath,
                    updates,
                    server.GlobalWeight,
                    server.GlobalBias);


                server.Aggregate(updates);

                DisplayRoundResults(round, server);
            }

            Console.WriteLine();
            Console.WriteLine("Federated training completed.");
        }// end of RunFederatedTraining method

        // Collect local model updates from all UAVs.
        private static List<ModelUpdate> CollectModelUpdates(
            List<Uav> uavs,
            FederatedServer server,
            int round,
            int experimentChoice)
        {
            var updates = new List<ModelUpdate>();

            foreach (Uav uav in uavs)
            {
                // Synchronise with the current global model.
                uav.SetModel(
                    server.GlobalWeight,
                    server.GlobalBias);

                // Perform local training.
                uav.Train(LocalEpochs, LearningRate);

                // Generate the local model update.
                ModelUpdate update = uav.GenerateUpdate(round);

                // Apply the selected attack where appropriate.
                update = ApplySelectedAttack(
                    uav,
                    update,
                    server,
                    experimentChoice);

                updates.Add(update);
            }

            return updates;
        }// end of CollectModelUpdates method

        // Apply a poisoning attack to compromised UAV 3.
        private static ModelUpdate ApplySelectedAttack(
            Uav uav,
            ModelUpdate update,
            FederatedServer server,
            int experimentChoice)
        {
            // Only UAV 3 is compromised.
            if (uav.Id != 3)
            {
                return update;
            }

            switch (experimentChoice)
            {
                case 2:
                    return SignFlippingAttack.Apply(
                        update,
                        server.GlobalWeight,
                        server.GlobalBias,
                        strength: 5.0);

                case 3:
                    return GaussianNoiseAttack.Apply(
                        update,
                        noiseStrength: 2.0);

                case 4:
                    return ModelScalingAttack.Apply(
                        update,
                        server.GlobalWeight,
                        server.GlobalBias,
                        strength: 5.0);

                default:
                    // Clean FedAvg: no attack.
                    return update;
            }
        }// end of ApplySelectedAttack method

        // Display the global model after each round.
        private static void DisplayRoundResults(
            int round,
            FederatedServer server)
        {
            Console.WriteLine(
                $"Round {round,2}: " +
                $"Weight = {server.GlobalWeight:F4}, " +
                $"Bias = {server.GlobalBias:F4}");
        }

        // Evaluate the final global model.
        private static void EvaluateGlobalModel(
            FederatedServer server)
        {
            List<TrainingSample> testData =
                CreateEvaluationDataset();

            ModelEvaluator evaluator = new ModelEvaluator();

            evaluator.Evaluate(
                server.GlobalWeight,
                server.GlobalBias,
                testData);

        }// end of EvaluateGlobalModel method

        // Create the common evaluation dataset.
        private static List<TrainingSample> CreateEvaluationDataset()
        {
            return new List<TrainingSample>
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
        }// end of CreateEvaluationDataset method

    }// end of class Program

}// end of namespace FederatedPoisoningDetector
