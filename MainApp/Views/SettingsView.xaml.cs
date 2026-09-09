using MainApp.Services;
using MainApp.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for SetupWindow.xaml
    /// </summary>
    public partial class SettingsView : Window, IDisposable
    {
        internal SettingsView()
        {
            InitializeComponent();

            ViewModel.RequestClose += OnRequestClose;

            locationControl.InitializeAndVerify(Settings.Default.CurrentLocation);

            UpdateLayout();
        }

        public string? NewDatabaseLocation { get; set; }

        public string? NewCurrentLocation { get; set; }

        private void OnRequestClose(object? sender, EventArgs e)
        {
            NewDatabaseLocation = ViewModel.SaveLocation;
            NewCurrentLocation = ViewModel.LocationInput;

            Close();
        }

        public void Dispose()
        {
            SettingsViewModel vm = (SettingsViewModel)DataContext;
            vm.RequestClose -= OnRequestClose;
        }
    }
}
