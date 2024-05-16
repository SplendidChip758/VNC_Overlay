using System.Windows;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using Application = System.Windows.Application;

namespace VNCOverlay
{
    public partial class App : Application
    {
        private IHost _host;
        private EventLog _eventLog;
        private NotifyIcon _notifyIcon;
        private MainWindow _mainWindow;
       
        private string _overlay;

        public App()
        {
            _eventLog = EventLogHelper.InitializeEventLog();
            _overlay = Utilities.InitializeOverlay();

            _host = Host.CreateDefaultBuilder()
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        })
                        .ConfigureServices((context, services) =>
                        {  
                            services.AddSingleton(_eventLog);
                            services.AddSingleton<PortsHelper>();
                            services.AddHostedService<Worker>();
                            services.AddLogging(configure => configure.AddConsole());
                        })
                        .Build();

            // Initialize the tray icon
            InitializeTrayIcon();
            
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            _eventLog.WriteEntry("Starting VNCOverlay service", EventLogEntryType.Information);
            base.OnStartup(e);           
            await _host.StartAsync();

            var portsHelper = _host.Services.GetRequiredService<PortsHelper>();
            portsHelper.LoadPorts();
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _eventLog.WriteEntry("Stopping VNCOverlay service", EventLogEntryType.Information);
            await _host.StopAsync();
            _host.Dispose();
            _notifyIcon.Dispose();
            base.OnExit(e);
        }

        private void InitializeTrayIcon()
        {
            _notifyIcon = new NotifyIcon();
            _notifyIcon.Icon = new Icon("VNC_Overlay_Logo.ico"); // Ensure you have an icon file in your project
            _notifyIcon.Visible = true;

            var contextMenu = new ContextMenuStrip();
            var settingsMenuItem = new ToolStripMenuItem("Settings", null, OnSettingsClicked);
            var exitMenuItem = new ToolStripMenuItem("Exit", null, OnExitClicked);

            contextMenu.Items.Add(settingsMenuItem);
            contextMenu.Items.Add(exitMenuItem);

            _notifyIcon.ContextMenuStrip = contextMenu;
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            Shutdown();
        }
    }

}
