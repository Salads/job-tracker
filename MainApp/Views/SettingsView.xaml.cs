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
        public SettingsView()
        {
            InitializeComponent();

            ViewModel.RequestClose += OnRequestClose;

            OriginalDatabaseLocation = ViewModel.SaveLocation;
            OriginalCurrentLocation = ViewModel.LocationInput;

            locationControl.InitializeAndVerify(Settings.Default.CurrentLocation);

            UpdateLayout();
        }

        private string OriginalDatabaseLocation { get; set; }

        private string OriginalCurrentLocation { get; set; }

        public string? NewDatabaseLocation { get; set; }

        public string? NewCurrentLocation { get; set; }

        private void OnRequestClose(object? sender, EventArgs e)
        {
            NewDatabaseLocation = (OriginalDatabaseLocation != ViewModel.SaveLocation ? ViewModel.SaveLocation : null);

            NewDatabaseLocation = (OriginalCurrentLocation != ViewModel.LocationInput ? ViewModel.LocationInput : null);

            Close();
        }

        public void Dispose()
        {
            SettingsViewModel vm = (SettingsViewModel)DataContext;
            vm.RequestClose -= OnRequestClose;
        }
    }
}
