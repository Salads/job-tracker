using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp.ViewModels
{
    public partial class DescriptionViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial string Description { get; set; } = string.Empty;

        public event Action? CloseRequest;

        [RelayCommand]
        public void RequestClose()
        {
            CloseRequest?.Invoke();
        }
    }
}
