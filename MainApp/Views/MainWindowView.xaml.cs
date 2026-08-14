using MainApp.Services;
using MainApp.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
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
    public partial class MainWindow : Window
    {
        public MainWindow()
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
            EditDescriptionWindow descView = new EditDescriptionWindow()
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
            NewPostingWindow addnewWindow = new NewPostingWindow()
            {
                Owner = this
            };
            addnewWindow.ShowDialog();

            if (addnewWindow.DialogResult == true)
            {
                MainWindowViewModel vm = (MainWindowViewModel)DataContext;
                JobPosting newPosting = ((NewPostingWindowViewModel)addnewWindow.DataContext).GetJobPosting();
                vm.AddNewJob(newPosting);
            }
        }
    }
}