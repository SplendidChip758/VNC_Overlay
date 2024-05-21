using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace VNCOverlay
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly EventLogHelper _eventLogHelper;
        private readonly PortsHelper _portsHelper;
        private readonly OverlayHelper _overlayHelper;
        private readonly HashSet<int> _activeConnections = new HashSet<int>();
        private bool _overlayVisible = false;

        public Worker(ILogger<Worker> logger, EventLogHelper eventLogHelper, PortsHelper portsHelper, OverlayHelper overlayHelper)
        {
            _logger = logger;
            _eventLogHelper = eventLogHelper;
            _portsHelper = portsHelper;
            _overlayHelper = overlayHelper;

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            _eventLogHelper.EventLog.WriteEntry("Worker running at: " + DateTimeOffset.Now, EventLogEntryType.Information);

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
                            _eventLogHelper.EventLog.WriteEntry($"Established connection found on port {conn.LocalEndPoint.Port} at: {DateTimeOffset.Now}", EventLogEntryType.Information);

                            if (!_overlayVisible)
                            {
                                _overlayHelper.ShowOverlay();
                                _overlayVisible = true;
                            }
                        }
                    }

                    // Remove connections that are no longer established
                    _activeConnections.RemoveWhere(port => !establishedConnections.Any(conn => conn.LocalEndPoint.Port == port));

                    // If no active connections, send "Hide" command
                    if (_activeConnections.Count == 0 && _overlayVisible)
                    {
                        _overlayHelper.HideOverlay();
                        _overlayVisible = false;
                    }

                    await Task.Delay(1000, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred.");
                    _eventLogHelper.EventLog.WriteEntry($"An error occurred: {ex.Message}", EventLogEntryType.Error);
                }
            }
        }
    }
}
