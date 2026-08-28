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

            LocationAdorner = new LocationAdorner(locationTextBox)
            {
                Trimming = TextTrimming.CharacterEllipsis,
                TypeFace = "Arial",
                TextColor = Brushes.DarkSlateGray
            };

            Binding visibilityBinding = new Binding(nameof(NewPostingWindowViewModel.ShowLocationFullName))
            {
                Source = DataContext,
                Converter = new BooleanToVisibilityConverter()
            };
            BindingOperations.SetBinding(LocationAdorner, UIElement.VisibilityProperty, visibilityBinding);

            Binding textBinding = new Binding(nameof(NewPostingWindowViewModel.LocationFullName))
            {
                Source = DataContext
            };
            BindingOperations.SetBinding(LocationAdorner, LocationAdorner.TextProperty, textBinding);

            Loaded += (_, _) => 
            {
                MaxHeight = MinHeight = ActualHeight;

                // Setup the Adorner
                AdornerLayer locationLayer = AdornerLayer.GetAdornerLayer(locationTextBox);
                locationLayer.Add(LocationAdorner);
            };

            jobTypeCombo.ItemsSource = Enum.GetValues<JobType>();
            jobArrangementCombo.ItemsSource = Enum.GetValues<JobArrangement>();
            jobStatusCombo.ItemsSource = Enum.GetValues<JobStatus>();

            UpdateLayout();
        }

        private LocationAdorner LocationAdorner { get; set; }

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
