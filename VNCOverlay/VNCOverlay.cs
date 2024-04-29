using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Lifetime;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace VNCOverlay
{
    public partial class VNCOverlay : ServiceBase
    {

        private Timer checkPortTimer;
        private bool isOverlayVisible = false;

        public VNCOverlay()
        {
            InitializeComponent();
            checkPortTimer = new Timer(10000); // Set the interval to 10 seconds (10000 milliseconds)
            checkPortTimer.Elapsed += (sender, e) => CheckPortUsage(5900); // Replace 12345 with your port
            checkPortTimer.AutoReset = true;
            checkPortTimer.Enabled = true;

            EventLog.WriteEntry("Service started and monitoring port.");
        }

        protected override void OnStart(string[] args)
        {
            checkPortTimer.Start();
            EventLog.WriteEntry("Service started and is now monitoring port.");
        }

        protected override void OnStop()
        {
            checkPortTimer.Stop();
            SendCommandToOverlay("hide"); // Ensure overlay is hidden when service stops
            EventLog.WriteEntry("Service stopped.");
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
                EventLog.WriteEntry("Error running netstat: " + ex.Message, EventLogEntryType.Error);
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
                    EventLog.WriteEntry($"Connection timeout: {tex.Message}", EventLogEntryType.Error);
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry($"Failed to send command '{command}' to overlay: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }

    }
}
