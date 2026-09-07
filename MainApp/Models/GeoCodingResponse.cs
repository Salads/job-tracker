using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;
using static MainApp.Services.IGeocodingService;

namespace MainApp.Models
{
    public enum ResponseResult
    {
        Uninitialized, // No result yet, just uninitialized.
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

        public bool IsRemote { get; set; } = false;

        public GeoCoordinate Coords { get; set; }

        public string DisplayName { get; set; }
    }
}
