using MainApp.Models;
using MainApp.Services;
using MainApp.ViewModels;
using MainApp.Views;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }

        private void URL_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink link = (Hyperlink)e.OriginalSource;
            Process.Start(new ProcessStartInfo(link.NavigateUri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void descButton_Click(object sender, RoutedEventArgs e)
        {
            Button senderButton = (Button)sender;
            JobPosting jobPosting = (JobPosting)senderButton.DataContext;
            EditDescriptionView descView = new EditDescriptionView()
            {
                Owner = this
            };

            EditDescriptionViewModel editDescriptionViewModel = (descView.DataContext as EditDescriptionViewModel)!;
            editDescriptionViewModel.JobDescription = jobPosting.JobDescription;

            descView.ShowDialog();

            if (descView.DialogResult == true)
            {
                jobPosting.JobDescription = editDescriptionViewModel.JobDescription;
            }
        }

        private void addNewButton_Click(object sender, RoutedEventArgs e)
        {
            NewPostingView addnewWindow = new NewPostingView()
            {
                Owner = this
            };
            addnewWindow.ShowDialog();
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView settingsView = new SettingsView() { Owner = this };
            settingsView.ShowDialog();

            if (settingsView.DatabaseChanged)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                vm.RefreshJobPostings();
            }

            if(settingsView.CurrentLocationChanged)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                vm.RecalculateAllJobPostingDistances();
            }
        }

        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            Trace.WriteLine("DataGrid_CellEditEnding");

            JobPosting rowPosting = (JobPosting)e.Row.Item;
            if(e.EditingElement is TextBox)
            {
                TextBox element = (TextBox)e.EditingElement;
                string newValue = element.Text;
                UpdateJobPostingByColumnHeader(rowPosting, (string)e.Column.Header, newValue);
            }
            else if(e.EditingElement is ComboBox)
            {
                ComboBox element = (ComboBox)e.EditingElement;
                int newValue = Convert.ToInt32(element.SelectedItem);
                UpdateJobPostingByColumnHeader(rowPosting, (string)e.Column.Header, newValue);
            }
        }

        private void UpdateJobPostingByColumnHeader(JobPosting posting, string columnHeaderName, string value)
        {
            switch (columnHeaderName)
            {
                case "Title":
                    posting.JobTitle = value; break;
                case "CompanyName":
                    posting.JobCompanyName = value; break;
                case "PostingURL":
                    posting.JobPostingURL = value; break;
                case "Location":
                    posting.JobLocation = value; break;
                case "Description":
                    posting.JobDescription = value; break;
                default:
                    break;
            }

            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            db.UpdateJobPosting(posting);
        }

        private void UpdateJobPostingByColumnHeader(JobPosting posting, string columnHeaderName, int value)
        {
            switch (columnHeaderName)
            {
                case "Type":
                    posting.JobType = (JobType)value; break;
                case "Arrangement":
                    posting.JobArrangement = (JobArrangement)value; break;
                case "Status":
                    posting.JobStatus = (JobStatus)value; break;
                default:
                    break;
            }

            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            db.UpdateJobPosting(posting);
        }

        private void jobsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            JobPosting? selectedPosting = (JobPosting)jobsDataGrid.SelectedItem;
            DataGridCellInfo focusedCell = jobsDataGrid.CurrentCell;
            if(selectedPosting != null && jobsDataGrid.SelectedCells.Count > 6 && (string)focusedCell.Column.Header == "Location")
            {
                EditLocationView dialog = new EditLocationView(selectedPosting)
                {
                    Owner = this
                };
                dialog.ShowDialog();
                jobsDataGrid.CommitEdit();
            }
            
        }
    }
}