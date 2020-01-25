using GalaSoft.MvvmLight;
using SmartWeightDevice.Domain;
using SmartWeightDevice.Extensions;
using System;
using System.Collections.ObjectModel;
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

        public void GenerateData(double weight, RecognizedObjects recognizedObject)
        {
            var scaleCalculator = new ScaleCoreCalculator();
            var recognizedObjectInfos = scaleCalculator.Calculate(weight, recognizedObject);

            WeightString = $"{Math.Round(recognizedObjectInfos.WeightKilograms, 3)}Kg";
            RecognizedObjectNameString = recognizedObject.DisplayText();
            PriceString = $"{Math.Round(recognizedObjectInfos.PriceEuro, 2)}€";
            PricePerKiloString = $"{Math.Round(recognizedObjectInfos.PricePerKgEuro, 3)}€/Kg";
            CaloriesString = $"{Math.Round(recognizedObjectInfos.Calories, 0)}cal / {Math.Round(recognizedObjectInfos.Joules, 0)}J";
            FruitImagePath = recognizedObjectInfos.MainImagePath;
            BarcodeImagePath = recognizedObjectInfos.BarCodePath;
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
