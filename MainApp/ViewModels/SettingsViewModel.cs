using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;
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
            SaveCommand = new RelayCommand(OnSaveCommand, () => !HasErrors && LocationValid);
            CancelCommand = new RelayCommand(OnCancelCommand);

            SaveLocation = Settings.Default.SaveLocation;
            LocationInput = Settings.Default.CurrentLocation;

            ValidateAllProperties();
        }

        public event EventHandler? RequestClose;

        public IRelayCommand SaveCommand { get; set; }

        public IRelayCommand CancelCommand { get; set; }

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

        private void OnSaveCommand()
        {
            Settings.Default.SaveLocation = SaveLocation;
            Settings.Default.CurrentLocation = LocationInput;
            Settings.Default.Save();

            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void OnCancelCommand()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }
    }
}
