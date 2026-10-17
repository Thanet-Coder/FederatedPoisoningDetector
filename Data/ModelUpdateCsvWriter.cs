
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Data
{
    public static class ModelUpdateCsvWriter
    {

        public static void Write(
            string filePath,
            List<ModelUpdate> updates,
            double globalWeight,
            double globalBias,
            int simulationId = 1)
        {
            bool fileExists = File.Exists(filePath);

            using StreamWriter writer =
                new StreamWriter(filePath, append: true);

            if (!fileExists || new FileInfo(filePath).Length == 0)
            {
                writer.WriteLine(
                    "SimulationId,UavId,Round,WeightDelta,BiasDelta," +
                    "UpdateMagnitude,SampleCount,IsMalicious");
            }

            foreach (ModelUpdate update in updates)
            {
                double weightDelta =
                    update.Weight - globalWeight;

                double biasDelta =
                    update.Bias - globalBias;

                double magnitude = Math.Sqrt(
                    weightDelta * weightDelta +
                    biasDelta * biasDelta);

                writer.WriteLine(string.Join(",",
                    simulationId,
                    update.UavId,
                    update.Round,
                    weightDelta.ToString("R", CultureInfo.InvariantCulture),
                    biasDelta.ToString("R", CultureInfo.InvariantCulture),
                    magnitude.ToString("R", CultureInfo.InvariantCulture),
                    update.SampleCount,
                    update.IsMalicious ? 1 : 0));
            }
        }// end of Write method

    }// end of ModelUpdateCsvWriter class

}// end of FederatedPoisoningDetector.Data namespace
