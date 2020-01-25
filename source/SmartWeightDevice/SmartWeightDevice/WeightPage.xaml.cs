using System;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class WeightPage : Window
    {
        MainWindowViewModel _viewModel;

        public WeightPage()
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }
    }
}
