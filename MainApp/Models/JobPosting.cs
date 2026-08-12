using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace MainApp
{
    public partial class JobPosting
    {
        public JobPosting() 
        {
            JobTitle = string.Empty;
            JobCompanyName = string.Empty;
            // JobPostingURL;
            JobType = JobType.FullTime;
            JobArrangement = JobArrangement.OnSite;
            JobLocation = string.Empty;
            JobDistance = 0.0f;
            JobDescription = string.Empty;
            JobStatus = JobStatus.Applied;
        }

        public string JobTitle {  get; set; }

        public string JobCompanyName { get; set; }

        public Uri? JobPostingURL { get; set; }

        public JobType JobType { get; set; }

        public JobArrangement JobArrangement { get; set; }

        public string JobLocation { get; set; }

        public float JobDistance { get; set; }

        public string JobDescription { get; set; }

        public JobStatus JobStatus { get; set; }

        public override string ToString()
        {
            return $"Job Title: {JobTitle}\n" +
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
