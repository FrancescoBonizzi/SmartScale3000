namespace SmartWeightDevice.Domain
{
    public class WeightedObjectInfos
    {
        public WeightedObjectInfos(
            RecognizedObjects recognizedObject,
            double weightKilograms,
            double calories,
            double pricePerKgEuro,
            string barCodePath,
            string mainImagePath)
        {
            RecognizedObject = recognizedObject;
            WeightKilograms = weightKilograms;
            Calories = calories;
            PricePerKgEuro = pricePerKgEuro;
            BarCodePath = barCodePath;
            MainImagePath = mainImagePath;

        }

        public RecognizedObjects RecognizedObject { get; }
        public double WeightKilograms { get; }
        public double Calories { get; set; }
        public double Joules => Calories * 4.184;
        public double PricePerKgEuro { get; set; }
        public double PriceEuro => PricePerKgEuro * WeightKilograms;
        public string BarCodePath { get; set; }
        public string MainImagePath { get; set; }
    }
}
