using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;

namespace MainApp.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel() 
        {
            AddNewPostingCommand = new RelayCommand<Window>(OpenNewPostingWindow);
            databaseService.RefreshJobPostings(JobPostings);
        }

        public ObservableCollection<JobPosting> JobPostings { get; } = new ObservableCollection<JobPosting>();

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
                databaseService.AddNewJob(((NewPostingWindowViewModel)addnewWindow.DataContext).GetJobPosting());
                databaseService.RefreshJobPostings(JobPostings);
            }
        }
    }
}
