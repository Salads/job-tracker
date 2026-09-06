using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Linq;

namespace MainApp
{
    public partial class JobPosting : ObservableValidator
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

        public JobPosting GetClone()
        {
            JobPosting clone = new JobPosting();
            clone.SetFrom(this);
            return clone;
        }

        public void SetFrom(JobPosting jobPosting)
        {
            RowID          = jobPosting.RowID;
            JobTitle       = jobPosting.JobTitle;
            JobCompanyName = jobPosting.JobCompanyName;
            JobPostingURL  = jobPosting.JobPostingURL;
            JobType        = jobPosting.JobType;
            JobArrangement = jobPosting.JobArrangement;
            JobLocation    = jobPosting.JobLocation;
            JobDistance    = jobPosting.JobDistance;
            JobDescription = jobPosting.JobDescription;
            JobStatus      = jobPosting.JobStatus;

            ValidateAllProperties();
        }

        public void ValidateAllPropertiesManually()
        {
            ValidateAllProperties();
        }

        [ObservableProperty]
        public partial Int64 RowID { get; set; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string JobTitle {  get; set; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string JobCompanyName { get; set; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [CustomValidation(typeof(JobPosting), nameof(ValidatePostingURL))]
        public partial string JobPostingURL { get; set; }

        [ObservableProperty]
        public partial JobType JobType { get; set; }

        [ObservableProperty]
        public partial JobArrangement JobArrangement { get; set; }

        [ObservableProperty]
        public partial string JobLocation { get; set; }

        [ObservableProperty]
        public partial int JobDistance { get; set; }

        [Required]
        [NotifyDataErrorInfo]
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

        public static ValidationResult ValidatePostingURL(string postingURL, ValidationContext context)
        {
            Uri uri;
            bool result = Uri.TryCreate(postingURL, UriKind.Absolute, out uri);

            if (!result)
            {
                // TODO(Salads): Try to deconstruct it to see if we can correct it for the user.
                //               Ideally, shouldn't happen since URLs are usually copy-pasted.
            }

            if (result && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
            {
                return ValidationResult.Success!;
            }
            else
            {
                return new("Not a valid HTTP/S URL!");
            }
        }
    }
}
