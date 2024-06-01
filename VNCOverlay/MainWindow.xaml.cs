using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace VNCOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetClickThrough(hwnd);
        }

        private void SetClickThrough(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }

        public void HideOverlays()
        {
            OverlayMain.Visibility = Visibility.Collapsed;
            OverlayBasic.Visibility = Visibility.Collapsed;
            OverlayCenter.Visibility = Visibility.Collapsed;
            OverlayLoud.Visibility = Visibility.Collapsed;
        }
        public void ShowOverlayBasic()
        {
            OverlayBasic.Visibility = Visibility.Visible;
            OverlayMain.Visibility = Visibility.Collapsed;
            OverlayCenter.Visibility = Visibility.Collapsed;
            OverlayLoud.Visibility = Visibility.Collapsed;
        }
        public void ShowOverlayCenter()
        {
            OverlayCenter.Visibility = Visibility.Visible;
            OverlayMain.Visibility = Visibility.Collapsed;
            OverlayBasic.Visibility = Visibility.Collapsed;
            OverlayLoud.Visibility = Visibility.Collapsed;
        }
        public void ShowOverlayLoud()
        {
            OverlayLoud.Visibility = Visibility.Visible;
            OverlayMain.Visibility = Visibility.Collapsed;
            OverlayBasic.Visibility = Visibility.Collapsed;
            OverlayCenter.Visibility = Visibility.Collapsed;
        }
    }
}