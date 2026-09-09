using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;

namespace MainApp.ViewModels
{
    public partial class SettingsViewModel : ObservableValidator
    {
        public SettingsViewModel()
        {
            SaveLocation = Settings.Default.SaveLocation;
            LocationInput = Settings.Default.CurrentLocation;

            ValidateAllProperties();
        }

        public event EventHandler? RequestClose;

        [ObservableProperty]
        public partial string LocationInput { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LocationFullName { get; set; } = string.Empty;

        [AllowedValues(true)]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial bool LocationValid { get; set; } = false;

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string SaveLocation { get; set; } = string.Empty;

        [RelayCommand]
        private void ResetSaveFilePath()
        {
            SaveLocation = (string)Settings.Default.Properties["SaveLocation"].DefaultValue;
        }

        [RelayCommand]
        private void BrowseSaveFilePath()
        {
            IDialogService dialogService = App.Current.Services.GetService<IDialogService>()!;
            string? newSaveFilePath = dialogService.GetDatabaseSaveLocation();
            if(newSaveFilePath != null)
            {
                SaveLocation = newSaveFilePath;
            }
        }

        [RelayCommand(CanExecute = nameof(SaveCommandCanExecute))]
        private void Save()
        {
            Settings.Default.SaveLocation = SaveLocation;
            Settings.Default.CurrentLocation = LocationInput;
            Settings.Default.Save();

            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private bool SaveCommandCanExecute()
        {
            return !HasErrors && LocationValid;
        }

        [RelayCommand]
        private void Cancel()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
