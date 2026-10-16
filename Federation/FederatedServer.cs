
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Federation
{
    public class FederatedServer
    {
        public double GlobalWeight { get; private set; }

        public double GlobalBias { get; private set; }

        public FederatedServer()
        {
            GlobalWeight = 0.0;
            GlobalBias = 0.0;
        }// end of constructor


        public void Aggregate(List<ModelUpdate> updates)
        {
            if (updates == null || updates.Count == 0)
            {
                throw new ArgumentException(
                    "At least one model update is required.");
            }

            int totalSamples = updates.Sum(
                update => update.SampleCount);

            if (totalSamples <= 0)
            {
                throw new ArgumentException(
                    "Total sample count must be positive.");
            }

            double weightedWeight = 0.0;
            double weightedBias = 0.0;

            foreach (var update in updates)
            {
                weightedWeight +=
                    update.Weight * update.SampleCount;

                weightedBias +=
                    update.Bias * update.SampleCount;
            }

            GlobalWeight = weightedWeight / totalSamples;
            GlobalBias = weightedBias / totalSamples;
        }// end of Aggregate method


    }// end of class FederatedServer
}// end of namespace FederatedPoisoningDetector.Federation
