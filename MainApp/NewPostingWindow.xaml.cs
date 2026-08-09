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

namespace MainApp
{
    /// <summary>
    /// Interaction logic for NewPostingWindow.xaml
    /// </summary>
    public partial class NewPostingWindow : Window
    {
        public NewPostingWindow()
        {
            InitializeComponent();

            jobTypeCombo.ItemsSource = Enum.GetValues<JobType>();
            jobArrangementCombo.ItemsSource = Enum.GetValues<JobArrangement>();
            jobStatusCombo.ItemsSource = Enum.GetValues<JobStatus>();

            jobTypeCombo.SelectedIndex = 0;
            jobArrangementCombo.SelectedIndex = 0;
            jobStatusCombo.SelectedIndex = 0;
        }
    }
}
