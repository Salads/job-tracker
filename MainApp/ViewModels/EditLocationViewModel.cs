using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using MainApp.Models;

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

        private async void OnSaveCommand()
        {
            if (JobPosting != null)
            {
                JobPosting.JobLocation = LocationInput;

                // Make sure to update the distance to the new location
                IGeocodingService gService = App.Current.Services.GetService<IGeocodingService>()!;
                GeoCodingResponse curLocationResponse = await gService.GetLocationCoordinatesAsync(Settings.Default.CurrentLocation);
                GeoCodingResponse jobLocationResponse = await gService.GetLocationCoordinatesAsync(LocationInput);

                IDistanceCalculatorService distService = App.Current.Services.GetService<IDistanceCalculatorService>()!;
                IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;

                JobPosting.JobDistance = (int)distService.GetDistanceBetween(curLocationResponse, jobLocationResponse);
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
