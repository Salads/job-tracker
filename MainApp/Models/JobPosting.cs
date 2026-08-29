using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MainApp
{
    public partial class JobPosting : ObservableObject
    {
        public JobPosting() 
        {
            RowID = -1;
            JobTitle = string.Empty;
            JobCompanyName = string.Empty;
            JobPostingURL = string.Empty;
            JobType = JobType.FullTime;
            JobArrangement = JobArrangement.OnSite;
            JobLocation = string.Empty;
            JobDistance = 0;
            JobDescription = string.Empty;
            JobStatus = JobStatus.Applied;
        }

        [ObservableProperty]
        public partial Int64 RowID { get; set; }

        [ObservableProperty]
        public partial string JobTitle {  get; set; }
        
        [ObservableProperty]
        public partial string JobCompanyName { get; set; }

        [ObservableProperty]
        public partial string JobPostingURL { get; set; }

        [ObservableProperty]
        public partial JobType JobType { get; set; }

        [ObservableProperty]
        public partial JobArrangement JobArrangement { get; set; }

        [ObservableProperty]
        public partial string JobLocation { get; set; }

        [ObservableProperty]
        public partial int JobDistance { get; set; }

        [ObservableProperty]
        public partial string JobDescription { get; set; }

        [ObservableProperty]
        public partial JobStatus JobStatus { get; set; }

        public override string ToString()
        {
            return $"Row ID: {RowID}\n" +
                   $"Job Title: {JobTitle}\n" +
                   $"Job Company Name: {JobCompanyName}\n" +
                   $"Job Posting URL: {JobPostingURL}\n" +
                   $"Job Type: {JobType}\n" +
                   $"Job Arrangement: {JobArrangement}\n" +
                   $"Job Location: {JobLocation}\n" +
                   $"Job Distance: {JobDistance}\n" +
                   $"Job Description: {JobDescription}\n" +
                   $"Job Status: {JobStatus}";
        }
    }
}
