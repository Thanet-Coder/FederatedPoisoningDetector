
using System;
using System.Collections.Generic;
using System.IO;
using FederatedPoisoningDetector.Federation;
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Simulation
{
    public class DatasetGenerator
    {
        public void Generate()
        {
            Random random = new Random(42);

            string outputPath = Path.Combine(
                "datasets",
                "training.csv");

            Directory.CreateDirectory("datasets");

            // Start with a fresh dataset.
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            FederatedSimulationRunner runner =
                new FederatedSimulationRunner();

            int simulationId = 1;

            // Four scenarios: clean and three attack types.
            for (int experimentChoice = 1;
                 experimentChoice <= 4;
                 experimentChoice++)
            {
                // Fifty simulations for each scenario.
                for (int run = 1; run <= 50; run++)
                {
                    List<Uav> uavs = CreateFiveUavs(random);

                    FederatedServer server =
                        new FederatedServer();

                    // Rotate the compromised UAV.
                    int compromisedUavId = ((run - 1) % 5) + 1;


                    double? attackStrength = experimentChoice switch
                    {
                        2 => 2.0 + random.NextDouble() * 4.0,
                        3 => 0.5 + random.NextDouble() * 2.5,
                        4 => 2.0 + random.NextDouble() * 4.0,
                        _ => null
                    };



                    runner.Run(
                        uavs,
                        server,
                        experimentChoice,
                        displayResults: false,
                        csvPath: outputPath,
                        simulationId: simulationId,
                        compromisedUavId: compromisedUavId,
                        attackStrength: attackStrength);


                    Console.WriteLine(
                        $"Simulation {simulationId} completed " +
                        $"(Scenario {experimentChoice}, UAV {compromisedUavId}).");

                    simulationId++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Dataset generation completed.");
            Console.WriteLine($"Output: {outputPath}");
        }// end of Generate method


        private List<Uav> CreateFiveUavs(Random random)
        {
            return new List<Uav>
    {
        new Uav(1, new List<TrainingSample>
        {
            CreateSample(0.10, 0, random),
            CreateSample(0.20, 0, random),
            CreateSample(0.30, 0, random),
            CreateSample(0.70, 1, random)
        }),

        new Uav(2, new List<TrainingSample>
        {
            CreateSample(0.15, 0, random),
            CreateSample(0.25, 0, random),
            CreateSample(0.75, 1, random),
            CreateSample(0.85, 1, random)
        }),

        new Uav(3, new List<TrainingSample>
        {
            CreateSample(0.40, 0, random),
            CreateSample(0.60, 1, random),
            CreateSample(0.80, 1, random),
            CreateSample(0.90, 1, random)
        }),

        new Uav(4, new List<TrainingSample>
        {
            CreateSample(0.05, 0, random),
            CreateSample(0.15, 0, random),
            CreateSample(0.25, 0, random),
            CreateSample(0.35, 0, random)
        }),

        new Uav(5, new List<TrainingSample>
        {
            CreateSample(0.65, 1, random),
            CreateSample(0.75, 1, random),
            CreateSample(0.85, 1, random),
            CreateSample(0.95, 1, random)
        })
    };
        }// end of CreateFiveUavs method



        private double VaryTemperature(
    double temperature,
    int fireDetected,
    Random random)
        {
            // Generate a variation between -0.03 and +0.03.
            double variation =
                (random.NextDouble() * 0.06) - 0.03;

            double adjusted = temperature + variation;

            // Keep temperatures within the expected range
            // and preserve the original fire/no-fire label.
            if (fireDetected == 0)
            {
                return Math.Clamp(adjusted, 0.0, 0.49);
            }

            return Math.Clamp(adjusted, 0.51, 1.0);
        }// end of VaryTemperature method


        private TrainingSample CreateSample(
            double temperature,
            int fireDetected,
            Random random)
        {
            double variedTemperature =
                VaryTemperature(temperature, fireDetected, random);

            return new TrainingSample(
                variedTemperature,
                fireDetected);
        }// end of CreateSample method



    }// end of DatasetGenerator class

}// end of FederatedPoisoningDetector.Simulation namespace
