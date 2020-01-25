using SmartWeightDevice.Domain;
using SmartWeightDevice.Extensions;
using System.Collections.Generic;

namespace SmartWeightDevice
{
    public class ScaleCoreCalculator
    {
        private readonly Dictionary<RecognizedObjects, double> _pricesPerKilo = new Dictionary<RecognizedObjects, double>()
        {
            [RecognizedObjects.Apple] = 1.98,
            [RecognizedObjects.Banana] = 1.98,
            [RecognizedObjects.Orange] = 2.78,
        };

        private readonly Dictionary<RecognizedObjects, double> _caloriesPerGram = new Dictionary<RecognizedObjects, double>()
        {
            [RecognizedObjects.Apple] = 0.52,
            [RecognizedObjects.Banana] = 0.89,
            [RecognizedObjects.Orange] = 0.47,
        };

        public WeightedObjectInfos Calculate(
            double weightGrams,
            RecognizedObjects recognizedObject)
        {
            return new WeightedObjectInfos(
                recognizedObject: recognizedObject,
                weightKilograms: (double)weightGrams / 1_000.0,
                calories: (_caloriesPerGram[recognizedObject] * weightGrams),
                pricePerKgEuro: _pricesPerKilo[recognizedObject],
                barCodePath: "images/sample-barcode.jpg",
                mainImagePath: recognizedObject.MainImagePath());
        }
    }
}
