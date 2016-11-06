using System;
using System.Diagnostics;
using System.Printing;
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
            VideoElement.MediaEnded += (sender, args) =>
            {
                VideoElement.Stop();
                PlayButton.Visibility = Visibility.Visible;
                ShowWindow(_mainWindowsHandle, Maximized);
            };
            _photoBooth = Process.Start(Settings.Default.PhotoBoothExecPath);
            Thread.Sleep(5000);
            _mainWindowsHandle = _photoBooth.MainWindowHandle;
            ShowWindow(_mainWindowsHandle, Hide);
            Closed += (sender, args) =>
            {
                try
                {
                    if (!_photoBooth.HasExited)
                        _photoBooth.Kill();
                }
                catch (Exception)
                {
                    // ignored
                }
            };

            var printQueueMonitor = new PrintQueueMonitor(new LocalPrintServer().DefaultPrintQueue.FullName);
            printQueueMonitor.Start();
            printQueueMonitor.OnJobAdded += () =>
            {
                int timeToWait;
                try
                {
                    timeToWait = int.Parse(Settings.Default.WaitingTime);
                }
                catch (Exception)
                {
                    timeToWait = 10000; // 10 Sec by default
                }

                Thread.Sleep(timeToWait);
                ShowWindow(_mainWindowsHandle, Hide);
            };
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            VideoElement.Play();
            PlayButton.Visibility = Visibility.Collapsed;
        }
    }
}
