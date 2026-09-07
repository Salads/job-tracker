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
using System.Windows.Controls.Primitives;
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
                ViewModel.AddNewJob(addnewWindow.GetJobPosting());
            }
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsView settingsView = new SettingsView() { Owner = this };
            settingsView.ShowDialog();

            if (settingsView.DatabaseChanged)
            {
                ViewModel.RefreshJobPostings();
            }

            if(settingsView.CurrentLocationChanged)
            {
                ViewModel.RecalculateAllJobPostingDistances();
            }
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            JobPosting? selected = (JobPosting?)jobsDataGrid.SelectedItem;
            if(selected == null)
            {
                return;
            }

            OpenEditViewForPosting(selected);
        }

        private void ContextMenuItemEdit_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
            DataGridRow row = (DataGridRow)contextMenu.PlacementTarget;
            JobPosting selected = (JobPosting)row.DataContext;

            OpenEditViewForPosting(selected);
        }

        private void OpenEditViewForPosting(JobPosting posting)
        {
            NewPostingView editPostingWindow = new NewPostingView(posting)
            {
                Owner = this
            };
            editPostingWindow.ShowDialog();

            if (editPostingWindow.DialogResult == true && editPostingWindow.EditMode)
            {
                ViewModel.UpdatePosting(posting, editPostingWindow.GetJobPosting());
            }
        }
    }
}