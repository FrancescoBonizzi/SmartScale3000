using GalaSoft.MvvmLight;
using SmartWeightDevice.Domain;
using SmartWeightDevice.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SmartWeightDevice.ViewModels
{
    public class WeightPageViewModel : ViewModelBase
    {
        public class NiceNotification
        {
            public NiceNotification(string text, string color)
            {
                Text = text;
                Color = color;
            }

            public static implicit operator NiceNotification(string val) => new NiceNotification(val, "LightGreen");

            public string Text { get; set; }
            public string Color { get; set; }
            
        }

        public ObservableCollection<NiceNotification> Notifications { get; set; }
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
            Notifications = new ObservableCollection<NiceNotification>();
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
                     var mfi = new System.Globalization.DateTimeFormatInfo();
                     string strMonthName = mfi.GetMonthName(DateTime.Now.Month).ToString();
                     switch (recognizedObject)
                     {
                         case RecognizedObjects.Apple:
                             Notifications.Add($"Apples are a seasonal fruit in {strMonthName}!");
                             Notifications.Add("If you buy 200gr more, you'll get a 20% discount!");
                             Thread.Sleep(500);
                             Notifications.Add("Pears are discounted too!");
                             break;

                         case RecognizedObjects.Orange:
                             if (!new[] { 1, 6, 7, 8, 9, 10, 11, 12 }.Any(a => a == DateTime.Now.Month))
                                 Notifications.Add($"Oranges are a seasonal fruit in {strMonthName}!");
                             else
                                 Notifications.Add(new NiceNotification($"You are buying off-season fruits. Oranges are in season from February to May.", "Khaki"));
                             Notifications.Add("Good choice! In this period Vitamin C is important!");
                             break;

                         case RecognizedObjects.Banana:
                             Notifications.Add($"Bananas are a seasonal fruit in {strMonthName}!");
                             Notifications.Add("Buongustaio!");
                             break;

                         case RecognizedObjects.Strawberry:
                             if (!new[] { 2, 3, 4, 5 }.Any(a => a == DateTime.Now.Month))
                                 Notifications.Add($"Stawberries are a seasonal fruit in {strMonthName}!");
                             else
                                 Notifications.Add(new NiceNotification($"You are buying off-season fruits. Strawberries are in season from February to May.", "Khaki"));
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
