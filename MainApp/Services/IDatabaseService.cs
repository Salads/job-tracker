using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Text;

namespace MainApp.Services
{
    public interface IDatabaseService
    {
        public int? GetJobsDBVersion();

        public string? GetFullNameForInput(string inputLocation);

        public void AddLocationMappingToCache(string inputLocation, string fullLocation);

        public void AddLocationToCoordsCache(string fullLocation, GeoCoordinate coords);

        public GeoCoordinate? GetLocationFromCache(string fullName);

        public long AddNewJob(JobPosting newJobPosting);

        public void UpdateJobPosting(JobPosting posting);

        public void RefreshJobPostings(ObservableCollection<JobPosting> jobPostings);
    }
}
