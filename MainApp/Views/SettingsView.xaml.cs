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

            SettingsViewModel vm = (SettingsViewModel)DataContext;
            vm.RequestClose += OnRequestClose;

            OriginalDatabaseLocation = vm.SaveLocation;
            OriginalCurrentLocation = vm.LocationInput;

            locationControl.InitializeAndVerify(Settings.Default.CurrentLocation);

            UpdateLayout();
        }

        private string OriginalDatabaseLocation { get; set; }

        private string OriginalCurrentLocation { get; set; }

        public bool DatabaseChanged { get;  private set; }

        public bool CurrentLocationChanged { get; private set; }

        private void OnRequestClose(object? sender, EventArgs e)
        {
            SettingsViewModel vm = (SettingsViewModel)DataContext;
            if (OriginalDatabaseLocation != vm.SaveLocation)
            {
                DatabaseChanged = true;
            }

            if (OriginalCurrentLocation != vm.LocationInput)
            {
                CurrentLocationChanged = true;
            }

            Close();
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                FileName = Settings.Default.SaveLocation,
                DefaultExt = ".db",
                Filter = "SQLite Database (.db)|*.db",
                OverwritePrompt = false,
                Title = "Choose / Create Save File"
            };

            bool? result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                SettingsViewModel vm = (SettingsViewModel)DataContext;

                if(vm.SaveLocation != dialog.FileName)
                {
                    DatabaseChanged = true;
                }

                vm.SaveLocation = dialog.FileName;
            }
        }

        private void saveLocationResetButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsViewModel vm = (SettingsViewModel)DataContext;
            vm.SaveLocation = (string)Settings.Default.Properties["SaveLocation"].DefaultValue;
        }

        public void Dispose()
        {
            SettingsViewModel vm = (SettingsViewModel)DataContext;
            vm.RequestClose -= OnRequestClose;
        }
    }
}
