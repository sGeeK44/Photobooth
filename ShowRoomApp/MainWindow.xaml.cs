using System;
using System.Diagnostics;
using System.Printing;
using System.Threading;
using System.Windows;
using CabineParty.Core;
using CabineParty.Core.Windows;
using CabineParty.ShowRoomApp.Properties;

namespace CabineParty.ShowRoomApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private WindowsManager _mainWindows;
        private Process _photoBooth;

        public MainWindow()
        {
            InitializeComponent();
            StartPhotobooth();
            Closed += ExitPhotobooth;
            SetupVideo();
            SetupPrinter();
        }

        private void ExitPhotobooth(object sender, EventArgs args)
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
        }

        private void SetupPrinter()
        {
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
                _mainWindows.Hide();
            };
        }

        private void SetupVideo()
        {
            VideoElement.Source = new Uri(Settings.Default.VideoSource);
            VideoElement.MediaEnded += (sender, args) =>
            {
                VideoElement.Stop();
                PlayButton.Visibility = Visibility.Visible;
                _mainWindows.Show();
            };
        }

        private void StartPhotobooth()
        {
            ProcessHelper.KillAll(Settings.Default.PhotoBoothProcessName);
            _photoBooth = ProcessHelper.Start(Settings.Default.PhotoBoothExecPath);
            _mainWindows = new WindowsManager(_photoBooth);
            _mainWindows.Hide();
        }

        private void Play_OnClick(object sender, RoutedEventArgs e)
        {
            VideoElement.Play();
            PlayButton.Visibility = Visibility.Collapsed;
        }
    }
}
