using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace MainApp.Services
{
    internal class SettingsDialogResult
    {
        public string? NewSaveLocation;

        public string? NewCurrentLocation;
    }

    internal interface IDialogService
    {
        JobPosting? ShowNewJobPostingDialog();

        JobPosting? ShowEditJobPostingDialog(JobPosting editPosting);

        SettingsDialogResult ShowSettingsDialog();

        string? GetDatabaseSaveLocation();

        void ShowViewDescriptionDialog(string description);

        string? ShowEditDescriptionDialog(string description);
    }
}
