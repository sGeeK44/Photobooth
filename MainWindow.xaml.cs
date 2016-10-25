using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using PhotoBooth.Properties;

namespace PhotoBooth
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int Hide = 0;
        private const int Minimized = 6;
        private const int Maximized = 3;
        private const int Show = 5;

        private readonly IntPtr _mainWindowsHandle;
        private readonly Process _photoBooth;


        public MainWindow()
        {
            InitializeComponent();
            VideoElement.Source = new Uri(Settings.Default.VideoSource);
            VideoElement.MediaEnded += (sender, args) => {
                VideoElement.Stop();
                ShowWindow(_mainWindowsHandle, Maximized);
                Thread.Sleep(10000);
                ShowWindow(_mainWindowsHandle, Hide);
            };
            _photoBooth = Process.Start(Settings.Default.PhotoBoothExecPath);
            Thread.Sleep(5000);
            _mainWindowsHandle = _photoBooth.MainWindowHandle;
            ShowWindow(_mainWindowsHandle, Hide);
            Closed += (sender, args) => _photoBooth.Kill();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            VideoElement.Play();
        }
    }
}
