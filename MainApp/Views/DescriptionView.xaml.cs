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
    /// Interaction logic for DescriptionView.xaml
    /// </summary>
    public partial class DescriptionView : Window
    {
        public DescriptionView()
        {
            InitializeComponent();

            ViewModel.CloseRequest += OnRequestClose;
        }

        private void OnRequestClose()
        {
            Close();
        }
    }
}
