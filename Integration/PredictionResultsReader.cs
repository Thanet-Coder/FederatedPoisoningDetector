using System.Linq;
using System;
using System.Globalization;
using System.IO;

namespace FederatedPoisoningDetector.Integration
{
    public class PredictionResultsReader
    {
        public void DisplaySuspiciousUpdates(string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                Console.WriteLine("Prediction file not found.");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("SUSPICIOUS UAV MODEL UPDATES");
            Console.WriteLine("----------------------------");

            int suspiciousCount = 0;

            foreach (string line in File.ReadLines(csvPath).Skip(1))
            {
                string[] fields = line.Split(',');

                if (fields.Length != 5)
                {
                    continue;
                }

                if (fields[3] != "1")
                {
                    continue;
                }

                string uavId = fields[1];
                string round = fields[2];

                double probability = double.Parse(
                    fields[4],
                    CultureInfo.InvariantCulture);

                Console.WriteLine(
                    $"UAV {uavId}, Round {round}: " +
                    $"Suspicious ({probability:P0})");

                suspiciousCount++;
            }

            Console.WriteLine();
            Console.WriteLine(
                $"Total suspicious updates: {suspiciousCount}");
        }
    }
}
