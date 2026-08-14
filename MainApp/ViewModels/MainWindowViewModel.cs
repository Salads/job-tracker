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
            AddNewPostingCommand = new RelayCommand<Window>(OpenNewPostingWindow);

            JobPostings.PropertyChangedEx += HandleJobPostingPropertyChangedEx;

            databaseService.RefreshJobPostings(JobPostings);
        }

        public ObservableCollectionEx<JobPosting> JobPostings { get; } = new ObservableCollectionEx<JobPosting>();

        [ObservableProperty]
        public partial JobPosting SelectedPosting { get; set; } = new JobPosting();

        private DatabaseService databaseService = new DatabaseService();

        public ICommand AddNewPostingCommand { get; }

        private void OpenNewPostingWindow(Window? owner)
        {
            NewPostingWindow addnewWindow = new NewPostingWindow()
            {
                Owner = owner
            };
            addnewWindow.ShowDialog();

            if(addnewWindow.DialogResult == true)
            {
                JobPosting newPosting = ((NewPostingWindowViewModel)addnewWindow.DataContext).GetJobPosting();
                databaseService.AddNewJob(newPosting);
                JobPostings.Add(newPosting);
            }
        }

        private void HandleJobPostingPropertyChangedEx(object? sender, PropertyChangedEventArgs e)
        {
            JobPosting posting = (JobPosting)sender!;
            databaseService.UpdateJobPosting(posting);
        }
    }
}
