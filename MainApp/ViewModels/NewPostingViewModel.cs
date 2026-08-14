using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MainApp.Models;
using System.ComponentModel.DataAnnotations;
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

            SaveCommand = new RelayCommand<Window>(OnSaveCommand, _ => !HasErrors);
            CancelCommand = new RelayCommand<Window>(OnCancelCommand);

            ErrorsChanged += (_, _) => ((RelayCommand<Window>)SaveCommand).NotifyCanExecuteChanged();
            ValidateAllProperties();
        }

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

        [Required]
        [NotifyDataErrorInfo]
        [ObservableProperty]
        public partial string Location { get; set; } = string.Empty;

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
                JobDescription = JobDescription,
                JobStatus = JobStatus
            };
        }

        private void OnSaveCommand(Window? view)
        {
            if(view != null)
            {
                view.DialogResult = true;
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
    }
}
