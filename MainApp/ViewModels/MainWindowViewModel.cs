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
        }

        public ObservableCollectionEx<JobPosting> JobPostings { get; } = new ObservableCollectionEx<JobPosting>();

        [ObservableProperty]
        public partial JobPosting SelectedPosting { get; set; } = new JobPosting();

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

        public void UpdatePosting(JobPosting postingToUpdate, JobPosting sourcePosting)
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;

            postingToUpdate.SetFrom(sourcePosting);
            db.UpdateJobPosting(postingToUpdate);
        }

        private void HandleJobPostingPropertyChangedEx(object? sender, PropertyChangedEventArgs e)
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            JobPosting posting = (JobPosting)sender!;
            db.UpdateJobPosting(posting);
        }

        public void AddNewJob(JobPosting posting)
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            posting.RowID = db.AddNewJob(posting);
            JobPostings.Add(posting);
        }

        public void RefreshJobPostings()
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            db.RefreshJobPostings(JobPostings);
        }

        public async void RecalculateAllJobPostingDistances()
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
