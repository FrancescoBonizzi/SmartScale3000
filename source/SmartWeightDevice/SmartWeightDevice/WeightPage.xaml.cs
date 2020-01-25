using SmartWeightDevice.Extensions;
using SmartWeightDevice.ViewModels;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SmartWeightDevice
{
    public partial class WeightPage : Window
    {
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

            if (viewModel.RecognizedObject == Domain.RecognizedObjects.Unrecognized)
            {
                gridBottom.Visibility = Visibility.Collapsed;
                gridMainContent.Visibility = Visibility.Collapsed;
                gridErrorMessage.Visibility = Visibility.Visible;
            }

            Task.Run(async () => await CloseAfter());
        }

        private async Task CloseAfter()
        {
            await Task.Delay(8000);
            Application.Current.Dispatcher.Invoke(
                 DispatcherPriority.Background,
                 new Action(() =>
                 {
                     Close();
                 }));
        }
    }
}
