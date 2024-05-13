using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace VNCOverlayWorkerService
{
    public class PipeServer
    {
        private readonly string _pipeName;
        private NamedPipeServerStream _pipeServer;
        private readonly ILogger _logger;
        private readonly EventLog _eventLog;

        public PipeServer(string pipeName, ILogger logger, EventLog eventLog)
        {
            _pipeName = pipeName;
            _logger = logger;
            _eventLog = eventLog;

            _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Waiting for pipe connection...");
                    await _pipeServer.WaitForConnectionAsync(stoppingToken);

                    _logger.LogInformation("Pipe connection established.");
                    using (var sr = new StreamReader(_pipeServer))
                    using (var sw = new StreamWriter(_pipeServer))
                    {
                        string request = await sr.ReadLineAsync();
                        string response = ProcessCommand(request);
                        await sw.WriteLineAsync(response);
                        await sw.FlushAsync();
                    }
                    _pipeServer.Disconnect();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in pipe server operation.");
                    if (!_pipeServer.IsConnected)
                    {
                        _pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    }
                }
            }
        }

        private string ProcessCommand(string command)
        {
            // Here you would handle different commands
            switch (command.ToUpper())
            {
                case "SHOW":
                    return "Showing overlay";
                case "HIDE":
                    return "Hiding overlay";
                default:
                    return "Unknown command";
            }
        }       
    }
}
