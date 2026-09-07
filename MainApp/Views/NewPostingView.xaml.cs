using CommunityToolkit.Mvvm.Input;
using MainApp.Models;
using MainApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for NewPostingWindow.xaml
    /// </summary>
    public partial class NewPostingView : Window
    {

        public NewPostingView(JobPosting? posting)
        {
            InitializeComponent();

            ViewModel.RequestClose += OnRequestClose;

            Loaded += (_, _) => 
            {
                MaxHeight = MinHeight = ActualHeight;
                locationControl.InitializeAndVerify(posting != null ? posting.JobLocation : string.Empty);
            };

            if (posting != null)
            {
                ViewModel.SetEditPosting(posting);
                Title = "Edit Job Posting";
                descTextBlock.Text = $"({ViewModel.JobPosting.JobDescription.Length} chars)";
            }

            locationControl.ViewModel.RemoteAllowed = true;

            UpdateLayout();
        }

        public bool EditMode { get; set; }

        public JobPosting GetJobPosting()
        {
            return ViewModel.JobPosting;
        }

        private void OnRequestClose(bool saved, bool editMode)
        {
            DialogResult = saved;
            EditMode = editMode;
            Close();
        }

        private void descButton_Click(object sender, RoutedEventArgs e)
        {
            EditDescriptionView editDescWindow = new EditDescriptionView()
            {
                Owner = this
            };

            editDescWindow.ViewModel.JobDescription = ViewModel.JobPosting.JobDescription;

            editDescWindow.ShowDialog();

            int newCharCount = 0;
            if (editDescWindow.DialogResult == true)
            {
                newCharCount = editDescWindow.ViewModel.JobDescription.Length;
                ViewModel.JobPosting.JobDescription = editDescWindow.ViewModel.JobDescription;
            }

            descTextBlock.Text = $"({newCharCount} chars)";
        }
    }
}
