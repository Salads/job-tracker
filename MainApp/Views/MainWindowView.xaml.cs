using CommunityToolkit.Mvvm.Input;
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
            DescriptionView descView = new DescriptionView()
            {
                Owner = this
            };

            DescriptionViewModel descViewModel = descView.ViewModel;
            descViewModel.Description = jobPosting.JobDescription;

            descView.ShowDialog();
        }

        private void addNewButton_Click(object sender, RoutedEventArgs e)
        {
            NewPostingView addnewWindow = new NewPostingView(null)
            {
                Owner = this
            };
            addnewWindow.ShowDialog();

            if (addnewWindow.DialogResult == true)
            {
                IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;

                long rowid = db.AddNewJob(addnewWindow.GetJobPosting());
                addnewWindow.GetJobPosting().RowID = rowid;

                ViewModel.JobPostings.Add(addnewWindow.ViewModel.JobPosting);
            }
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

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            JobPosting? selected = (JobPosting?)jobsDataGrid.SelectedItem;
            if(selected == null)
            {
                return;
            }

            NewPostingView editPostingWindow = new NewPostingView(selected)
            {
                Owner = this
            };
            editPostingWindow.ShowDialog();

            if (editPostingWindow.DialogResult == true && editPostingWindow.EditMode)
            {
                IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
                
                db.UpdateJobPosting(editPostingWindow.GetJobPosting());
                selected.SetFrom(editPostingWindow.GetJobPosting());
            }
        }
    }
}