using MainApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Net;
using System.Text;

namespace MainApp.ViewModels
{
    public partial class LocationViewModel
    {
        public IGeocodingService.GeoCodingResponse GetLocationCoords(string location)
        {
            // Check database for cached results.
            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            string? fullName = db.GetLocationMappingFromCache(location);
            if (fullName != null)
            {
                IGeocodingService.GeoCodingResponse result = new IGeocodingService.GeoCodingResponse(IGeocodingService.ResponseResult.OK)
                {
                    Coords = db.GetLocationCoordsFromCache(fullName)!,
                    DisplayName = fullName
                };

                return result;
            }

            // Could not get name mapping, so we need to make a geocoding request.
            IGeocodingService gc = App.Current.Services.GetService<IGeocodingService>()!;
            IGeocodingService.GeoCodingResponse response = gc.GetLocationCoordinates(location);
            if(response.Result == IGeocodingService.ResponseResult.OK)
            {
                // Update cache with our new data.
                db.EnsureLocationMappingExists(location, response.DisplayName);
                db.EnsureLocationCoordsExists(response.DisplayName, response.Coords);
            }

            return response;
        }
    }
}
