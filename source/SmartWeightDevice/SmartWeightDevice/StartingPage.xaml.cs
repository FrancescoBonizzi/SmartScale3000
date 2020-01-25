using ImageClassification.ModelScorer;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using ScaleMessagesManager;
using SmartWeightDevice.Domain;
using SmartWeightDevice.Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace SmartWeightDevice
{
    public partial class StartingPage : System.Windows.Window
    {
        private ScaleManager _scaleManager;
        private const string _title = "SmartWeight3000";
        private double _lastWeight = double.MaxValue;

        public StartingPage()
        {
            InitializeComponent();
            InitializeScale();
        }

        private void InitializeScale()
        {
            txtTitle.Text = _title;

            _scaleManager = new ScaleManager(
                WeightArrived,
                FinalWeightArrived);
            _scaleManager.StartListening();
        }

        private void StopLoader()
        {
            if (txtTitle.Opacity < 1)
            {
                txtTitle.Animate(
                    from: null,
                    to: 0,
                    propertyPath: nameof(Opacity),
                    completed: () =>
                    {
                        txtTitle.Text = _title;
                        txtTitle.Animate(
                            from: null,
                            to: 1,
                            propertyPath: nameof(Opacity));
                    });
            }

            txtPutAProduct.Animate(
                from: null,
                to: 1,
                propertyPath: nameof(Opacity));
        }

        private void WeightArrived(double weightGrams)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          if (weightGrams < 10)
                          {
                              // Mi rimetto in ascolto
                              InitializeScale();

                              if (_lastWeight >= 10)
                              {
                                  StopLoader();
                              }
                          }
                          else
                          {
                              txtTitle.Text = $"{Math.Round(weightGrams, 0)}gr";
                              txtPutAProduct.Animate(
                                  from: null,
                                  to: 0,
                                  propertyPath: nameof(Opacity));
                          }

                          _lastWeight = weightGrams;
                      }));
            }
            catch
            {

            }
        }

        private void FinalWeightArrived(double weight)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action(() =>
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
                    }));
            }
            catch
            {

            }
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

            switch (prediction.PredictedLabel)
            {
                case "orange":
                    return RecognizedObjects.Orange;

                case "banana":
                    return RecognizedObjects.Banana;

                case "apple":
                case "Granny Smith":
                    return RecognizedObjects.Apple;

                default:
                    return RecognizedObjects.Unrecognized;
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
                capture.Dispose();
                return BitmapConverter.ToBitmap(frame);
            }

            return null;
        }

    }
}
