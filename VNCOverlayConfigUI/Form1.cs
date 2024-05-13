using System.Configuration;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO.Pipes;

namespace VNCOverlayConfigUI
{
    public partial class Form1 : Form
    {
        private ErrorProvider errorProvider = new ErrorProvider();
        private EventLog _eventLog;

        public Form1()
        {
            InitializeComponent();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink; // Optional: set blink behavior

            if (!EventLog.SourceExists("VNCOverlayConfig"))
            {
                // Create the source if it does not exist.
                EventLog.CreateEventSource("VNCOverlayConfig", "VNCOverlayLog");
            }
            else
            {
                // Check which log it is registered to
                string logName = EventLog.LogNameFromSourceName("VNCOverlayConfig", ".");
                if (logName != "VNCOverlayLog")
                {
                    // Delete and recreate the source if registered incorrectly
                    EventLog.DeleteEventSource("VNCOverlayConfig");
                    EventLog.CreateEventSource("VNCOverlayConfig", "VNCOverlayLog");
                }
            }

            _eventLog = new EventLog();
            _eventLog.Source = "VNCOverlayConfig";
            _eventLog.Log = "VNCOverlayLog";
            _eventLog.WriteEntry("its working");
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
            string exePath = Path.Combine("E:\\source\\repos\\VNCOverlay\\VNCOverlay\\bin\\Release", "VNCOverlay.exe");
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

        private void btnOverlayBasic_Click(object sender, EventArgs e)
        {
            //StopOverlayUI();
            UpdateOverlay("OverlayBasic");
            //StartOverlayUI();
            SendCommandToOverlay("update");
        }

        private void btnOverlayLoud_Click(object sender, EventArgs e)
        {
            //StopOverlayUI();
            UpdateOverlay("OverlayLoud");
            //StartOverlayUI();
            SendCommandToOverlay("update");
        }

        private void btnOverlayCenter_Click(object sender, EventArgs e)
        {
            //StopOverlayUI();
            UpdateOverlay("OverlayCenter");
            //StartOverlayUI();
            SendCommandToOverlay("update");
        }

        private void UpdateOverlay(string window)
        {
            // Properly locate the service's configuration file assuming it is in the same directory
#if DEBUG
            string exePath = Path.Combine("E:\\source\\repos\\VNCOverlay\\VNCOverlayUI\\bin\\Debug", "VNCOverlayUI.exe");
#else
            string exePath = Path.Combine(Application.StartupPath, "VNCOverlayUI.exe");
#endif

            Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);
            if (config.AppSettings.Settings["Overlay"] != null)
            {
                config.AppSettings.Settings["Overlay"].Value = window;
            }
            else
            {
                config.AppSettings.Settings.Add("Overlay", window);
            }
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");


            lblStatus.Text = "overlay updated";
        }

        private void SendCommandToOverlay(string command)
        {
            using (var client = new NamedPipeClientStream(".", "VNCOverlayPipe", PipeDirection.Out))
            {
                try
                {
                    if (!client.IsConnected)
                    {
                        client.Connect(5000); // Wait 5000 milliseconds to connect
                    }

                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine(command);
                        writer.Flush();
                    }
                }
                catch (System.TimeoutException tex)
                {
                    _eventLog.WriteEntry($"Connection timeout: {tex.Message}", EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    _eventLog.WriteEntry($"Failed to send command '{command}' to overlay: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }
    }
}
