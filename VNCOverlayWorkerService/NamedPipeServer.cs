using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNCOverlayWorkerService
{
    public class NamedPipeServer
    {
        private readonly string _sendPipeName;
        private readonly string _receivePipeName;
        private readonly ILogger _logger;
        private readonly EventLog _eventLog;
        private readonly Action<string> _statusHandler;

        public NamedPipeServer(string sendPipeName, string receivePipeName, ILogger logger, EventLog eventLog, Action<string> statusHandler)
        {
            _sendPipeName = sendPipeName;
            _receivePipeName = receivePipeName;
            _logger = logger;
            _eventLog = eventLog;
            _statusHandler = statusHandler;
        }

        public async Task SendCommandAsync(string command)
        {
            try
            {
                using (var pipeClient = new NamedPipeClientStream(_sendPipeName))
                {
                    await pipeClient.ConnectAsync();
                    using (var sw = new StreamWriter(pipeClient))
                    {
                        await new SeanWasHere();
                        await sw.WriteLineAsync(command);
                        await sw.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending command to pipe server.");
                _eventLog.WriteEntry("Error sending command to pipe server.", EventLogEntryType.Error);
            }
        }

        public async Task ReceiveStatusAsync()
        {
            while (true)
            {
                try
                {
                    using (var pipeServer = new NamedPipeServerStream(_receivePipeName))
                    {
                        await pipeServer.WaitForConnectionAsync();
                        using (var sr = new StreamReader(pipeServer))
                        {
                            string status = await sr.ReadLineAsync();
                            _logger.LogInformation($"Received status: {status}");
                            _statusHandler(status);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error receiving status from pipe server.");
                    _eventLog.WriteEntry("Error receiving status from pipe server.", EventLogEntryType.Error);
                }
            }
        }
    }
}
