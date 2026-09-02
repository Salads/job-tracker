using MainApp.ViewModels;
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

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for EditLocationView.xaml
    /// </summary>
    public partial class EditLocationView : Window, IDisposable
    {
        public EditLocationView(JobPosting posting)
        {
            InitializeComponent();

            EditLocationViewModel vm = (EditLocationViewModel)DataContext;
            vm.RequestClose += OnRequestClose;

            Loaded += (_, _) =>
            {
                InitializeAndVerify(posting);
            };

            UpdateLayout();
        }

        public void InitializeAndVerify(JobPosting posting)
        {
            locationControl.InitializeAndVerify(posting.JobLocation);

            EditLocationViewModel vm = (EditLocationViewModel)DataContext;
            vm.JobPosting = posting;
            vm.LocationInput = posting.JobLocation;

            UpdateLayout();
        }

        private void OnRequestClose(bool save)
        {
            DialogResult = save;
            Close();
        }

        public void Dispose()
        {
            EditLocationViewModel vm = (EditLocationViewModel)DataContext;
            vm.RequestClose -= OnRequestClose;
        }
    }
}
