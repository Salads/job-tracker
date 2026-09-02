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

        public void RecalculateAllJobPostingDistances()
        {
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;

            string curLocationInput = Settings.Default.CurrentLocation;
            IGeocodingService.GeoCodingResponse curLocationResponse = gc.GetLocationCoordinates(curLocationInput);
            if(curLocationResponse.Result != IGeocodingService.ResponseResult.OK)
            {
                return; // TODO(Salads): User Feedback on error
            }

            foreach(JobPosting posting in JobPostings)
            {
                IGeocodingService.GeoCodingResponse jobLocationResponse = gc.GetLocationCoordinates(posting.JobLocation);
                if(jobLocationResponse.Result != IGeocodingService.ResponseResult.OK)
                {
                    continue;
                }

                if (curLocationResponse.Result == IGeocodingService.ResponseResult.OK && jobLocationResponse.Result == IGeocodingService.ResponseResult.OK)
                {
                    IDistanceCalculatorService distanceCalculatorService = App.Current.Services.GetService<IDistanceCalculatorService>()!;
                    float fDistance = distanceCalculatorService.GetDistanceBetween(curLocationResponse.Coords, jobLocationResponse.Coords);
                    posting.JobDistance = (int)fDistance;
                }
                else
                {
                    posting.JobDistance = -1;
                }

                db.UpdateJobPosting(posting);
            }
        }
    }
}
