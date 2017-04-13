using System;
using System.Diagnostics;
using System.Printing;
using CabineParty.Core;
using CabineParty.Core.Windows;
using CabineParty.UnitCodeApp.Properties;

namespace CabineParty.UnitCodeApp
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
                // TODO add log checker.
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
    }
}
