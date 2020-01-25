using SmartWeightDevice.Extensions;
using SmartWeightDevice.ViewModels;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class WeightPage : Window
    {
        private readonly WeightPageViewModel _viewModel;

        public WeightPage(WeightPageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            Opacity = 0;
            this.Animate(
              from: null,
              to: 1,
              duration: 1500,
              propertyPath: nameof(Opacity));
        }
    }
}
