using System.ServiceProcess;

namespace VNCOverlay
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new VNCOverlay()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
