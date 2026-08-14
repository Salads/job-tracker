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

        public NewPostingView()
        {
            InitializeComponent();

            jobTypeCombo.ItemsSource = Enum.GetValues<JobType>();
            jobArrangementCombo.ItemsSource = Enum.GetValues<JobArrangement>();
            jobStatusCombo.ItemsSource = Enum.GetValues<JobStatus>();
        }

        private void descButton_Click(object sender, RoutedEventArgs e)
        {
            NewPostingWindowViewModel thisVM = (NewPostingWindowViewModel)DataContext;
            EditDescriptionView editDescWindow = new EditDescriptionView()
            {
                Owner = this
            };

            EditDescriptionViewModel descVM = (EditDescriptionViewModel)editDescWindow.DataContext;
            descVM.JobDescription = thisVM.JobDescription;

            editDescWindow.ShowDialog();

            int newCharCount = 0;
            if (editDescWindow.DialogResult == true)
            {
                newCharCount = descVM.JobDescription.Length;
                thisVM.JobDescription = descVM.JobDescription;
            }

            descTextBlock.Text = $"({newCharCount} chars)";
        }
    }
}
