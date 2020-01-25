using ImageClassification.ImageDataStructures;
using ImageClassification.ModelScorer;
using Microsoft.ML;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScaleMessagesManager;
using SmartWeightDevice.Domain;
using System;
using System.Drawing;
using System.IO;

namespace SmartWeightDevice
{
    public partial class StartingPage : System.Windows.Window
    {
        private ScaleManager _scaleManager;

        public StartingPage()
        {
            InitializeComponent();
            InitializeScale();

            FinalWeightArrived(800);
        }

        private void InitializeScale()
        {
            _scaleManager = new ScaleManager(
                WeightArrived,
                FinalWeightArrived);
            _scaleManager.StartListening();
        }

        private void StopLoader()
        {

        }

        private void WeightArrived(double weight)
        {
            // TODO Aggiorno il loader
        }

        private void FinalWeightArrived(double weight)
        {
            // Stoppo l'ascolto della bilancia
            _scaleManager.StopListening();

            // Prendo l'immagine
            var image = CaptureCameraCallback();
            var fruitsDirectory = "picturesFruits";
            Directory.CreateDirectory(fruitsDirectory);
            var imagePath = Path.GetFullPath($@"{fruitsDirectory}/{image}-{Guid.NewGuid().ToString("N")}.jpg");
            image.Save(imagePath);

            // Riconoscimento
            var fruit = DoRecognizeObject(imagePath);

            StopLoader();

            // Mostro il risultato
            var weightPage = new WeightPage(new ViewModels.WeightPageViewModel(
                weight,
                fruit));
            weightPage.ShowDialog();

            // Mi rimetto in ascolto
            InitializeScale();
        }

        private RecognizedObjects DoRecognizeObject(string imagePath)
        {
            var assets = Path.GetFullPath("assets");
            var tagsTsv = Path.Combine(assets, "inputs", "images", "tags.tsv");
            var imagesFolder = Path.Combine(assets, "inputs", "images");
            var inceptionPb = Path.Combine(assets, "inputs", "inception", "tensorflow_inception_graph.pb");
            var labelsTxt = Path.Combine(assets, "inputs", "inception", "imagenet_comp_graph_label_strings.txt");

            var model = new TFModelScorer();
            var predictionEngine = model.LoadModel(tagsTsv, imagesFolder, inceptionPb);

            var prediction = model.PredictSingleImageDataUsingModel(
                imagePath, 
                labelsTxt, 
                predictionEngine);

            switch(prediction.PredictedLabel)
            {
                case "orange":
                    return RecognizedObjects.Orange;

                case "banana":
                    return RecognizedObjects.Banana;

                default:
                    return RecognizedObjects.Apple;
            }
        }

        private Bitmap CaptureCameraCallback()
        {
            var frame = new Mat();
            var capture = new VideoCapture(0);
            capture.Open(0);

            if (capture.IsOpened())
            {
                capture.Read(frame);
                return BitmapConverter.ToBitmap(frame);
            }

            return null;
        }

    }
}
