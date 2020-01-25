using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Threading;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;
using ImageClassification.ModelScorer;
using Microsoft.ML;
using ImageClassification.ImageDataStructures;

namespace SWScreen
{
    public partial class MainWindow : System.Windows.Window
    {
        TFModelScorer Model;
        PredictionEngine<ImageNetData, ImageNetPrediction> PredictionEngine;
        VideoCapture Capture;
        Mat Frame;
        bool IsCameraRunning = false;
        private readonly BackgroundWorker _worker = new BackgroundWorker();
        private string _assetsPath = GetAbsolutePath("assets");

        public MainWindow()
        {
            var tagsTsv = Path.Combine(_assetsPath, "inputs", "images", "tags.tsv");
            var imagesFolder = Path.Combine(_assetsPath, "inputs", "images");
            var inceptionPb = Path.Combine(_assetsPath, "inputs", "inception", "tensorflow_inception_graph.pb");

            Model = new TFModelScorer();
            PredictionEngine = Model.LoadModel(tagsTsv, imagesFolder, inceptionPb);

            _worker.DoWork += WorkerOnDoWork;
            _worker.RunWorkerCompleted += WorkerCompleted;
            Model = new TFModelScorer();
            PredictionEngine = Model.LoadModel(tagsTsv, imagesFolder, inceptionPb);
            Directory.CreateDirectory("pictures");
        }

        
        private void WorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var pictureSource = CaptureCameraCallback();
            doWorkEventArgs.Result = pictureSource;
        }

        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs completedEventArgs)
        {
            try
            {
                if (completedEventArgs.Error != null)
                {
                    LogException(completedEventArgs.Error);
                }
                if (completedEventArgs.Result != null && completedEventArgs.Result is Bitmap)
                {
                    var picture = (Bitmap)completedEventArgs.Result;

                    var pictureName = Guid.NewGuid();
                    if (ImageBox.Source != null)
                        ImageBox.Source = null;

                    picture.Save(Path.Combine("pictures", pictureName + ".jpg"));

                    //ImageBox.Source = BitmapToImageSource(pictureForBox);

                    var labelsTxt = Path.Combine(_assetsPath, "inputs", "inception", "imagenet_comp_graph_label_strings.txt");
                    var prediction = Model.PredictSingleImageDataUsingModel(GetAbsolutePath(Path.Combine("pictures", pictureName + ".jpg")), labelsTxt, PredictionEngine);
                    picture.Dispose();
                    //File.Delete("picture.jpg");

                    FruitLabel.Content = $"This fruit is {prediction.PredictedLabel}.";
                    PrecisionLabel.Content = $"Precision of {prediction.Probability}.";
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
            finally
            {
                TakePhotoBtn.IsEnabled = true;
            }
        }

        private Bitmap CaptureCameraCallback()
        {

            Frame = new Mat();
            Capture = new VideoCapture(0);
            Capture.Open(0);

            if (Capture.IsOpened())
            {
                while (IsCameraRunning)
                {
                    Capture.Read(Frame);
                    var bitmap = BitmapConverter.ToBitmap(Frame);
                    Capture.Dispose();
                    return bitmap;
                }
            }

            return null;
        }
        
        private void TakePhotoBtn_Click(object sender, RoutedEventArgs e)
        {
            TakePhotoBtn.IsEnabled = false;

            _worker.RunWorkerAsync();
                IsCameraRunning = true;
        }
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        public static string GetAbsolutePath(string relativePath)
        {
            var _dataRoot = new FileInfo(typeof(MainWindow).Assembly.Location);
            string assemblyFolderPath = _dataRoot.Directory.FullName;
            string fullPath = Path.Combine(assemblyFolderPath, relativePath);
            return fullPath;
        }

        private void LogException(Exception ex)
        {
            Debug.Write(ex.Message);
            Debug.Write(ex.StackTrace);
            Debug.Write(ex.InnerException?.Message);
            Debug.Write(ex.InnerException?.StackTrace);
        }

    }
}
