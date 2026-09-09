using CommunityToolkit.Mvvm.ComponentModel;
using MainApp.Controls;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MainApp.Views
{
    /// <summary>
    /// Interaction logic for LocationControl.xaml
    /// </summary>
    public partial class LocationView : UserControl
    {
        public LocationView()
        {
            InitializeComponent();

            LocationAdorner = new LocationAdorner(locationTextBox)
            {
                Trimming = TextTrimming.CharacterEllipsis,
                TypeFace = "Arial",
                TextColor = Brushes.DarkSlateGray
            };

            #region Adorner Initialization
            Binding visibilityBinding = new Binding(nameof(LocationViewModel.IsValid))
            {
                Source = DataContext,
                Converter = new BooleanToVisibilityConverter()
            };
            BindingOperations.SetBinding(LocationAdorner, UIElement.VisibilityProperty, visibilityBinding);

            Binding textBinding = new Binding(nameof(LocationViewModel.LocationResult))
            {
                Source = DataContext
            };
            BindingOperations.SetBinding(LocationAdorner, LocationAdorner.TextProperty, textBinding);

            Loaded += (_, _) =>
            {
                AdornerLayer layer = AdornerLayer.GetAdornerLayer(locationTextBox);
                layer.Add(LocationAdorner);
            };
            #endregion

            LocationViewModel vm = (LocationViewModel)DataContext;
            vm.PropertyChanged += (_, e) =>
            {
                if(e.PropertyName == nameof(LocationViewModel.LocationInput))
                {
                    SetCurrentValue(LocationInputProperty, vm.LocationInput);
                }
                else if(e.PropertyName == nameof(LocationViewModel.LocationResult))
                {
                    SetCurrentValue(LocationResultProperty, vm.LocationResult);
                }
                else if(e.PropertyName == nameof(LocationViewModel.IsValid))
                {
                    SetCurrentValue(IsValidProperty, vm.IsValid);
                }
            };

            UpdateLayout();
        }

        public LocationAdorner LocationAdorner { get; set; }

        public void InitializeAndVerify(string inputLocation)
        {
            LocationViewModel vm = (LocationViewModel)DataContext;
            vm.InitializeAndVerify(inputLocation);
        }

        #region LocationInputProperty
        public static readonly DependencyProperty LocationInputProperty = DependencyProperty.Register(
            name: "LocationInput", 
            propertyType: typeof(string),
            ownerType: typeof(LocationView),
            typeMetadata: new PropertyMetadata(defaultValue: string.Empty, OnLocationInputPropertyChanged)
        );

        private static void OnLocationInputPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LocationView control || control.DataContext is not LocationViewModel vm)
            {
                return;
            }

            vm.LocationInput = (string)e.NewValue!;
        }

        public string LocationInput
        {
            get => (string)GetValue(LocationInputProperty);
            set => SetValue(LocationInputProperty, value);
        }
        #endregion

        #region LocationResultProperty
        public static readonly DependencyProperty LocationResultProperty = DependencyProperty.Register(
            name: "LocationResult",
            propertyType: typeof(string),
            ownerType: typeof(LocationView),
            typeMetadata: new PropertyMetadata(defaultValue: string.Empty)
        );

        public string LocationResult
        {
            get => (string)GetValue(LocationResultProperty);
            set => SetValue(LocationResultProperty, value);
        }
        #endregion

        #region IsValidProperty
        public static readonly DependencyProperty IsValidProperty = DependencyProperty.Register(
            name: "IsValid",
            propertyType: typeof(bool),
            ownerType: typeof(LocationView),
            typeMetadata: new PropertyMetadata(defaultValue: false)
        );

        public bool IsValid
        {
            get => (bool)GetValue(IsValidProperty);
            set => SetValue(IsValidProperty, value);
        }
        #endregion
    }
}
