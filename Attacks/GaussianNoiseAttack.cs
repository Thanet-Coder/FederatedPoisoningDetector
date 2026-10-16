
using FederatedPoisoningDetector.Models;

namespace FederatedPoisoningDetector.Attacks
{
    public static class GaussianNoiseAttack
    {
        private static readonly Random RandomGenerator =
            new Random(42);

        private static double NextGaussian()
        {
            // Box-Muller transformation.
            double u1 = 1.0 - RandomGenerator.NextDouble();
            double u2 = 1.0 - RandomGenerator.NextDouble();

            return Math.Sqrt(-2.0 * Math.Log(u1)) *
                   Math.Cos(2.0 * Math.PI * u2);
        }// end of NextGaussian method

        public static ModelUpdate Apply(
            ModelUpdate update,
            double noiseStrength = 2.0)
        {
            if (noiseStrength < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(noiseStrength),
                    "Noise strength cannot be negative.");
            }

            return new ModelUpdate
            {
                UavId = update.UavId,
                Round = update.Round,

                Weight = update.Weight +
                         NextGaussian() * noiseStrength,

                Bias = update.Bias +
                       NextGaussian() * noiseStrength,

                SampleCount = update.SampleCount,
                IsMalicious = true
            };
        }// end of Apply method

    }// end of class GaussianNoiseAttack

}// end of namespace FederatedPoisoningDetector.Attacks
