using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.IO;
using System.Text;

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

        public enum ResponseResult
        {
            OK,            // Has a result!
            NoResult,      // Query could noot find anything.
            ServerError,   // Server gave unexpected output
            NetworkError,  // Internet down, DNS, etc.
            JSONError      // JSON decoding failed.
        }

        public class GeoCodingResponse
        {
            public GeoCodingResponse(ResponseResult ok)
            {
                Result = ok;
                Coords = new GeoCoordinate();
                DisplayName = string.Empty;
            }

            public GeoCodingResponse(ResponseResult ok, double latitude, double longitude, string displayName)
            {
                Result = ok;
                Coords = new GeoCoordinate(latitude, longitude);
                DisplayName = displayName;
            }

            public ResponseResult Result;

            public GeoCoordinate Coords { get; set; }

            public string DisplayName { get; set; }
        }

        public GeoCodingResponse GetLocationCoordinates(string query);

        public GeoCodingStatus GetGeoCodingStatus();
    }
}
