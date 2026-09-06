using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO;
using System.Text;
using MainApp.Models;

namespace MainApp.Services
{
    public interface IGeocodingService
    { 
        public class GeoCodingStatus
        {
            public GeoCodingStatus(bool ok, string description)
            {
                OK = ok;
                Description = description;
            }

            public bool OK { get; set; }

            public string Description { get; set; }
        }

        public Task<GeoCodingResponse> GetLocationCoordinatesAsync(string query);

        public GeoCodingStatus GetGeoCodingStatus();
    }
}
