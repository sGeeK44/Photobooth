using System;
using System.Diagnostics;
using System.Printing;
using System.Windows.Input;
using System.Windows.Media;
using CabineParty.Core;
using CabineParty.Core.Mvvm;
using CabineParty.Core.Windows;
using CabineParty.UnitCodeApp.Properties;

namespace CabineParty.UnitCodeApp
{
    public class MainWindowViewModel : ViewModel
    {
        private const int DefaultCodeLength = 8;
        private readonly Color _defaultBackGroundColor = Colors.Beige;
        private WindowsManager _mainWindows;
        private Process _photoBooth;
        private string _inputCode;

        public MainWindowViewModel()
        {
            AddDigit = new RelayCommand(_ => InputCode += _, _ => string.IsNullOrEmpty(InputCode) || InputCode.Length < CodeLength);
            Cancel = new RelayCommand(_ => InputCode = string.Empty, _ => !string.IsNullOrEmpty(InputCode));
            Correct = new RelayCommand(_ => InputCode = InputCode.Substring(0, InputCode.Length - 1), _ => !string.IsNullOrEmpty(InputCode));
            Validate = new RelayCommand(_ => ValidateCode(), _ => !string.IsNullOrEmpty(InputCode) && InputCode.Length == CodeLength);

            StartPhotobooth();
            SetupPrinter();
        }

        public int CodeLength => GetCodeLength();

        public Brush BackgroundColor
        {
            get
            {
                try
                {
                    var color = ColorConverter.ConvertFromString(Settings.Default.BackgroundColor)
                             ?? _defaultBackGroundColor;

                    return new SolidColorBrush((Color)color);

                }
                catch (Exception)
                {
                    return new SolidColorBrush(_defaultBackGroundColor);
                }
            }
        }

        public string InputCode
        {
            get { return _inputCode; }
            set { Set(() => InputCode, ref _inputCode, value); }
        }

        public ICommand AddDigit { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Correct { get; set; }
        public ICommand Validate { get; set; }

        private void ValidateCode()
        {
            InputCode = string.Empty;
            _mainWindows.Show();
        }

        private static int GetCodeLength()
        {
            int result;
            return int.TryParse(Settings.Default.CodeLength, out result)
                 ? result
                 : DefaultCodeLength;
        }

        private void ExitPhotobooth()
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
                _mainWindows.Hide();
            };
        }

        private void StartPhotobooth()
        {
            ProcessHelper.KillAll(Settings.Default.PhotoBoothProcessName);
            _photoBooth = ProcessHelper.Start(Settings.Default.PhotoBoothExecPath);
            _mainWindows = new WindowsManager(_photoBooth);
            _mainWindows.Hide();
        }

        public void Closed(object sender, EventArgs e)
        {
            ExitPhotobooth();
        }
    }
}
