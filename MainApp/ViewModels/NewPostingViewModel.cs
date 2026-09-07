using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Models;
using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.Device.Location;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Input;

namespace MainApp.ViewModels
{
    public partial class NewPostingWindowViewModel : ObservableValidator
    {
        public NewPostingWindowViewModel()
        {
            JobDescriptionLabel = "(0 chars)";

            SaveCommand = new RelayCommand(OnSaveCommand, () => !JobPosting.HasErrors && LocationValid);
            CancelCommand = new RelayCommand(OnCancelCommand);

            JobPosting.ErrorsChanged += (_, _) => ((RelayCommand)SaveCommand).NotifyCanExecuteChanged();

            ValidateAllProperties();
        }

        public event Action<bool, bool>? RequestClose;

        [ObservableProperty]
        public partial string LocationInput { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string LocationResult { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial bool LocationValid { get; set; } = false;

        [ObservableProperty]
        public partial string JobDescriptionLabel { get; set; }

        [ObservableProperty]
        public partial JobPosting JobPosting { get; private set; } = new JobPosting();

        [ObservableProperty]
        public partial bool EditMode { get; set; }

        public IRelayCommand SaveCommand { get; }

        public IRelayCommand CancelCommand { get; }

        public void SetEditPosting(JobPosting posting)
        {
            JobPosting.SetFrom(posting);
            EditMode = true;
            JobPosting.ValidateAllPropertiesManually();
        }

        private async void OnSaveCommand()
        {
            if(!LocationValid)
            {
                throw new Exception("OnSaveCommand called when !LocationValid - SHOULD NOT HAPPEN");
            }

            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;

            var curLocationResponse = await gc.GetLocationCoordinatesAsync(Settings.Default.CurrentLocation);
            var jobLocationResponse = await gc.GetLocationCoordinatesAsync(JobPosting.JobLocation);

            IDistanceCalculatorService distCalcService = App.Current.Services.GetService<IDistanceCalculatorService>()!;
            float fDistance = distCalcService.GetDistanceBetween(curLocationResponse, jobLocationResponse);
            JobPosting.JobDistance = (int)fDistance;

            RequestClose?.Invoke(true, EditMode);
        }

        private void OnCancelCommand()
        {
            RequestClose?.Invoke(false, EditMode);
        }
    }
}
