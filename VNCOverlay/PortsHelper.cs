using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VNCOverlay
{
    public class PortsHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PortsHelper> _logger;
        private const int DefaultPort = 5900;
        public List<int> _ports { get; private set; } = new List<int>();

        public PortsHelper(IConfiguration configuration, ILogger<PortsHelper> logger)
        {
            _configuration = configuration;
            _logger = logger;
            LoadPorts();
        }

        public void LoadPorts()
        {
            _ports.Clear();
            try
            {
                string portsValue = _configuration["PortNumber"];
                if (!string.IsNullOrWhiteSpace(portsValue))
                {
                    foreach (var port in portsValue.Split(','))
                    {
                        if (int.TryParse(port.Trim(), out int portNumber))
                        {
                            _ports.Add(portNumber);
                        }
                        else
                        {
                            _logger.LogWarning($"Invalid port '{port}' in configuration. This entry will be ignored.");
                        }
                    }
                }

                if (_ports.Count == 0)
                {
                    _ports.Add(DefaultPort);
                    _logger.LogInformation("No valid port numbers found in configuration. Using default port 5900.");
                }
            }
            catch (Exception ex)
            {
                _ports.Add(DefaultPort);
                _logger.LogError(ex, "Error reading 'PortNumber' from configuration. Using default port 5900.");
            }
        }

        public void UpdatePorts(string portsText)
        {
            _ports.Clear();
            try
            {
                if (!string.IsNullOrWhiteSpace(portsText))
                {
                    foreach (var port in portsText.Split(','))
                    {
                        if (int.TryParse(port.Trim(), out int portNumber))
                        {
                            _ports.Add(portNumber);
                        }
                        else
                        {
                            _logger.LogWarning($"Invalid port '{port}' entered. This entry will be ignored.");
                        }
                    }
                }
                _logger.LogInformation("Ports Updated successfully.");
                SavePorts();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Updating ports.");
            }
        }

        public void SavePorts()
        {
            try
            {
                // Assuming you have a method to update the configuration
                string portsValue = string.Join(",", _ports);
                // Update your configuration file with the new portsValue
                Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings["PortNumber"] != null)
                {
                    config.AppSettings.Settings["PortNumber"].Value = portsValue;
                }
                else
                {
                    config.AppSettings.Settings.Add("PortNumber", portsValue);
                }
                config.Save(ConfigurationSaveMode.Modified);
                System.Configuration.ConfigurationManager.RefreshSection("appSettings");

                _logger.LogInformation("Port saved successfully.");
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error Saving ports");
            }
        }
    }
}
