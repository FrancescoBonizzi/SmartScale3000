using SmartWeightDevice.Domain;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class StartingPage : Window
    {
        public StartingPage()
        {
            InitializeComponent();

            // TODO START ascolto coda
        }

        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO STOP Ascolto coda

            // START LOADER

            // TODO Fai foto

            var weightPage = new WeightPage(new ViewModels.WeightPageViewModel(
                800,
                RecognizedObjects.Apple));
            weightPage.ShowDialog();

            // TODO START ascolto coda
        }
    }
}
