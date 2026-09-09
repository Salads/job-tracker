using MainApp.ViewModels;
using MainApp.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MainApp.Services
{
    internal class DialogService : IDialogService
    {
        public JobPosting? ShowEditJobPostingDialog(JobPosting editPosting)
        {
            NewPostingView editPostingWindow = new NewPostingView(editPosting)
            {
                Owner = App.Current.MainWindow
            };
            editPostingWindow.ShowDialog();

            return (editPostingWindow.DialogResult == true ? editPostingWindow.GetJobPosting() : null);
        }

        public JobPosting? ShowNewJobPostingDialog()
        {
            NewPostingView addnewWindow = new NewPostingView(null)
            {
                Owner = App.Current.MainWindow
            };
            addnewWindow.ShowDialog();

            return (addnewWindow.DialogResult == true ? addnewWindow.GetJobPosting() : null);
        }

        public SettingsDialogResult ShowSettingsDialog()
        {
            SettingsView settingsView = new SettingsView() { Owner = App.Current.MainWindow };
            settingsView.ShowDialog();

            return new SettingsDialogResult
            {
                NewSaveLocation = settingsView.NewDatabaseLocation,
                NewCurrentLocation = settingsView.NewCurrentLocation,
            };
        }

        public void ShowViewDescriptionDialog(string description)
        {
            DescriptionView descView = new DescriptionView(description)
            {
                Owner = App.Current.MainWindow
            };

            descView.ShowDialog();
        }

        public string? ShowEditDescriptionDialog(string description)
        {
            EditDescriptionView editDescWindow = new EditDescriptionView(description)
            {
                Owner = App.Current.MainWindow
            };
            editDescWindow.ShowDialog();

            return (editDescWindow.DialogResult == true ? editDescWindow.ViewModel.JobDescription : null);
        }

        public string? GetDatabaseSaveLocation()
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
            return (result == true ? dialog.FileName : null);
        }
    }
}
