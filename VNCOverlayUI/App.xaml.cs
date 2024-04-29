using System;
using System.IO.Pipes;
using System.IO;
using System.Windows;
using System.Threading.Tasks;
using System.Security.AccessControl;
using System.Security.Principal;

namespace VNCOverlayUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow mainWindow;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the main window but do not show it immediately.
            mainWindow = new MainWindow();

            // Start the pipe server to listen for commands from the service.
            StartPipeServer();
        }

        private void StartPipeServer()
        {
            Task.Run(() => {
                PipeSecurity pipeSecurity = new PipeSecurity();
                SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
                pipeSecurity.AddAccessRule(new PipeAccessRule(sid, PipeAccessRights.ReadWrite, AccessControlType.Allow));

                using (var server = new NamedPipeServerStream("VNCOverlayPipe", PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity))
                {
                    using (var reader = new StreamReader(server))
                    {
                        while (true)
                        {
                            server.WaitForConnection();

                            string command;
                            while ((command = reader.ReadLine()) != null)
                            {
                                switch (command.ToLower())
                                {
                                    case "show":
                                        Dispatcher.Invoke(() =>
                                        {
                                            // Ensure the main window is initialized and call show
                                            mainWindow?.Show();
                                            mainWindow?.Activate(); // Bring window to front
                                        });
                                        break;
                                    case "hide":
                                        Dispatcher.Invoke(() =>
                                        {
                                            // Ensure the main window is initialized and call hide
                                            mainWindow?.Hide();
                                        });
                                        break;
                                }
                            }
                            server.Disconnect();
                        }
                    }
                }
            });
        }
    }
}
