
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Federation
{
    public class ModelEvaluator
    {
        private static double Sigmoid(double x)
        {
            // Numerically stable sigmoid.
            if (x >= 0)
            {
                return 1.0 / (1.0 + Math.Exp(-x));
            }

            double expX = Math.Exp(x);
            return expX / (1.0 + expX);
        }

        public void Evaluate(
            double weight,
            double bias,
            List<TrainingSample> testData)
        {
            if (testData == null || testData.Count == 0)
            {
                throw new ArgumentException(
                    "Evaluation data cannot be empty.");
            }

            int truePositive = 0;
            int trueNegative = 0;
            int falsePositive = 0;
            int falseNegative = 0;

            foreach (var sample in testData)
            {
                double probability = Sigmoid(
                    weight * sample.Temperature + bias);

                int prediction = probability >= 0.5 ? 1 : 0;
                int actual = sample.FireDetected;

                if (prediction == 1 && actual == 1)
                    truePositive++;
                else if (prediction == 0 && actual == 0)
                    trueNegative++;
                else if (prediction == 1 && actual == 0)
                    falsePositive++;
                else
                    falseNegative++;
            }

            int total = testData.Count;

            double accuracy =
                (double)(truePositive + trueNegative) / total;

            double precision =
                truePositive + falsePositive > 0
                ? (double)truePositive /
                    (truePositive + falsePositive)
                : 0.0;

            double recall =
                truePositive + falseNegative > 0
                ? (double)truePositive /
                    (truePositive + falseNegative)
                : 0.0;

            double f1Score =
                precision + recall > 0
                ? 2.0 * precision * recall /
                    (precision + recall)
                : 0.0;

            Console.WriteLine();
            Console.WriteLine("GLOBAL MODEL EVALUATION");
            Console.WriteLine("-----------------------");

            Console.WriteLine("Confusion Matrix:");
            Console.WriteLine(
                $"True Positives : {truePositive}");
            Console.WriteLine(
                $"True Negatives : {trueNegative}");
            Console.WriteLine(
                $"False Positives: {falsePositive}");
            Console.WriteLine(
                $"False Negatives: {falseNegative}");

            Console.WriteLine();
            Console.WriteLine($"Accuracy : {accuracy:P2}");
            Console.WriteLine($"Precision: {precision:P2}");
            Console.WriteLine($"Recall   : {recall:P2}");
            Console.WriteLine($"F1-Score : {f1Score:P2}");
        }
    }
}
