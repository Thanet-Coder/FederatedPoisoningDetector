
using System;
using System.Collections.Generic;
using FederatedPoisoningDetector.Models;
using FederatedPoisoningDetector.Federation;
using FederatedPoisoningDetector.Attacks;
using FederatedPoisoningDetector.Data;

namespace FederatedPoisoningDetector.Simulation
{
    public class FederatedSimulationRunner
    {
        private const int TrainingRounds = 10;
        private const int LocalEpochs = 100;
        private const double LearningRate = 0.1;




        public void Run(
            List<Uav> uavs,
            FederatedServer server,
            int experimentChoice,
            bool displayResults = true,
            string? csvPath = "datasets/updates.csv",
            int simulationId = 1,
            int compromisedUavId = 3,
            double? attackStrength = null)


        {
            // Initialise a reproducible Gaussian-noise sequence
            // for this simulation.
            GaussianNoiseAttack.SetSeed(42 + simulationId);

            if (csvPath != null)
            {
                string? directory =
                    System.IO.Path.GetDirectoryName(csvPath);

                if (!string.IsNullOrEmpty(directory))
                {
                    System.IO.Directory.CreateDirectory(directory);
                }
            }





            for (int round = 1; round <= TrainingRounds; round++)
            {


                List<ModelUpdate> updates =
                    CollectModelUpdates(
                        uavs,
                        server,
                        round,
                        experimentChoice,
                        compromisedUavId,
                        attackStrength);


                // Export before global aggregation.
                if (csvPath != null)
                {
                    ModelUpdateCsvWriter.Write(
                        csvPath,
                        updates,
                        server.GlobalWeight,
                        server.GlobalBias,
                        simulationId);
                }


                server.Aggregate(updates);

                if (displayResults)
                {
                    DisplayRoundResults(round, server);
                }
            }

            if (displayResults)
            {
                Console.WriteLine();
                Console.WriteLine("Federated training completed.");
            }
        }// end of Run method



        private List<ModelUpdate> CollectModelUpdates(
            List<Uav> uavs,
            FederatedServer server,
            int round,
            int experimentChoice,
            int compromisedUavId,
            double? attackStrength)


        {
            var updates = new List<ModelUpdate>();

            foreach (Uav uav in uavs)
            {
                uav.SetModel(
                    server.GlobalWeight,
                    server.GlobalBias);

                uav.Train(LocalEpochs, LearningRate);

                ModelUpdate update = uav.GenerateUpdate(round);



                update = ApplySelectedAttack(
                    uav,
                    update,
                    server,
                    experimentChoice,
                    compromisedUavId,
                    attackStrength);



                updates.Add(update);
            }

            return updates;
        }// end of CollectModelUpdates method



        private ModelUpdate ApplySelectedAttack(
            Uav uav,
            ModelUpdate update,
            FederatedServer server,
            int experimentChoice,
            int compromisedUavId,
            double? attackStrength)
        {
            if (uav.Id != compromisedUavId)
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
                        strength: attackStrength ?? 5.0);

                case 3:
                    return GaussianNoiseAttack.Apply(
                        update,
                        noiseStrength: attackStrength ?? 2.0);

                case 4:
                    return ModelScalingAttack.Apply(
                        update,
                        server.GlobalWeight,
                        server.GlobalBias,
                        strength: attackStrength ?? 5.0);

                default:
                    return update;
            }
        }// end of ApplySelectedAttack method



        private void DisplayRoundResults(
            int round,
            FederatedServer server)
        {
            Console.WriteLine(
                $"Round {round,2}: " +
                $"Weight = {server.GlobalWeight:F4}, " +
                $"Bias = {server.GlobalBias:F4}");
        }// end of DisplayRoundResults method

    }// end of FederatedSimulationRunner class

}// end of namespace
