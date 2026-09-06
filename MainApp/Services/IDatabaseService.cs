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

        public string? GetLocationMappingFromCache(string inputLocation);

        public void EnsureLocationMappingExists(string inputLocation, string fullLocation);

        public void EnsureLocationCoordsExists(string fullLocation, GeoCoordinate coords);

        public GeoCoordinate? GetLocationCoordsFromCache(string fullName);

        public long AddNewJob(JobPosting newJobPosting);

        public void UpdateJobPosting(JobPosting posting);

        public void RefreshJobPostings(ObservableCollection<JobPosting> jobPostings);

        public void RemoveJobPosting(JobPosting posting);
    }
}
