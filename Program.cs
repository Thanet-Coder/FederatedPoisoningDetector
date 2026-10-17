
using System;
using System.Collections.Generic;
using FederatedPoisoningDetector.Models;
using FederatedPoisoningDetector.Simulation;
using FederatedPoisoningDetector.Federation;
using FederatedPoisoningDetector.Integration;


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

            if (experimentChoice == 5)
            {
                DatasetGenerator generator =
                    new DatasetGenerator();

                generator.Generate();
                return;
            }

            if (experimentChoice == 6)
            {
                RunDetectionExperiment();
                return;
            }

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
            Console.WriteLine("5. Generate Machine Learning Dataset");
            Console.WriteLine("6. Run Python Poisoning Detection");
            Console.WriteLine();

            while (true)
            {
                Console.Write("Enter choice (1-6): ");

                if (int.TryParse(Console.ReadLine(), out int choice)
                    && choice >= 1 && choice <= 6)
                {
                    Console.WriteLine();
                    return choice;
                }

                Console.WriteLine("Invalid selection. Try again.");
            }
        }// end of SelectExperiment method


        private static int SelectDetectionScenario()
        {
            Console.WriteLine();
            Console.WriteLine("Select detection scenario:");
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
        }// end of SelectDetectionScenario method




        private static void RunDetectionExperiment()
        {
            int detectionScenario = SelectDetectionScenario();

            // Assign a unique filename to each detection scenario.
            string scenarioName = detectionScenario switch
            {
                1 => "clean",
                2 => "sign_flipping",
                3 => "gaussian_noise",
                4 => "model_scaling",
                _ => throw new InvalidOperationException("Invalid scenario.")
            };

            string updatesPath = $"datasets/{scenarioName}_updates.csv";
            string predictionsPath = $"datasets/{scenarioName}_predictions.csv";

            // Remove previous results for the selected scenario only.
            if (System.IO.File.Exists(updatesPath))
            {
                System.IO.File.Delete(updatesPath);
            }

            if (System.IO.File.Exists(predictionsPath))
            {
                System.IO.File.Delete(predictionsPath);
            }

            // Create the UAV swarm and federated server.
            List<Uav> uavs = CreateFiveUavs();

            FederatedServer server = CreateFederatedServer();

            FederatedSimulationRunner runner =
                new FederatedSimulationRunner();

            Console.WriteLine("Generating fresh model updates...");
            Console.WriteLine();

            // Run the selected federated learning experiment.
            runner.Run(
                uavs,
                server,
                experimentChoice: detectionScenario,
                displayResults: true,
                csvPath: updatesPath,
                simulationId: 201,
                compromisedUavId: 3,
                attackStrength: 5.0);

            Console.WriteLine();
            Console.WriteLine($"Model updates saved to: {updatesPath}");

            // Run the Python machine learning classifier.
            PythonPredictionRunner pythonRunner =
                new PythonPredictionRunner();

            bool success = pythonRunner.Run(
                updatesPath,
                predictionsPath);

            if (success)
            {
                Console.WriteLine("Python detection completed successfully.");

                PredictionResultsReader resultsReader =
                    new PredictionResultsReader();

                resultsReader.DisplaySuspiciousUpdates(predictionsPath);
            }
            else
            {
                Console.WriteLine("Python detection failed.");
            }

        }// end of RunDetectionExperiment method



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
            FederatedSimulationRunner runner =
                new FederatedSimulationRunner();

            runner.Run(uavs, server, experimentChoice);
        } // end of RunFederatedTraining method

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
