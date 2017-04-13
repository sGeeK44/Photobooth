using System;
using System.Windows.Media;
using CabineParty.Core.Mvvm;
using CabineParty.UnitCodeApp.Properties;

namespace CabineParty.UnitCodeApp
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly Color _defaultBackGroundColor = Colors.Beige;
        private string _inputCode;

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
    }
}
