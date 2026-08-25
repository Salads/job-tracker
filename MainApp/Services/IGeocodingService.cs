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
        public enum GeoCodingError
        {
            Success,
            CouldNotConnect,
            ServerInternal,
            Params,
            TooFast,
            Blocked
        }

        public GeoCoordinate? GetLocationCoordinates(string city, string state);
    }
}
