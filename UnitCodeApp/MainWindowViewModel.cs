using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
        private const string DefaultValidCodeFilePath = "ValidCode.txt";
        private const string DefaultUsedCodeFilePath = "UsedCode.txt";
        private readonly Color _defaultBackGroundColor = Colors.Beige;
        private WindowsManager _mainWindows;
        private Process _photoBooth;
        private string _inputCode;
        private Color _codeColor;

        public MainWindowViewModel()
        {
            CodeColor = ValidCodeColor;
            AddDigit = new RelayCommand(_ => InputCode += _, _ => string.IsNullOrEmpty(InputCode) || InputCode.Length < CodeLength);
            Cancel = new RelayCommand(_ =>
            {
                InputCode = string.Empty;
            }, _ => !string.IsNullOrEmpty(InputCode));
            Correct = new RelayCommand(_ => InputCode = InputCode.Substring(0, InputCode.Length - 1), _ => !string.IsNullOrEmpty(InputCode));
            Validate = new RelayCommand(_ => ValidateCode(), _ => !string.IsNullOrEmpty(InputCode) && InputCode.Length == CodeLength);

            StartPhotobooth();
            SetupPrinter();
        }

        public Color ValidCodeColor => Colors.White;
        public Color WrongCodeColor => Colors.Red;

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
            set
            {
                CodeColor = ValidCodeColor;
                Set(() => InputCode, ref _inputCode, value);
            }
        }

        public Color CodeColor
        {
            get { return _codeColor; }
            set { Set(() => CodeColor, ref _codeColor, value); }
        }

        public ICommand AddDigit { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Correct { get; set; }
        public ICommand Validate { get; set; }

        private void ValidateCode()
        {
            if (!IsValidCode())
            {
                CodeColor = WrongCodeColor;
                return;
            }

            MarkCodeAsUsed();
            InputCode = string.Empty;
            _mainWindows.Show();
        }

        private void MarkCodeAsUsed()
        {
            AddUsedCode();
            RemoveCode();
        }

        private void AddUsedCode()
        {
            File.AppendAllLines(GetUsedCodeFilePath(), new List<string> { GetUsedValue() });
        }

        private string GetUsedValue()
        {
            return $"Code:{InputCode} used at {DateTime.Now:F}";
        }

        private bool IsValidCode()
        {
            return File.ReadAllLines(GetValidCodeFilePath()).Any(_ => _ == InputCode);
        }

        private static string GetValidCodeFilePath()
        {
            return GetFilePath(Settings.Default.ValidCodeFilePath ?? DefaultValidCodeFilePath);
        }

        private static string GetUsedCodeFilePath()
        {
            return GetFilePath(Settings.Default.UsedCodeFilePath ?? DefaultUsedCodeFilePath);
        }

        private static string GetFilePath(string filePath)
        {
            if (File.Exists(filePath))
                return filePath;

            var file = File.Create(filePath);
            file.Close();

            return filePath;
        }

        private void RemoveCode()
        {
            var tempFile = Path.GetTempFileName();
            var validCodeFile = GetValidCodeFilePath();
            using (var sr = new StreamReader(validCodeFile))
            using (var sw = new StreamWriter(tempFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line != InputCode)
                        sw.WriteLine(line);
                }
            }

            File.Delete(validCodeFile);
            File.Move(tempFile, validCodeFile);
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
