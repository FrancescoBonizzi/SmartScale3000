using SmartWeightDevice.Extensions;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class StartingPage : Window
    {
        public StartingPage()
        {
            InitializeComponent();

            // Start ascolto coda
        }

        private void Window_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO STOP Ascolto coda

            var weightPage = new WeightPage(123);
            weightPage.ShowDialog();

            // TODO START ascolto coda
        }
    }
}
