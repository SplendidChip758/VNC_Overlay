using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows;
using Application = System.Windows.Application;

namespace VNCOverlay
{
    public partial class App : Application
    {
        private IHost _host { get; }
        private NotifyIcon _notifyIcon;
        private EventLogHelper _eventLogHelper;
        private OverlayHelper _overlayHelper;
        private PortsHelper _portsHelper;

        private string _overlay;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                        .ConfigureAppConfiguration((context, config) =>
                        {
                            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        })
                        .ConfigureServices((context, services) =>
                        {
                            services.AddSingleton<EventLogHelper>();
                            services.AddSingleton<PortsHelper>();
                            services.AddSingleton<OverlayHelper>();
                            services.AddHostedService<Worker>();
                            services.AddLogging(configure => configure.AddConsole());
                        })
                        .Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            await _host.StartAsync();
            InitializeTrayIcon();

            _eventLogHelper = _host.Services.GetRequiredService<EventLogHelper>();
            _portsHelper = _host.Services.GetRequiredService<PortsHelper>();
            _overlayHelper = _host.Services.GetRequiredService<OverlayHelper>();

            _eventLogHelper.EventLog.WriteEntry("Starting VNCOverlay service", EventLogEntryType.Information);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            _eventLogHelper.EventLog.WriteEntry("Stopping VNCOverlay service", EventLogEntryType.Information);
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
            var settingsWindow = new SettingsWindow(_portsHelper, _overlayHelper);
            settingsWindow.Show();
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            Shutdown();
        }
    }

}
