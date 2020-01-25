using BarcodeLib;
using SmartWeightDevice.Domain;
using SmartWeightDevice.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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
            var weightedObjectInfos = new WeightedObjectInfos(
                recognizedObject: recognizedObject,
                weightKilograms: (double)weightGrams / 1_000.0,
                calories: (_caloriesPerGram[recognizedObject] * weightGrams),
                pricePerKgEuro: _pricesPerKilo[recognizedObject],
                mainImagePath: recognizedObject.MainImagePath());

            var barcodeText = Math.Round(weightedObjectInfos.PriceEuro * 10000, 0).ToString().PadLeft(12, '0');
            var barcodeGenerator = new Barcode();
            
            var barcodeImage = barcodeGenerator.Encode(
                TYPE.UPCA,
                barcodeText,
                Color.Black,
                Color.White,
                290,
                120);
            var filename = Path.GetFullPath($"{Guid.NewGuid().ToString("N")}.jpg");
            barcodeImage.Save(filename);

            weightedObjectInfos.BarCodePath = filename;

            return weightedObjectInfos;
        }
    }
}
