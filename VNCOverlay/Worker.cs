using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Configuration;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Windows.Media;

namespace VNCOverlay
{
    public class Worker : BackgroundService
    {       
        private readonly ILogger<Worker> _logger;
        private readonly EventLog _eventLog;
        private readonly PortsHelper _portsHelper;
        private readonly HashSet<int> _activeConnections = new HashSet<int>();
        private bool _overlayVisible = false;       

        public Worker(ILogger<Worker> logger, EventLog eventLog, PortsHelper portsHelper)
        {
            _logger = logger;
            _eventLog = eventLog;
            _portsHelper = portsHelper;
                       
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
                    var establishedConnections = activeConnections.Where(conn => _portsHelper._ports.Contains(conn.LocalEndPoint.Port) && conn.State == TcpState.Established);

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
