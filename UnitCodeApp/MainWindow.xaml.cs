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
        public MainWindow()
        {
            InitializeComponent();
            Closed += ((MainWindowViewModel)DataContext).Closed;
        }
    }
}
