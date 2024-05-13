using System;
using System.Configuration;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace VNCOverlayUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Window currentOverlay;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the set window but do not show it immediately.
            ConfigureOverlay();
#if BITMAP
            currentOverlay.Show();
            CaptureAndSaveVisual(currentOverlay, "overlayPreview.png");
            Application.Current.Shutdown();
#else
            // Start the pipe server to listen for commands from the service.
            StartPipeServer();
#endif
        }

        public void CaptureAndSaveVisual(Window window, string filename)
        {
            try
            {
                // Define the actual size
                int width = (int)window.ActualWidth;
                int height = (int)window.ActualHeight;
                double dpiX = 96.0 * VisualTreeHelper.GetDpi(window).DpiScaleX;
                double dpiY = 96.0 * VisualTreeHelper.GetDpi(window).DpiScaleY;

                double scaleX = 1;
                double scaleY = 1;

                // Create a RenderTargetBitmap with scaled dimensions
                RenderTargetBitmap renderTarget = new RenderTargetBitmap(
                    (int)(width * scaleX),
                    (int)(height * scaleY),
                    dpiX, dpiY, PixelFormats.Pbgra32);

                // Create a DrawingVisual to use for rendering
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext context = visual.RenderOpen())
                {
                    // Apply the scale transform
                    VisualBrush brush = new VisualBrush(window);
                    context.PushTransform(new ScaleTransform(scaleX, scaleY));
                    context.DrawRectangle(brush, null, new Rect(new Point(), new Size(width, height)));
                    context.Pop();
                }

                // Render the scaled visual
                renderTarget.Render(visual);

                // Encode the image to a PNG (could be JPEG or other formats)
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                // Save the image to a file
                using (FileStream file = File.Create(filename))
                {
                    encoder.Save(file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error capturing screen: " + ex.Message);
            }
        }

        private void ConfigureOverlay()
        {
            ConfigurationManager.RefreshSection("appSettings");
            string defaultOverlay = ConfigurationManager.AppSettings["Overlay"];
            switch (defaultOverlay)
            {
                case "MainWindow":
                    currentOverlay = new MainOverlay();
                    break;
                case "OverlayBasic":
                    currentOverlay = new OverlayBasic();
                    break;
                case "OverlayLoud":
                    currentOverlay = new OverlayLoud();
                    break;
                case "OverlayCenter":
                    currentOverlay = new OverlayCenter();
                    break;
                default:
                    currentOverlay = new MainOverlay(); // Fallback to MainWindow
                    break;
            }
        }

        private async void StartPipeServer()
        {
            var pipeSecurity = new PipeSecurity();
            var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
            pipeSecurity.AddAccessRule(new PipeAccessRule(sid, PipeAccessRights.ReadWrite, AccessControlType.Allow));

            while (true)
            {
                try
                {
                    using (var server = new NamedPipeServerStream("VNCOverlayPipe", PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 0, 0, pipeSecurity))
                    {
                        await Task.Factory.FromAsync(server.BeginWaitForConnection, server.EndWaitForConnection, null);
                        using (var reader = new StreamReader(server))
                        {
                            string command;
                            while ((command = await reader.ReadLineAsync()) != null)
                            {
                                await ProcessCommandAsync(command);
                            }
                        }
                        server.Disconnect();
                    }
                }
                catch (Exception ex)
                {
                    // Log the error
                    Console.WriteLine($"Error in pipe server: {ex.Message}");
                    // Optional: Decide whether to retry connection, pause, or handle the error differently
                }
            }
        }

        private async Task ProcessCommandAsync(string command)
        {
            switch (command.ToLower())
            {
                case "show":
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {                      
                        ShowCurrentOverlay();
                    });
                    break;
                case "hide":
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {                       
                        HideCurrentOverlay();
                    });
                    break;
                case "update":
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        HideCurrentOverlay();
                        ConfigureOverlay();
                        ShowCurrentOverlay();
                    });
                    break;
            }
        }

        // Ensure this is accessible for the StartPipeServer method
        public void ShowCurrentOverlay()
        {
            Dispatcher.Invoke(() =>
            {
                currentOverlay?.Show();
                currentOverlay?.Activate(); // Bring window to front
                SendVisibilityStatus(true);
            });
        }

        public void HideCurrentOverlay()
        {
            Dispatcher.Invoke(() =>
            {
                currentOverlay?.Hide();
                SendVisibilityStatus(false);
            });
        }

        private void SendVisibilityStatus(bool isVisible)
        {
            using (var client = new NamedPipeClientStream(".", "VNCOverlayStatusPipe", PipeDirection.Out))
            {
                try
                {
                    client.Connect(1000);  // Shorter timeout for status updates

                    using (var writer = new StreamWriter(client))
                    {
                        writer.WriteLine(isVisible ? "visible" : "hidden");
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., logging)
                    Console.WriteLine($"Failed to send visibility status: {ex.Message}");
                }
            }
        }

    }
}
