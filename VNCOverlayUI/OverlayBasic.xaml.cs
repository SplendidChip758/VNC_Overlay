using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;


namespace VNCOverlayUI
{
    /// <summary>
    /// Interaction logic for OverlayBasic.xaml
    /// </summary>
    public partial class OverlayBasic : Window
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_LAYERED = 0x80000;

        public OverlayBasic()
        {
            InitializeComponent();
            this.Loaded += OverlayBasic_Loaded;
        }

        private void OverlayBasic_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetClickThrough(hwnd);
        }

        private void SetClickThrough(IntPtr hwnd)
        {
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_LAYERED);
        }
    }
}
