using System.Linq;
using Microsoft.ML;
using ImageClassification.ImageDataStructures;
using static ImageClassification.ModelScorer.ModelHelpers;
using System.Diagnostics;

namespace ImageClassification.ModelScorer
{
    public class TFModelScorer
    {
        private readonly MLContext mlContext;
        private static string ImageReal = nameof(ImageReal);

        public TFModelScorer()
        {
            mlContext = new MLContext();
        }

        public struct ImageNetSettings
        {
            public const int imageHeight = 224;
            public const int imageWidth = 224;
            public const float mean = 117;
            public const bool channelsLast = true;
        }

        public struct InceptionSettings
        {
            public const string inputTensorName = "input";
            public const string outputTensorName = "softmax2";
        }

        public PredictionEngine<ImageNetData, ImageNetPrediction> LoadModel(string dataLocation, string imagesFolder, string modelLocation)
        {
            Debug.WriteLine($"Model location: {modelLocation}");
            Debug.WriteLine($"Images folder: {imagesFolder}");
            Debug.WriteLine($"Training file: {dataLocation}");
            Debug.WriteLine($"Default parameters: image size=({ImageNetSettings.imageWidth},{ImageNetSettings.imageHeight}), image mean: {ImageNetSettings.mean}");

            var data = mlContext.Data.LoadFromTextFile<ImageNetData>(dataLocation, hasHeader: true);

            var pipeline = mlContext.Transforms.LoadImages(
                outputColumnName: "input",
                imageFolder: imagesFolder,
                inputColumnName: nameof(ImageNetData.ImagePath))
                    .Append(mlContext.Transforms.ResizeImages(
                        outputColumnName: "input", imageWidth: ImageNetSettings.imageWidth, imageHeight: ImageNetSettings.imageHeight, inputColumnName: "input"))
                    .Append(mlContext.Transforms.ExtractPixels(
                        outputColumnName: "input", interleavePixelColors: ImageNetSettings.channelsLast, offsetImage: ImageNetSettings.mean))
                    .Append(mlContext.Model.LoadTensorFlowModel(modelLocation).ScoreTensorFlowModel(
                        outputColumnNames: new[] { "softmax2" },
                        inputColumnNames: new[] { "input" },
                        addBatchDimensionInput: true));

            ITransformer model = pipeline.Fit(data);

            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageNetData, ImageNetPrediction>(model);

            return predictionEngine;
        }

        public ImageNetDataProbability PredictSingleImageDataUsingModel(
            string imagePath, 
            string labelsLocation,
            PredictionEngine<ImageNetData, ImageNetPrediction> model)
        {
            Debug.WriteLine($"Images folder: {imagePath}");
            Debug.WriteLine($"Labels folder: {labelsLocation}");
            var labels = ReadLabels(labelsLocation);
            var testData = new ImageNetData { ImagePath = imagePath };

            var probs = model.Predict(testData).PredictedLabels;
            var imageData = new ImageNetDataProbability()
            {
                ImagePath = testData.ImagePath,
                Label = string.Empty
            };
            (imageData.PredictedLabel, imageData.Probability) = GetBestLabel(labels, probs);
            return imageData;
        }
    }
}
