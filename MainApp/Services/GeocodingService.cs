using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Device.Location;
using static MainApp.Services.IGeocodingService;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Media;

/*
 * TODO(Salads)
 * 
 * Use "Nominatim" free geocoding service (1 request per second)
 * https://nominatim.openstreetmap.org/search?<params>
 * 
 * Then use haversine distance since we don't really care about travelling directions, just "relative distance".
 */

namespace MainApp.Services
{
    public class GeocodingService : IGeocodingService
    {
        const string APISearchURL = "https://nominatim.openstreetmap.org/search";
        const string APIStatusURL = "https://nominatim.openstreetmap.org/status";

        private DateTime lastRequestTime = DateTime.Now;

        public GeoCodingStatus GetGeoCodingStatus()
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(APIStatusURL);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Job Tracker");

            using HttpResponseMessage response = httpClient.GetAsync("?format=text").Result;
            string responseContent = response.Content.ReadAsStringAsync().Result;
            return new GeoCodingStatus(response.IsSuccessStatusCode, responseContent);
        }

        public GeoCodingResponse GetLocationCoordinates(string query)
        {
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
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    List<JSONGeoCoordinates>? geoCoordinates = JsonSerializer.Deserialize<List<JSONGeoCoordinates>>(jsonResponse);

                    if (geoCoordinates != null && geoCoordinates.Count > 0)
                    {
                        JSONGeoCoordinates coords = geoCoordinates[0];
                        result.Coords.Longitude = coords.lon;
                        result.Coords.Latitude = coords.lat;
                        result.DisplayName = coords.display_name;
                        result.Result = ResponseResult.OK;
                        Trace.WriteLine($"Latitude: {coords.lat} Longitude: {coords.lon}\n");
                    }
                    else
                    {
                        result.Result = ResponseResult.JSONError;
                        Trace.WriteLine("Geocoding JSON Error");
                    }

                    return result;
                }
                else
                {
                    Trace.WriteLine($"Request Failed: {response.ReasonPhrase}");
                    return new GeoCodingResponse(ResponseResult.ServerError);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Request Failed: {e.Message}");
                result.Result = ResponseResult.NetworkError;
            }

            return result;
        }
    }
}
