
namespace FederatedPoisoningDetector.Models
{
    public class ModelUpdate
    {
        public int UavId { get; set; }

        public int Round { get; set; }

        public double Weight { get; set; }

        public double Bias { get; set; }

        public int SampleCount { get; set; }

        public bool IsMalicious { get; set; }




    }// end of class ModelUpdate


}// end of namespace FederatedPoisoningDetector.Models

