using System.Configuration;
using System.ServiceProcess;

namespace VNCOverlayConfigUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveConfiguration(txtPortNumber.Text);
            lblStatus.Text = "Configuration saved!";
        }

        private void SaveConfiguration(string portNumber)
        {
            // Properly locate the service's configuration file assuming it is in the same directory
            string exePath = Path.Combine(Application.StartupPath, "VNCOverlay.exe");
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
        }

        private void buttonStartService_Click(object sender, EventArgs e)
        {
            ControlService("start");
            lblStatus.Text = "Service started.";
        }

        private void buttonStopService_Click(object sender, EventArgs e)
        {
            ControlService("stop");
            lblStatus.Text = "Service stopped.";
        }

        private void buttonRestartService_Click(object sender, EventArgs e)
        {
            ControlService("restart");
            lblStatus.Text = "Service restarted.";
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
                                lblStatus.Text = "Service started.";
                            }
                            break;
                        case "stop":
                            if (service.Status != ServiceControllerStatus.Stopped)
                            {
                                service.Stop();
                                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                                lblStatus.Text = "Service stopped.";
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
                            lblStatus.Text = "Service restarted.";
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
