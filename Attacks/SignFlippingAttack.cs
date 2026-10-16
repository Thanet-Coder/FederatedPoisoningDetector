using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Attacks
{
    public static class SignFlippingAttack
    {
        public static ModelUpdate Apply(
            ModelUpdate update,
            double globalWeight,
            double globalBias,
            double strength = 5.0)
        {
            if (strength <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(strength),
                    "Attack strength must be positive.");
            }

            double weightDelta = update.Weight - globalWeight;
            double biasDelta = update.Bias - globalBias;

            return new ModelUpdate
            {
                UavId = update.UavId,
                Round = update.Round,

                Weight = globalWeight - strength * weightDelta,
                Bias = globalBias - strength * biasDelta,

                SampleCount = update.SampleCount,
                IsMalicious = true
            };
        }// end of Apply method

    }// end of class SignFlippingAttack

}// end of namespace FederatedPoisoningDetector.Attacks
