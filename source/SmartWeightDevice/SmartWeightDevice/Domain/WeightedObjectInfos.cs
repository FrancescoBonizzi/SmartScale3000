namespace SmartWeightDevice.Domain
{
    public class WeightedObjectInfos
    {
        public RecognizedObjects RecognizedObject { get; }
        public double WeightKilograms { get; }
        public double Calories { get;  }
        public double Joules => Calories * 4.184;
        public double PricePerKgEuro { get; }
        public double PriceEuro => PricePerKgEuro * WeightKilograms;
        public string BarCodePath { get; set; }
        public string MainImagePath { get; }

        public WeightedObjectInfos(
            RecognizedObjects recognizedObject,
            double weightKilograms,
            double calories,
            double pricePerKgEuro,
            string mainImagePath)
        {
            RecognizedObject = recognizedObject;
            WeightKilograms = weightKilograms;
            Calories = calories;
            PricePerKgEuro = pricePerKgEuro;
            MainImagePath = mainImagePath;
        }
    }
}
