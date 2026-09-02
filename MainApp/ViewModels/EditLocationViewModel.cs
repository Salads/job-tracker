using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MainApp.ViewModels
{
    public partial class EditLocationViewModel : ObservableValidator
    {
        public EditLocationViewModel()
        {
            SaveCommand = new RelayCommand(OnSaveCommand, () => !HasErrors && LocationValid);
            CancelCommand = new RelayCommand(OnCancelCommand);
        }

        public event Action<bool>? RequestClose;

        public IRelayCommand SaveCommand { get; set; }

        public IRelayCommand CancelCommand { get; set; }

        [ObservableProperty]
        public partial string LocationInput { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LocationResult { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial bool LocationValid { get; set; } = false;

        public JobPosting? JobPosting { get; set; } = null;

        private void OnSaveCommand()
        {
            if (JobPosting != null)
            {
                JobPosting.JobLocation = LocationInput;

                // Make sure to update the distance to the new location
                IGeocodingService gService = App.Current.Services.GetService<IGeocodingService>()!;
                IGeocodingService.GeoCodingResponse curLocationResponse = gService.GetLocationCoordinates(Settings.Default.CurrentLocation);
                IGeocodingService.GeoCodingResponse jobLocationResponse = gService.GetLocationCoordinates(LocationInput);
                if(curLocationResponse.Result == IGeocodingService.ResponseResult.OK && jobLocationResponse.Result == IGeocodingService.ResponseResult.OK)
                {
                    IDistanceCalculatorService distService = App.Current.Services.GetService<IDistanceCalculatorService>()!;
                    JobPosting.JobDistance = (int)distService.GetDistanceBetween(curLocationResponse.Coords, jobLocationResponse.Coords);
                }

                IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
                db.UpdateJobPosting(JobPosting);
            }

            RequestClose?.Invoke(true);
        }

        private void OnCancelCommand()
        {
            RequestClose?.Invoke(false);
        }
    }
}
