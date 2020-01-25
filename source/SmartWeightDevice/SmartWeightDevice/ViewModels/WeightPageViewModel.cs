using GalaSoft.MvvmLight;
using SmartWeightDevice.Domain;
using SmartWeightDevice.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SmartWeightDevice.ViewModels
{
    public class WeightPageViewModel : ViewModelBase
    {
        public ObservableCollection<string> Notifications { get; set; }
        public string Date { get; set; }

        public string WeightString { get; set; }
        public string RecognizedObjectNameString { get; set; }
        public string PriceString { get; set; }
        public string PricePerKiloString { get; set; }
        public string CaloriesString { get; set; }
        public string FruitImagePath { get; set; }
        public string BarcodeImagePath { get; set; }

        public RecognizedObjects RecognizedObject { get; set; }

        private readonly DispatcherTimer _timer;
        private const string _hourFormat = "HH:mm:ss";
        private const string _dateFormat = "dd MMMM";

        public WeightPageViewModel(
            double weight,
            RecognizedObjects recognizedObject)
        {
            Notifications = new ObservableCollection<string>();
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _timer.Tick += _timer_Tick;
            _timer.Start();

            Date = DateTime.Now.ToString(_dateFormat);
            Hour = DateTime.Now.ToString(_hourFormat);

            GenerateData(weight, recognizedObject);
        }

        public void GenerateData(
            double weight, 
            RecognizedObjects recognizedObject)
        {
            RecognizedObject = recognizedObject;

            if (recognizedObject == RecognizedObjects.Unrecognized)
            {
                WeightString = string.Empty;
                RecognizedObjectNameString = string.Empty;
                PriceString = string.Empty;
                PricePerKiloString = string.Empty;
                CaloriesString = string.Empty;
                FruitImagePath = string.Empty;
                BarcodeImagePath = string.Empty;
                return;
            }

            var scaleCalculator = new ScaleCoreCalculator();
            var recognizedObjectInfos = scaleCalculator.Calculate(weight, recognizedObject);

            WeightString = $"{Math.Round(recognizedObjectInfos.WeightKilograms, 3):0.000}Kg";
            RecognizedObjectNameString = recognizedObject.DisplayText();
            PriceString = $"{Math.Round(recognizedObjectInfos.PriceEuro, 2)}€";
            PricePerKiloString = $"{Math.Round(recognizedObjectInfos.PricePerKgEuro, 3)}€/Kg";
            CaloriesString = $"{Math.Round(recognizedObjectInfos.Calories, 0)}cal / {Math.Round(recognizedObjectInfos.Joules, 0)}J";
            FruitImagePath = recognizedObjectInfos.MainImagePath;
            BarcodeImagePath = recognizedObjectInfos.BarCodePath;

            Task.Run(async () => await AddFakeNotifications(recognizedObject));
        }

        private async Task AddFakeNotifications(RecognizedObjects recognizedObject)
        {
            await Task.Delay(1500);
            Application.Current.Dispatcher.Invoke(
                 DispatcherPriority.Background,
                 new Action(() =>
                 {
                     switch (recognizedObject)
                     {
                         case RecognizedObjects.Apple:
                             Notifications.Add("If you buy other 200gr, you will get a 20% discount!");
                             Thread.Sleep(500);
                             Notifications.Add("There are also pears in discount!");
                             break;

                         case RecognizedObjects.Orange:
                             Notifications.Add("Good choice! In this period C-Vitamin is important!");
                             break;

                         case RecognizedObjects.Banana:
                             Notifications.Add("Buongustaio!");
                             break;
                     }
             }));
        }

        private void _timer_Tick(object sender, EventArgs e)
            => Hour = DateTime.Now.ToString(_hourFormat);

        private string _hour;
        public string Hour
        {
            get => _hour;
            set
            {
                if (_hour == value)
                    return;

                _hour = value;
                RaisePropertyChanged(nameof(Hour));
            }
        }
    }
}
