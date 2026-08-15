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

        private DatabaseService databaseService = new DatabaseService();

        private void HandleJobPostingPropertyChangedEx(object? sender, PropertyChangedEventArgs e)
        {
            JobPosting posting = (JobPosting)sender!;
            databaseService.UpdateJobPosting(posting);
        }

        public void AddNewJob(JobPosting posting)
        {
            posting.RowID = databaseService.AddNewJob(posting);
            JobPostings.Add(posting);
        }

        public void RefreshJobPostings()
        {
            databaseService.EnsureTableExists();
            databaseService.RefreshJobPostings(JobPostings);
        }
    }
}
