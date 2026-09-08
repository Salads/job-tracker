using MainApp.ViewModels;
using MainApp.Views;
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

            return new SettingsDialogResult()
            {
                NewSaveLocation = (settingsView.DatabaseChanged ? settingsView.ViewModel.SaveLocation : null),
                NewCurrentLocation = (settingsView.CurrentLocationChanged ? settingsView.ViewModel.LocationFullName : null),
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
    }
}
