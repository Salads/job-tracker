using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public override string ToString()
        {
            return $@"
                jobTitle:{jobTitle}
                companyName:{companyName}
                postingURL:{postingURL}
                jobType:{jobType}
                jobArrangement:{jobArrangement}
                location:{location}
                distance:{distance}
                jobDescription:{jobDescription}
                jobStatus:{jobStatus}";
        }
    }
}
