using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Timers;
using System.Configuration;

namespace VNCOverlay
{
    public partial class VNCOverlay : ServiceBase
    {       
        private EventLog _eventLog;
        private Timer checkPortTimer;
        private bool isOverlayVisible = false;
        private int portNumber;

        public VNCOverlay()
        {
            InitializeComponent();
            Debugger.Launch();

            if (!EventLog.SourceExists("VNCOverlay"))
            {
                // Create the source if it does not exist.
                EventLog.CreateEventSource("VNCOverlay", "VNCOverlayLog");
            }
            else
            {
                // Check which log it is registered to
                string logName = EventLog.LogNameFromSourceName("VNCOverlay", ".");
                if (logName != "VNCOverlayLog")
                {
                    // Delete and recreate the source if registered incorrectly
                    EventLog.DeleteEventSource("VNCOverlay");
                    EventLog.CreateEventSource("VNCOverlay", "VNCOverlayLog");
                }
            }

            _eventLog = new EventLog();
            _eventLog.Source = "VNCOverlay";
            _eventLog.Log = "VNCOverlayLog";
            _eventLog.WriteEntry("Your log message here", EventLogEntryType.Information);

            // Try to read the port number from the configuration file
            try
            {
                string portValue = ConfigurationManager.AppSettings["PortNumber"];
                if (!int.TryParse(portValue, out portNumber))
                {
                    // If parsing fails or the value is null, set to default
                    portNumber = 5900; // Set your desired default port number
                    _eventLog.WriteEntry($"Failed to read 'PortNumber' from app.config. Using default port {portNumber}.", EventLogEntryType.Warning);
                }
            }
            catch (Exception ex)
            {
                portNumber = 5900; // Set your desired default port number
                _eventLog.WriteEntry($"Error reading 'PortNumber' from app.config. Using default port {portNumber}. Exception: {ex.Message}", EventLogEntryType.Error);
            }

            checkPortTimer = new Timer(10000); // Set the interval to 10 seconds (10000 milliseconds)
            checkPortTimer.Elapsed += (sender, e) => CheckPortUsage(portNumber); // Replace 12345 with your port
            checkPortTimer.AutoReset = true;
            checkPortTimer.Enabled = true;

            _eventLog.WriteEntry("Service started and monitoring port.");
        }

        protected override void OnStart(string[] args)
        {
            checkPortTimer.Start();
            _eventLog.WriteEntry("Service started and is now monitoring port.");
        }

        protected override void OnStop()
        {
            checkPortTimer.Stop();
            SendCommandToOverlay("hide"); // Ensure overlay is hidden when service stops
            _eventLog.WriteEntry("Service stopped.");
        }

        private void CheckPortUsage(int port)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "netstat";
                psi.Arguments = "-an";
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;
                psi.CreateNoWindow = true;

                using (Process process = Process.Start(psi))
                {
                    using (StreamReader reader = process.StandardOutput)
                    {
                        string result = reader.ReadToEnd();
                        FindPortInNetstatOutput(result, port);
                    }
                }
            }
            catch (Exception ex)
            {
                _eventLog.WriteEntry("Error running netstat: " + ex.Message, EventLogEntryType.Error);
            }
        }

        private void FindPortInNetstatOutput(string output, int port)
        {
            string searchString = $":{port} ";
            bool isPortInUse = false;

            using (StringReader reader = new StringReader(output))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(searchString) && line.Contains("ESTABLISHED"))
                    {
                        isPortInUse = true;
                        break;
                    }
                }
            }

            // Use named pipe communication to control the overlay visibility
            if (isPortInUse && !isOverlayVisible)
            {
                SendCommandToOverlay("show");
                isOverlayVisible = true;
            }
            else if (!isPortInUse && isOverlayVisible)
            {
                SendCommandToOverlay("hide");
                isOverlayVisible = false;
            }
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
