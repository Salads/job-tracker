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
    public partial class SettingsView : Window
    {
        public SettingsView()
        {
            InitializeComponent();
        }

        private void browseButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = Settings.Default.SaveLocation;
            dialog.DefaultExt = ".db";
            dialog.Filter = "SQLite Database (.db)|*.db";
            dialog.OverwritePrompt = false;
            dialog.Title = "Choose / Create Save File";

            bool? result = dialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                ((SettingsViewModel)DataContext).SaveLocation = dialog.FileName;
            }
        }

        private void saveLocationResetButton_Click(object sender, RoutedEventArgs e)
        {
            ((SettingsViewModel)DataContext).SaveLocation = (string)Settings.Default.Properties["SaveLocation"].DefaultValue;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.SaveLocation = ((SettingsViewModel)DataContext).SaveLocation;
            Settings.Default.Save();
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
