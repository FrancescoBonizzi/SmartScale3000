using System.Collections.ObjectModel;

namespace SmartWeightDevice
{
    public class MainWindowViewModel
    {
        public ObservableCollection<string> Notifications { get; set; }

        public MainWindowViewModel()
        {
            Notifications = new ObservableCollection<string>();
        }
    }
}
