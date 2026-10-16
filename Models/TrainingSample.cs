
namespace FederatedPoisoningDetector.Models
{
    public class TrainingSample
    {
        public double Temperature { get; set; }

        public int FireDetected { get; set; }

        public TrainingSample(double temperature, int fireDetected)
        {
            Temperature = temperature;
            FireDetected = fireDetected;
        }
    }// end of class TrainingSample
}// end of namespace FederatedPoisoningDetector.Models

