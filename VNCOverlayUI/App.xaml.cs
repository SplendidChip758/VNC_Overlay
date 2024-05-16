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
        private MainOverlay _overlayWindow;
        private NamedPipeServer _pipeServer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _overlayWindow = new MainOverlay();          

            // Initialize the set window but do not show it immediately.
            ConfigureOverlay();
#if BITMAP
            _overlayWindow.Show();
            CaptureAndSaveVisual(currentOverlay, "overlayPreview.png");          
            Application.Current.Shutdown();
#else
            // Start the pipe server to listen for commands from the service.
            _pipeServer = new NamedPipeServer("StatusPipe", "OverlayPipe", HandleCommand);
            StartPipeServer();
#endif
        }

        private async void StartPipeServer()
        {
            await _pipeServer.ReceiveCommandAsync();
        }

        private void ConfigureOverlay()
        {
            ConfigurationManager.RefreshSection("appSettings");
            string defaultOverlay = ConfigurationManager.AppSettings["Overlay"];
            
            switch (defaultOverlay.ToLower())
            {
                case "main":
                     _overlayWindow.ShowOverlayMain();
                    break;
                case "basic":
                    _overlayWindow.ShowOverlayBasic();
                    break;
                case "center":
                    _overlayWindow.ShowOverlayCenter();
                    break;
                case "loud":
                    _overlayWindow.ShowOverlayLoud();
                    break;
                default:
                    _overlayWindow.ShowOverlayMain();
                    break;
            }
        }

        private void HandleCommand(string command)
        {
            Dispatcher.Invoke(() =>
            {
                switch (command.ToLower())
                {
                    case "show":
                        ShowOverlay();
                        break;
                    case "hide":
                        HideOverlay();
                        break;
                    case "update":
                        HideOverlay();
                        ConfigureOverlay();
                        ShowOverlay();
                        break;
                }             
            });
        }

        // Ensure this is accessible for the StartPipeServer method
        public void ShowOverlay()
        {
            Dispatcher.Invoke(() =>
            {
                _overlayWindow.Show();
                _overlayWindow.Activate(); // Bring window to front              
                _pipeServer.SendStatusAsync("Visible");
            });
        }

        public void HideOverlay()
        {
            Dispatcher.Invoke(() =>
            {
                _overlayWindow.Hide();
                _pipeServer.SendStatusAsync("Hidden");
            });
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
    }
}
