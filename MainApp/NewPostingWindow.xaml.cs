using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Diagnostics;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for NewPostingWindow.xaml
    /// </summary>
    public partial class NewPostingWindow : Window
    {
        JobPosting m_jobPosting = new JobPosting();

        public NewPostingWindow()
        {
            InitializeComponent();

            DataContext = m_jobPosting;

            jobTypeCombo.ItemsSource = Enum.GetValues<JobType>();
            jobArrangementCombo.ItemsSource = Enum.GetValues<JobArrangement>();
            jobStatusCombo.ItemsSource = Enum.GetValues<JobStatus>();
        }

        private void descButton_Click(object sender, RoutedEventArgs e)
        {
            EditDescriptionWindow editDescWindow = new EditDescriptionWindow(m_jobPosting.JobDescription)
            {
                Owner = this
            };
            editDescWindow.ShowDialog();

            int newCharCount = 0;
            if(editDescWindow.DialogResult == true)
            {
                newCharCount = editDescWindow.descTextBox.Text.Length;
                m_jobPosting.JobDescription = editDescWindow.descTextBox.Text;
            }

            descTextBlock.Text = $"({newCharCount} chars)";
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine(m_jobPosting.ToString());
        }
    }
}
