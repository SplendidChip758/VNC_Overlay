using System.Configuration;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace VNCOverlayConfigUI
{
    public partial class Form1 : Form
    {

        private ErrorProvider errorProvider = new ErrorProvider();

        public Form1()
        {
            InitializeComponent();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink; // Optional: set blink behavior
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfiguration(txtPortNumber.Text);
            lblStatus.Text = "Configuration saved!";
        }

        private void txtPortNumber_TextChanged(object sender, EventArgs e)
        {
            string pattern = @"^(\d+)(,\d+)*$"; // Regex pattern for comma-separated integers
            if (Regex.IsMatch(txtPortNumber.Text, pattern))
            {
                btnSave.Enabled = true; // Enable the update button if format is correct
                errorProvider.SetError(txtPortNumber, ""); // Clear any previous error messages
            }
            else
            {
                btnSave.Enabled = false; // Disable the update button if format is incorrect
                errorProvider.SetError(txtPortNumber, "Invalid format. Please enter numbers separated by commas.");
            }
        }


        private void SaveConfiguration(string portNumber)
        {
            // Properly locate the service's configuration file assuming it is in the same directory
#if DEBUG
            string exePath = Path.Combine("C:\\Program Files\\SplendidChip758 VNCOverlay", "VNCOverlay.exe");
#else
            string exePath = Path.Combine(Application.StartupPath, "VNCOverlay.exe");
#endif

            Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
            if (config.AppSettings.Settings["PortNumber"] != null)
            {
                config.AppSettings.Settings["PortNumber"].Value = portNumber;
            }
            else
            {
                config.AppSettings.Settings.Add("PortNumber", portNumber);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
            lblStatus.Text = "Configuration Saved";
        }

        private void btnStartService_Click(object sender, EventArgs e)
        {
            ControlService("start");
        }

        private void btnStopService_Click(object sender, EventArgs e)
        {
            ControlService("stop");           
        }

        private void btnRestartService_Click(object sender, EventArgs e)
        {
            ControlService("restart");           
        }

        private void ControlService(string command)
        {
            using (ServiceController service = new ServiceController("VNCOverlay"))
            {
                try
                {
                    switch (command)
                    {
                        case "start":
                            if (service.Status != ServiceControllerStatus.Running)
                            {
                                service.Start();
                                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                                lblStatus.Text = "Service Started.";
                            }
                            break;
                        case "stop":
                            if (service.Status != ServiceControllerStatus.Stopped)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                                lblStatus.Text = "Service Stopped.";
                            }
                            break;
                        case "restart":
                            if (service.Status != ServiceControllerStatus.Stopped)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                            }
                            service.Start();
                            service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                            lblStatus.Text = "Service Restarted.";
                            break;
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Error: " + ex.Message;
                }
            }
        }

    }
}
