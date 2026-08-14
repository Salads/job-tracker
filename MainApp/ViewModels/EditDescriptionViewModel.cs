using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MainApp.ViewModels
{
    public partial class EditDescriptionViewModel : ObservableValidator
    {
        public EditDescriptionViewModel() 
        {
            SaveCommand = new RelayCommand<Window>(OnSaveCommand, _ => !HasErrors);
            CancelCommand = new RelayCommand<Window>(OnCancelCommand);

            ErrorsChanged += (_, _) => ((RelayCommand<Window>)SaveCommand).NotifyCanExecuteChanged();
            ValidateAllProperties();
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand {  get; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string JobDescription { get; set; } = string.Empty;
        
        private void OnSaveCommand(Window? view)
        {
            if (view != null)
            {
                view.DialogResult = true;
                view.Close();
            }
        }
        
        private void OnCancelCommand(Window? view)
        {
            if (view != null)
            {
                view.DialogResult = false;
                view.Close();
            }
        }
    }
}
