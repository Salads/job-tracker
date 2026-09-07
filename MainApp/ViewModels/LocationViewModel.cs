using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MainApp.Models;

namespace MainApp.ViewModels
{
    public partial class LocationViewModel : ObservableValidator
    {
        public LocationViewModel()
        {
            ValidateLocationCommand = new RelayCommand(OnValidateLocationCommand);
        }

        public ICommand ValidateLocationCommand { get; }

        [ObservableProperty]
        public partial bool IsNotValidating { get; set; }

        [ObservableProperty]
        [CustomValidation(typeof(LocationViewModel), nameof(UpdateValidationError))]
        public partial string LocationInput { get; set; } = string.Empty;

        public ResponseResult LastResult = ResponseResult.Uninitialized;

        [ObservableProperty]
        public partial bool IsValid { get; set; } = true;

        [ObservableProperty]
        public partial string LocationResult { get; set; } = string.Empty;

        public string Error { get; set; } = "No Errors";

        private void ValidateInputLocation() => ValidateProperty(LocationInput, nameof(LocationInput));

        partial void OnLocationInputChanged(string value)
        {
            LocationResult = string.Empty;
            LastResult = ResponseResult.Uninitialized;
            IsValid = false;
            Error = "Location not verified";
            ValidateInputLocation();
        }

        public void InitializeAndVerify(string inputLocation)
        {
            LocationInput = inputLocation;
            OnValidateLocationCommand();
        }

        public static ValidationResult UpdateValidationError(string inputLocation, ValidationContext context)
        {
            LocationViewModel vm = (LocationViewModel)context.ObjectInstance;
            if (vm.IsValid)
            {
                vm.ClearErrors();
                return ValidationResult.Success!;
            }

            return new ValidationResult(string.IsNullOrEmpty(vm.Error) ? "Location not verified" : vm.Error);
        }

        private async void OnValidateLocationCommand()
        {
            Trace.WriteLine("LocationViewMode:OnValidateLocationCommand()");

            IsNotValidating = false;

            if (string.IsNullOrWhiteSpace(LocationInput))
            {
                Error = "Required";
                LocationResult = string.Empty;
                LastResult = ResponseResult.Uninitialized;
                IsValid = LastResult == ResponseResult.OK;
                ValidateInputLocation();
                IsNotValidating = true;
                return;
            }

            string trimmedLocation = LocationInput.Trim().ToLower();
            if(trimmedLocation == "remote")
            {
                LocationResult = "Remote";
                LastResult = ResponseResult.OK;
                IsValid = LastResult == ResponseResult.OK;
                ValidateInputLocation();
                IsNotValidating = true;
                return;
            }

            GeoCodingResponse result = await GetLocationCoords(LocationInput);
            LastResult = result.Result;
            IsValid = result.Result == ResponseResult.OK;

            if (IsValid)
            {
                LocationResult = result.DisplayName;
                Error = "No Errors";
                ClearErrors();
            }
            else
            {
                Error = result.Result switch
                {
                    ResponseResult.Uninitialized => "Required",
                    ResponseResult.ServerError => "Unexpected Server output",
                    ResponseResult.NetworkError => "Client-side network error",
                    ResponseResult.JSONError => "Error decoding server JSON response",
                    ResponseResult.NoResult => "No location found",
                    _ => "Unknown Error"
                };
            }

            ValidateInputLocation();
            IsNotValidating = true;
        }

        public async Task<GeoCodingResponse> GetLocationCoords(string location)
        {
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;
            GeoCodingResponse response = await gc.GetLocationCoordinatesAsync(location);
            return response;
        }
    }
}
