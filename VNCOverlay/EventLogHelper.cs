using System.Diagnostics;

namespace VNCOverlay
{
    public static class EventLogHelper
    {
        private const string DefaultEventLogSource = "VNCOverlay";
        private const string DefaultEventLogName = "VNCOverlayLog";

        public static EventLog InitializeEventLog(string source = DefaultEventLogSource, string logName = DefaultEventLogName)
        {
            if (!EventLog.SourceExists(source))
            {
                EventLog.CreateEventSource(source, logName);
            }

            return new EventLog
            {
                Source = source,
                Log = logName
            };
        }
    }
}
