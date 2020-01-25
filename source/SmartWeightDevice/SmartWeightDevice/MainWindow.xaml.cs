using System;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Notifications.Add(DateTime.Now.ToString());
        }
    }
}
