using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MainApp.ViewModels
{
    public class NewPostingWindowViewModel : ObservableValidator
    {
        public NewPostingWindowViewModel()
        {
            EditDescription = new RelayCommand<Window>(OpenDescriptionEditor);
            JobDescriptionLabel = "(0 chars)";
        }

        private string jobTitle;
        private string companyName;
        private Uri postingURL;
        private JobType jobType;
        private JobArrangement jobArrangement;
        private string location;
        private string jobDescription;
        private string jobDescriptionLabel;
        private JobStatus jobStatus;

        #region New Job Properties
        [Required]
        public string JobTitle
        {
            get => jobTitle;
            set => SetProperty(ref this.jobTitle, value, true);
        }

        [Required]
        public string CompanyName
        {
            get => companyName;
            set => SetProperty(ref this.companyName, value, true);
        }

        [Required]
        public Uri PostingURL
        {
            get => postingURL;
            set => SetProperty(ref this.postingURL, value, true);
        }

        [Required]
        public JobType JobType
        {
            get => jobType;
            set => SetProperty(ref this.jobType, value, true);
        }

        [Required]
        public JobArrangement JobArrangement
        {
            get => jobArrangement;
            set => SetProperty(ref this.jobArrangement, value, true);
        }

        [Required]
        public string Location
        {
            get => location;
            set => SetProperty(ref this.location, value, true);
        }

        [Required]
        public string JobDescription
        {
            get => jobDescription;
            set => SetProperty(ref this.jobDescription, value, true);
        }
        public string JobDescriptionLabel
        {
            get => jobDescriptionLabel;
            set => SetProperty(ref this.jobDescriptionLabel, value, true);
        }

        [Required]
        public JobStatus JobStatus
        {
            get => jobStatus;
            set => SetProperty(ref this.jobStatus, value, true);
        }
        #endregion

        public ICommand EditDescription { get; }

        private void OpenDescriptionEditor(Window? owner)
        {
            EditDescriptionWindow editDescWindow = new EditDescriptionWindow(JobDescription)
            {
                Owner = owner
            };
            editDescWindow.ShowDialog();

            int newCharCount = 0;
            if (editDescWindow.DialogResult == true)
            {
                newCharCount = editDescWindow.descTextBox.Text.Length;
                JobDescription = editDescWindow.descTextBox.Text;
            }

            JobDescriptionLabel = $"({newCharCount} chars)";
        }
    }
}
