using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace MainApp
{
    internal partial class JobPosting : ObservableObject
    {
        [ObservableProperty]
        string jobTitle;

        [ObservableProperty]
        string companyName;

        [ObservableProperty]
        Uri postingURL;

        [ObservableProperty]
        JobType jobType;

        [ObservableProperty]
        JobArrangement jobArrangement;

        [ObservableProperty]
        string location;

        [ObservableProperty]
        float distance;

        [ObservableProperty]
        string jobDescription;

        [ObservableProperty]
        JobStatus jobStatus;
    }
}
