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

            LocationVM = new LocationViewModel();

            ValidateLocationCommand = new RelayCommand(OnValidateLocationCommand);

            SaveLocation = Settings.Default.SaveLocation;
            Location = Settings.Default.CurrentLocation;

            OnValidateLocationCommand();

            ValidateAllProperties();
        }

        public ICommand ValidateLocationCommand { get; }

        private LocationViewModel LocationVM { get; set; }

        [ObservableProperty]
        public partial IGeocodingService.ResponseResult LocationValid { get; set; } = IGeocodingService.ResponseResult.Uninitialized;

        public string LocationError { get; set; } = "Location has not been verified.";

        [ObservableProperty]
        public partial string LocationFullName { get; set; } = string.Empty;

        public bool ShowLocationFullName => LocationValid == IGeocodingService.ResponseResult.OK;

        partial void OnLocationValidChanged(IGeocodingService.ResponseResult value)
            => OnPropertyChanged(nameof(ShowLocationFullName));

        [Required]
        [ObservableProperty]
        public partial string SaveLocation { get; set; }

        [ObservableProperty]
        [CustomValidation(typeof(SettingsViewModel), nameof(ValidateLocation))]
        public partial string Location { get; set; } = string.Empty;

        private void ValidateLocation() => ValidateProperty(Location, nameof(Location));

        partial void OnLocationChanged(string value)
        {
            LocationFullName = string.Empty;
            ValidateLocation();
        }

        private void OnValidateLocationCommand()
        {
            if (string.IsNullOrWhiteSpace(Location))
            {
                LocationError = "Required";
                LocationValid = IGeocodingService.ResponseResult.Uninitialized;
                ValidateLocation();
                return;
            }

            IGeocodingService.GeoCodingResponse result = LocationVM.GetLocationCoords(Location);
            LocationValid = result.Result;

            if (result.Result == IGeocodingService.ResponseResult.OK)
            {
                LocationFullName = result.DisplayName;
                LocationError = string.Empty;
            }
            else
            {
                LocationError = result.Result switch
                {
                    IGeocodingService.ResponseResult.Uninitialized => "Required",
                    IGeocodingService.ResponseResult.ServerError => "Unexpected Server output",
                    IGeocodingService.ResponseResult.NetworkError => "Client-side network error",
                    IGeocodingService.ResponseResult.JSONError => "Error decoding server JSON response",
                    IGeocodingService.ResponseResult.NoResult => "No location found",
                    _ => "Unknown Error"
                };
            }

            ValidateLocation();
        }

        public static ValidationResult ValidateLocation(string location, ValidationContext context)
        {
            // Reset error state to uninitialized/unverified

            SettingsViewModel vm = (SettingsViewModel)context.ObjectInstance;
            if (vm.LocationValid == IGeocodingService.ResponseResult.OK)
            {
                return ValidationResult.Success!;
            }

            return new(vm.LocationError);
        }
    }
}
