using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VNCOverlay
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private SettingsViewModel viewModel;
        private readonly PortsHelper _portsHelper;
        private readonly OverlayHelper _overlayHelper;

        public SettingsWindow(PortsHelper portsHelper, OverlayHelper overlayHelper)
        {
            InitializeComponent();
            viewModel = new SettingsViewModel();
            this.DataContext = viewModel;
            _portsHelper = portsHelper;
            _overlayHelper = overlayHelper;

            // Subscribe to the overlay status changed event
            _overlayHelper.OverlayStatusChanged += OnOverlayStatusChanged;

            // Set initial color based on the overlay's current status
            UpdateEllipseColor(_overlayHelper.IsOverlayActive);
        }

        private void PortNumberTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Enable or disable the Save button based on validation state
            if (Validation.GetHasError(PortNumberTxt))
            {
                SaveBtn.IsEnabled = false;
            }
            else
            {
                SaveBtn.IsEnabled = true;
            }
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            // Save the port number to the configuration file
            _portsHelper.UpdatePorts(viewModel.PortNumber);
            this.Close();
        }

        private void BasicBtn_Click(object sender, RoutedEventArgs e)
        {
            _overlayHelper.UpdateOverlayType("basic");
        }

        private void CenterBtn_Click(object sender, RoutedEventArgs e)
        {
            _overlayHelper.UpdateOverlayType("center");
        }

        private void LoudBtn_Click(object sender, RoutedEventArgs e)
        {
            _overlayHelper.UpdateOverlayType("loud");
        }

        private void OnOverlayStatusChanged(bool isActive)
        {
            UpdateEllipseColor(isActive);
        }

        private void UpdateEllipseColor(bool isActive)
        {
            if (isActive)
            {
                OverlayActive.Fill = new SolidColorBrush(Colors.Green);
            }
            else
            {
                OverlayActive.Fill = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
