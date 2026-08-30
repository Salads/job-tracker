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

            SaveCommand = new RelayCommand(OnSaveCommand, () => !HasErrors && LocationValid);
            CancelCommand = new RelayCommand(OnCancelCommand);

            ErrorsChanged += (_, _) => ((RelayCommand)SaveCommand).NotifyCanExecuteChanged();
            ValidateAllProperties();
        }

        public event Action? RequestClose;

        [ObservableProperty]
        public partial string LocationInput { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LocationResult { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool LocationValid { get; set; } = false;

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
        public partial string Location { get; set; } = string.Empty;

        public int Distance { get; set; }

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
        private void OnSaveCommand()
        {
            if(!LocationValid)
            {
                throw new Exception("OnSaveCommand called when !LocationValid - SHOULD NOT HAPPEN");
            }

            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;

            var curLocationResponse = gc.GetLocationCoordinates(Settings.Default.CurrentLocation);
            var jobLocationResponse = gc.GetLocationCoordinates(Location);

            if (curLocationResponse.Result == IGeocodingService.ResponseResult.OK && jobLocationResponse.Result == IGeocodingService.ResponseResult.OK)
            {
                float fDistance = (float)curLocationResponse.Coords.GetDistanceTo(jobLocationResponse.Coords) / 1609.344f; // We get it in meters, convert to miles
                Distance = (int)fDistance;
            }
            else
            {
                Distance = -1;
            }

            db.AddNewJob(GetJobPosting());

            RequestClose?.Invoke();
        }

        private void OnCancelCommand()
        {
            RequestClose?.Invoke();
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
    }
}
