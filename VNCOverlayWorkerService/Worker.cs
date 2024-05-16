using System.Diagnostics;
using System.Net.NetworkInformation;

namespace VNCOverlayWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private EventLog _eventLog;
        private List<int> ports = new List<int>();       
        public HashSet<int> _activeConnections;
        private bool _overlayVisible;

        private const string EventLogSource = "VNCOverlay";
        private const string EventLogName = "VNCOverlayLog";

        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _activeConnections = new HashSet<int>();

            InitializeEventLog();
            InitializePorts();
        }

        private void InitializeEventLog()
        {
            if (!EventLog.SourceExists(EventLogSource))
            {
                EventLog.CreateEventSource(EventLogSource, EventLogName);
            }
            _eventLog = new EventLog
            {
                Source = EventLogSource,
                Log = EventLogName
            };
        }

        private void InitializePorts()
        {
            // Read the port numbers from the configuration file
            try
            {
                string portsValue = _configuration["PortNumber"];
                if (!string.IsNullOrWhiteSpace(portsValue))
                {
                    foreach (var port in portsValue.Split(','))
                    {
                        if (int.TryParse(port.Trim(), out int portNumber))
                        {
                            ports.Add(portNumber);
                        }
                        else
                        {
                            _logger.LogWarning($"Invalid port '{port}' in configuration. This entry will be ignored.");
                            _eventLog.WriteEntry($"Invalid port '{port}' in configuration. This entry will be ignored.", EventLogEntryType.Warning);
                        }
                    }
                }

                if (ports.Count == 0)
                {
                    ports.Add(5900); // Default port
                    _logger.LogInformation("No valid port numbers found in configuration. Using default port 5900.");
                    _eventLog.WriteEntry("No valid port numbers found in configuration. Using default port 5900.", EventLogEntryType.Warning);
                }
            }
            catch (Exception ex)
            {
                ports.Add(5900); // Set your desired default port number
                _logger.LogError(ex, "Error reading 'PortNumber' from configuration. Using default port 5900.");
                _eventLog.WriteEntry($"Error reading 'PortNumber' from configuration. Using default port 5900. Exception: {ex.Message}", EventLogEntryType.Error);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _eventLog.WriteEntry("Worker running at: " + DateTimeOffset.Now, EventLogEntryType.Information);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Check active TCP connections
                    var activeConnections = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections();
                    var establishedConnections = activeConnections.Where(conn => ports.Contains(conn.LocalEndPoint.Port) && conn.State == TcpState.Established);

                    foreach (var conn in establishedConnections)
                    {
                        if (_activeConnections.Add(conn.LocalEndPoint.Port))
                        {
                            _logger.LogInformation($"Established connection found on port {conn.LocalEndPoint.Port} at: {DateTimeOffset.Now}");
                            _eventLog.WriteEntry($"Established connection found on port {conn.LocalEndPoint.Port} at: {DateTimeOffset.Now}", EventLogEntryType.Information);

                            if (!_overlayVisible)
                            {
                                
                            }                           
                        }
                    }

                    // Remove connections that are no longer established
                    _activeConnections.RemoveWhere(port => !establishedConnections.Any(conn => conn.LocalEndPoint.Port == port));

                    // If no active connections, send "Hide" command
                    if (_activeConnections.Count == 0 && _overlayVisible)
                    {
                        
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred.");
                    _eventLog.WriteEntry($"An error occurred: {ex.Message}", EventLogEntryType.Error);
                }            
            }
        }    
    }
}
