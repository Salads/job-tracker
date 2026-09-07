using MainApp.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Drawing.Text;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static MainApp.Services.IGeocodingService;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MainApp.Services
{
    public class NominatimService : IGeocodingService
    {
        const string APISearchURL = "https://nominatim.openstreetmap.org/search";
        const string APIStatusURL = "https://nominatim.openstreetmap.org/status";

        private DateTime lastRequestTime = DateTime.Now;

        private const float REQUEST_COOLDOWN = 1.5f;

        public GeoCodingStatus GetGeoCodingStatus()
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(APIStatusURL);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Job Tracker");

            using HttpResponseMessage response = httpClient.GetAsync("?format=text").Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            return new GeoCodingStatus(response.IsSuccessStatusCode, responseContent);
        }

        public async Task<GeoCodingResponse> GetLocationCoordinatesAsync(string query)
        {
            string trimmedQuery = query.Trim().ToLower();
            if(trimmedQuery == "remote")
            {
                GeoCodingResponse remoteResult = new GeoCodingResponse(ResponseResult.OK)
                {
                    Coords = new GeoCoordinate()
                    {
                        Latitude = 0,
                        Longitude = 0,
                    },
                    DisplayName = "Remote",
                    IsRemote = true
                };

                Trace.WriteLine($"NominatimService - REMOTE");
                return remoteResult;
            }

            IDatabaseService db = App.Current.Services.GetService<IDatabaseService>()!;
            string? fullName = db.GetLocationMappingFromCache(query);
            if (fullName != null)
            {
                GeoCodingResponse cacheResult = new GeoCodingResponse(ResponseResult.OK)
                {
                    Coords = db.GetLocationCoordsFromCache(fullName)!,
                    DisplayName = fullName
                };

                Trace.WriteLine($"NominatimService - CACHE HIT! - Lat:{cacheResult.Coords.Latitude}, Lon:{cacheResult.Coords.Longitude}");
                return cacheResult;
            }

            DateTime checkTime = DateTime.Now;
            if((checkTime - lastRequestTime).TotalSeconds < REQUEST_COOLDOWN)
            {
                TimeSpan timeDiff = checkTime - lastRequestTime;
                await Task.Delay((int)(timeDiff.TotalMilliseconds));
            }

            lastRequestTime = DateTime.Now;

            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(APISearchURL),
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Job Tracker");

            GeoCodingResponse result = new GeoCodingResponse(ResponseResult.NoResult);

            try
            {
                using HttpResponseMessage response = httpClient.GetAsync($"?q={query}&format=jsonv2").Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonResponse);
                    List<JSONGeoCoordinates>? geoCoordinates = await JsonSerializer.DeserializeAsync<List<JSONGeoCoordinates>>(new MemoryStream(jsonBytes));

                    if (geoCoordinates == null)
                    {
                        result.Result = ResponseResult.ServerError;
                        Trace.WriteLine($"NominatimService - CACHE MISS! - Server Error");
                    }
                    else if (geoCoordinates.Count <= 0)
                    {
                        result.Result = ResponseResult.NoResult;
                        Trace.WriteLine($"NominatimService - CACHE MISS! - No Result");
                    }
                    else
                    {
                        JSONGeoCoordinates coords = geoCoordinates[0];
                        result.Coords.Longitude = coords.lon;
                        result.Coords.Latitude = coords.lat;
                        result.DisplayName = coords.display_name;
                        result.Result = ResponseResult.OK;

                        // Update cache with our new data.
                        db.EnsureLocationMappingExists(query, coords.display_name);
                        db.EnsureLocationCoordsExists(coords.display_name, result.Coords);

                        Trace.WriteLine($"NominatimService - CACHE HIT! - Lat:{result.Coords.Latitude}, Lon:{result.Coords.Longitude}");
                    }

                    return result;
                }
                else
                {
                    Trace.WriteLine($"NominatimService - CACHE MISS! - Request Failed: {response.ReasonPhrase}");
                    return new GeoCodingResponse(ResponseResult.ServerError);
                }
            }
            catch (JsonException e)
            {
                Trace.WriteLine($"NominatimService - CACHE MISS! - Invalid JSON from Geocoding Service: {e.Message}");
                result.Result = ResponseResult.JSONError;
            }
            catch (Exception e)
            {
                Trace.WriteLine($"NominatimService - CACHE MISS! - Request Failed: {e.Message}");
                result.Result = ResponseResult.NetworkError;
            }

            return result;
        }
    }
}
