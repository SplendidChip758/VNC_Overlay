using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace VNCOverlay
{
    public class EventLogHelper
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EventLogHelper> _logger;
        public EventLog EventLog { get; private set; }

        private const string DefaultEventLogSource = "VNCOverlay";
        private const string DefaultEventLogName = "VNCOverlayLog";

        public EventLogHelper(IConfiguration configuration, ILogger<EventLogHelper> logger)
        {
            _logger = logger;
            _configuration = configuration;
            InitializeEventLog();
        }

        private void InitializeEventLog()
        {
            string source = _configuration["EventLogSource"] ?? DefaultEventLogSource;
            string logName = _configuration["EventLogName"] ?? DefaultEventLogName;

            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }

            EventLog = new EventLog
            {
                Source = source,
                Log = logName
            };
        }
    }
}
