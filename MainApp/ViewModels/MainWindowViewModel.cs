using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MainApp.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel() 
        {
            AddNewPostingCommand = new RelayCommand<Window>(OpenNewPostingWindow);
        }

        public ICommand AddNewPostingCommand { get; }

        private void OpenNewPostingWindow(Window? owner)
        {
            NewPostingWindow addnewWindow = new NewPostingWindow()
            {
                Owner = owner
            };
            addnewWindow.ShowDialog();
        }
    }
}
