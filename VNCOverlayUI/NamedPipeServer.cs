using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VNCOverlayUI
{
    public class NamedPipeServer
    {
        private readonly string _sendPipeName;
        private readonly string _receivePipeName;
        private readonly Action<string> _commandHandler;

        public NamedPipeServer(string sendPipeName, string receivePipeName, Action<string> commandHandler)
        {
            _receivePipeName = receivePipeName;
            _sendPipeName = sendPipeName;        
            _commandHandler = commandHandler;
        }

        public async Task ReceiveCommandAsync()
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
                        string command = await sr.ReadLineAsync();
                        _commandHandler(command);
                    }
                }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving command from pipe client: {ex.Message}");
                }
            }
        }   

        public async Task SendStatusAsync(string status)
        {
            try
            {
                using (var pipeClient = new NamedPipeClientStream(_sendPipeName))
                {
                    await pipeClient.ConnectAsync();
                    using (var sw = new StreamWriter(pipeClient))
                    {
                        await sw.WriteLineAsync(status);
                        await sw.FlushAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending status to pipe server: {ex.Message}");
            }
        }
    }
}
