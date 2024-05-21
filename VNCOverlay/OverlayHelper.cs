using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace VNCOverlay
{
    public class OverlayHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OverlayHelper> _logger;
        private readonly MainWindow _mainWindow;
        public string OverlayType { get; private set; }

        public OverlayHelper(IConfiguration configuration, ILogger<OverlayHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;

            _mainWindow = new MainWindow();

            LoadOverlayType();
        }

        public void LoadOverlayType()
        {
            try
            {
                string overlayType = _configuration["Overlay"];
                overlayType = overlayType.ToLower();
                if (!string.IsNullOrWhiteSpace(overlayType) && (overlayType == "basic" || overlayType == "center" || overlayType == "loud"))
                {
                    OverlayType = overlayType;
                }
                else
                {
                    OverlayType = "Basic";
                }
            }
            catch (Exception ex)
            {
                OverlayType = "Basic";
            }
        }

        public void UpdateOverlayType(string overlayType)
        {
            if (!string.IsNullOrWhiteSpace(overlayType) && (overlayType == "basic" || overlayType == "center" || overlayType == "loud"))
            {
                OverlayType = overlayType;

            }
            else
            {
                OverlayType = "Basic";
            }
        }

        public void SaveOverlayType(string overlayType)
        {
            _configuration["Overlay"] = overlayType;
        }

        public void ShowOverlay()
        {
            switch (OverlayType.ToLower())
            {
                case "basic":
                    _mainWindow.ShowOverlayBasic();
                    break;
                case "center":
                    _mainWindow.ShowOverlayCenter();
                    break;
                case "loud":
                    _mainWindow.ShowOverlayLoud();
                    break;
            }

            _mainWindow.Show();
        }

        public void HideOverlay()
        {
            _mainWindow.Hide();
            _mainWindow.HideOverlays();
        }

        public void RefreshOverlay()
        {
            if (_mainWindow.IsActive)
            {
                HideOverlay();
                ShowOverlay();
            }
        }
    }
}
