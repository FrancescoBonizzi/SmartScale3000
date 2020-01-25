using SmartWeightDevice.Extensions;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class WeightPage : Window
    {
        private readonly MainWindowViewModel _viewModel;

        public WeightPage(int weight)
        {
            InitializeComponent();
            _viewModel = new MainWindowViewModel(weight);
            DataContext = _viewModel;

            Opacity = 0;
            this.Animate(
              from: null,
              to: 1,
              duration: 1500,
              propertyPath: nameof(Opacity));
        }
    }
}
