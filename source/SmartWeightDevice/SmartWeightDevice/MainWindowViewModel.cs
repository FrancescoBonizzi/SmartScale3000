using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SmartWeightDevice
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<string> Notifications { get; set; }
        public string Date { get; set; }

        private readonly DispatcherTimer _timer;
        private const string _hourFormat = "HH:mm:ss";
        private const string _dateFormat = "dd MMMM";

        public MainWindowViewModel(int weight)
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
