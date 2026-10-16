
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Attacks
{
    public static class ModelScalingAttack
    {
        public static ModelUpdate Apply(
            ModelUpdate update,
            double globalWeight,
            double globalBias,
            double strength = 5.0)
        {
            if (!double.IsFinite(strength) || strength <= 1.0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(strength),
                    "Attack strength must be finite and greater than 1.");
            }

            // Calculate the legitimate local model changes.
            double weightDelta =
                update.Weight - globalWeight;

            double biasDelta =
                update.Bias - globalBias;

            // Amplify the changes without reversing direction.
            double scaledWeight =
                globalWeight + strength * weightDelta;

            double scaledBias =
                globalBias + strength * biasDelta;

            return new ModelUpdate
            {
                UavId = update.UavId,
                Round = update.Round,
                Weight = scaledWeight,
                Bias = scaledBias,
                SampleCount = update.SampleCount,
                IsMalicious = true
            };
        }// end of Apply method

    }// end of class ModelScalingAttack

}// end of namespace FederatedPoisoningDetector.Attacks
