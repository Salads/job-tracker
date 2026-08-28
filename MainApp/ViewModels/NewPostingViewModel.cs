using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Models;
using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace MainApp.ViewModels
{
    public partial class NewPostingWindowViewModel : ObservableValidator
    {
        public NewPostingWindowViewModel()
        {
            JobDescriptionLabel = "(0 chars)";

            ValidateLocationCommand = new RelayCommand(OnValidateLocationCommand);
            SaveCommand = new RelayCommand<Window>(OnSaveCommand, _ => !HasErrors);
            CancelCommand = new RelayCommand<Window>(OnCancelCommand);

            ErrorsChanged += (_, _) => ((RelayCommand<Window>)SaveCommand).NotifyCanExecuteChanged();
            ValidateAllProperties();
        }

        public LocationViewModel LocationVM { get; set; } = new LocationViewModel();

        [ObservableProperty]
        public partial IGeocodingService.ResponseResult LocationValid { get; set; } = IGeocodingService.ResponseResult.Uninitialized;

        public string LocationError { get; set; } = "Location has not been verified.";

        [ObservableProperty]
        public partial string LocationFullName { get; set; } = string.Empty;

        public bool ShowLocationFullName => LocationValid == IGeocodingService.ResponseResult.OK;

        partial void OnLocationValidChanged(IGeocodingService.ResponseResult value)
            => OnPropertyChanged(nameof(ShowLocationFullName));

        #region New Job Properties
        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string JobTitle { get; set; } = string.Empty;

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string CompanyName { get; set; } = string.Empty;

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        [CustomValidation(typeof(NewPostingWindowViewModel), nameof(ValidatePostingURL))]
        public partial string PostingURL { get; set; } = string.Empty;

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial JobType JobType { get; set; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial JobArrangement JobArrangement { get; set; }

        [ObservableProperty]
        [CustomValidation(typeof(NewPostingWindowViewModel), nameof(ValidateLocation))]
        public partial string Location { get; set; } = string.Empty;

        public float Distance { get; set; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string JobDescription { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string JobDescriptionLabel { get; set; }

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial JobStatus JobStatus { get; set; } = JobStatus.Applied;
        #endregion

        public ICommand ValidateLocationCommand { get; }

        public ICommand SaveCommand { get; }

        public ICommand CancelCommand { get; }

        public JobPosting GetJobPosting()
        {
            return new JobPosting()
            {
                JobTitle = JobTitle,
                JobCompanyName = CompanyName,
                JobPostingURL = PostingURL,
                JobType = JobType,
                JobArrangement = JobArrangement,
                JobLocation = Location,
                JobDistance = Distance,
                JobDescription = JobDescription,
                JobStatus = JobStatus
            };
        }

        private void ValidateLocation() => ValidateProperty(Location, nameof(Location));

        partial void OnLocationChanged(string value)
        {
            LocationFullName = string.Empty;
            ValidateLocation();
        }

        private void OnValidateLocationCommand()
        {
            if(string.IsNullOrWhiteSpace(Location))
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

        private void OnSaveCommand(Window? view)
        {
            if(view != null)
            {
                view.DialogResult = true;

                var curLocationResponse = LocationVM.GetLocationCoords(Settings.Default.CurrentLocation);
                var jobLocationResponse = LocationVM.GetLocationCoords(Location);

                if (curLocationResponse.Result == IGeocodingService.ResponseResult.OK && jobLocationResponse.Result == IGeocodingService.ResponseResult.OK)
                {
                    Distance = (float)curLocationResponse.Coords.GetDistanceTo(jobLocationResponse.Coords) / 1609.344f; // We get it in meters, convert to miles.
                }
                else
                {
                    Distance = -1;
                }

                view.Close();
            }

            Trace.WriteLine(GetJobPosting().ToString());
        }

        private void OnCancelCommand(Window? view)
        {
            if (view != null)
            {
                view.DialogResult = false;
                view.Close();
            }
        }

        public static ValidationResult ValidatePostingURL(string postingURL, ValidationContext context)
        {
            bool result = Uri.TryCreate(postingURL, UriKind.Absolute, out _);

            if (!result)
            {
                // TODO(Salads): Try to deconstruct it to see if we can correct it for the user.
                //               Ideally, shouldn't happen since URLs are usually copy-pasted.
            }

            if (result)
            {
                return ValidationResult.Success!;
            }
            else
            {
                return new("Not a valid HTTP/S URL!");
            }
        }

        public static ValidationResult ValidateLocation(string location, ValidationContext context)
        {
            // Reset error state to uninitialized/unverified

            NewPostingWindowViewModel vm = (NewPostingWindowViewModel)context.ObjectInstance;
            if(vm.LocationValid == IGeocodingService.ResponseResult.OK)
            {
                return ValidationResult.Success!;
            }

            return new(vm.LocationError);
        }
    }
}
