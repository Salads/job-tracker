using CommunityToolkit.Mvvm.ComponentModel;
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
    /// Interaction logic for EditDescriptionWindow.xaml
    /// </summary>
    public partial class EditDescriptionWindow : Window
    {
        public EditDescriptionWindow()
        {
            InitializeComponent();
        }

        private void descSaveButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void descCancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
