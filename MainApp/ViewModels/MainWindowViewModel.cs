using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;
using MainApp.Models;
using Microsoft.Extensions.DependencyInjection;

namespace MainApp.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel() 
        {
            JobPostings.PropertyChangedEx += HandleJobPostingPropertyChangedEx;
            RefreshJobPostings();

            if (!GetSettingsValid().Result)
            {
                ChangeSettings();
            }
        }

        public ObservableCollectionEx<JobPosting> JobPostings { get; } = new ObservableCollectionEx<JobPosting>();

        [ObservableProperty]
        public partial JobPosting SelectedPosting { get; set; } = new JobPosting();

        private async Task<bool> GetSettingsValid()
        {
            if (!SettingsViewModel.IsValidPath(Settings.Default.SaveLocation))
            {
                return false;
            }

            IGeocodingService geocodingService = App.Current.Services.GetService<IGeocodingService>()!;
            if (string.IsNullOrWhiteSpace(Settings.Default.CurrentLocation))
            {
                return false;
            }

            GeoCodingResponse result = await geocodingService.GetLocationCoordinatesAsync(Settings.Default.CurrentLocation);
            if(result.Result != ResponseResult.OK)
            {
                return false;
            }

            return true;
        }

        [RelayCommand]
        private void AddPosting()
        {
            IDialogService dialogService = App.Current.Services.GetService<IDialogService>()!;
            JobPosting? newJobPosting = dialogService.ShowNewJobPostingDialog();

            if(newJobPosting != null)
            {
                IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
                newJobPosting.RowID = db.AddNewJob(newJobPosting);
                JobPostings.Add(newJobPosting);
            }
        }

        [RelayCommand]
        private void EditPosting(JobPosting editPosting)
        {
            IDialogService dialogService = App.Current.Services.GetService<IDialogService>()!;
            JobPosting? editedJobPosting = dialogService.ShowEditJobPostingDialog(editPosting);
            if(editedJobPosting != null)
            {
                IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;

                editPosting.SetFrom(editedJobPosting);
                db.UpdateJobPosting(editPosting);
            }
        }

        [RelayCommand]
        private void DeletePosting(JobPosting? jobPosting)
        {
            if(jobPosting == null)
            {
                return;
            }

            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            db.RemoveJobPosting(jobPosting);
            JobPostings.Remove(jobPosting);
        }

        [RelayCommand]
        private void ChangeSettings()
        {
            IDialogService dialogService = App.Current.Services.GetService<IDialogService>()!;
            SettingsDialogResult dialogResult = dialogService.ShowSettingsDialog();

            if (dialogResult.NewSaveLocation != null)
            {
                RefreshJobPostings();
            }

            if (dialogResult.NewCurrentLocation != null)
            {
                RecalculateAllJobPostingDistances();
            }
        }

        [RelayCommand]
        private void ViewDescription(string description)
        {
            IDialogService dialogService = App.Current.Services.GetService<IDialogService>()!;
            dialogService.ShowViewDescriptionDialog(description);
        }

        private void HandleJobPostingPropertyChangedEx(object? sender, PropertyChangedEventArgs e)
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            JobPosting posting = (JobPosting)sender!;
            db.UpdateJobPosting(posting);
        }

        private void RefreshJobPostings()
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            db.RefreshJobPostings(JobPostings);
        }

        private async void RecalculateAllJobPostingDistances()
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;

            string curLocationInput = Settings.Default.CurrentLocation;
            GeoCodingResponse curLocationResponse = await gc.GetLocationCoordinatesAsync(curLocationInput);

            foreach(JobPosting posting in JobPostings)
            {
                GeoCodingResponse jobLocationResponse = await gc.GetLocationCoordinatesAsync(posting.JobLocation);
                IDistanceCalculatorService distanceCalculatorService = App.Current.Services.GetService<IDistanceCalculatorService>()!;
                float fDistance = distanceCalculatorService.GetDistanceBetween(curLocationResponse, jobLocationResponse);
                posting.JobDistance = (int)fDistance;

                db.UpdateJobPosting(posting);
            }
        }
    }
}
