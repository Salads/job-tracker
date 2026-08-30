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
        [CustomValidation(typeof(LocationViewModel), nameof(UpdateValidationError))]
        public partial string LocationInput { get; set; } = string.Empty;

        public IGeocodingService.ResponseResult LastResult = IGeocodingService.ResponseResult.Uninitialized;

        [ObservableProperty]
        public partial bool IsValid { get; set; } = true;

        [ObservableProperty]
        public partial string LocationResult { get; set; } = string.Empty;

        public string Error { get; set; } = "No Errors";

        private void ValidateInputLocation() => ValidateProperty(LocationInput, nameof(LocationInput));

        partial void OnLocationInputChanged(string value)
        {
            LocationResult = string.Empty;
            LastResult = IGeocodingService.ResponseResult.Uninitialized;
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

        private void OnValidateLocationCommand()
        {
            Trace.WriteLine("LocationViewMode:OnValidateLocationCommand()");

            if (string.IsNullOrWhiteSpace(LocationInput))
            {
                Error = "Required";
                LocationResult = string.Empty;
                LastResult = IGeocodingService.ResponseResult.Uninitialized;
                IsValid = LastResult == IGeocodingService.ResponseResult.OK;
                ValidateInputLocation();
                return;
            }

            IGeocodingService.GeoCodingResponse result = GetLocationCoords(LocationInput);
            LastResult = result.Result;
            IsValid = result.Result == IGeocodingService.ResponseResult.OK;

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
                    IGeocodingService.ResponseResult.Uninitialized => "Required",
                    IGeocodingService.ResponseResult.ServerError => "Unexpected Server output",
                    IGeocodingService.ResponseResult.NetworkError => "Client-side network error",
                    IGeocodingService.ResponseResult.JSONError => "Error decoding server JSON response",
                    IGeocodingService.ResponseResult.NoResult => "No location found",
                    _ => "Unknown Error"
                };
            }

            ValidateInputLocation();
        }

        public IGeocodingService.GeoCodingResponse GetLocationCoords(string location)
        {
            // Check database for cached results.
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            string? fullName = db.GetLocationMappingFromCache(location);
            if (fullName != null)
            {
                IGeocodingService.GeoCodingResponse result = new IGeocodingService.GeoCodingResponse(IGeocodingService.ResponseResult.OK)
                {
                    Coords = db.GetLocationCoordsFromCache(fullName)!,
                    DisplayName = fullName
                };

                return result;
            }

            // Could not get name mapping, so we need to make a geocoding request.
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;
            IGeocodingService.GeoCodingResponse response = gc.GetLocationCoordinates(location);
            if(response.Result == IGeocodingService.ResponseResult.OK)
            {
                // Update cache with our new data.
                db.EnsureLocationMappingExists(location, response.DisplayName);
                db.EnsureLocationCoordsExists(response.DisplayName, response.Coords);
            }

            return response;
        }
    }
}
