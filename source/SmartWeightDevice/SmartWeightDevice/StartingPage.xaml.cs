using ScaleMessagesManager;
using SmartWeightDevice.Domain;
using System.Windows;

namespace SmartWeightDevice
{
    public partial class StartingPage : Window
    {
        private ScaleManager _scaleManager;

        public StartingPage()
        {
            InitializeComponent();
            InitializeComponent();
        }

        private void InitializeScale()
        {
            _scaleManager = new ScaleManager(
                WeightArrived,
                FinalWeightArrived);
            _scaleManager.StartListening();
        }

        private void StopLoader()
        {

        }

        private void WeightArrived(double weight)
        {
            // TODO Aggiorno il loader
        }

        private void FinalWeightArrived(double weight)
        {
            // Stoppo l'ascolto della bilancia
            _scaleManager.StopListening();

            // TODO Fai foto

            StopLoader();

            // Mostro il risultato
            var weightPage = new WeightPage(new ViewModels.WeightPageViewModel(
                weight,
                RecognizedObjects.Apple));
            weightPage.ShowDialog();

            // Mi rimetto in ascolto
            InitializeScale();
        }
    }
}
