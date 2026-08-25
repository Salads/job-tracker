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

        GeoCoordinate? IGeocodingService.GetLocationCoordinates(string city, string state)
        {
            HttpClient httpClient = new()
            {
                BaseAddress = new Uri(APISearchURL),
            };
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Job Tracker");

            using HttpResponseMessage response = httpClient.GetAsync($"?city={city}&state={state}&format=jsonv2").Result;
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = response.Content.ReadAsStringAsync().Result;

                List<JSONGeoCoordinates>? geoCoordinates = JsonSerializer.Deserialize<List<JSONGeoCoordinates>>(jsonResponse);
                GeoCoordinate? result = new GeoCoordinate();

                if (geoCoordinates != null && geoCoordinates.Count > 0)
                {
                    JSONGeoCoordinates coords = geoCoordinates[0];
                    result.Longitude = coords.lon;
                    result.Latitude = coords.lat;
                    Trace.WriteLine($"Latitude: {coords.lat} Longitude: {coords.lon}\n");
                }
                else
                {
                    result = null;
                    Trace.WriteLine("Geocoding JSON Error");
                }

                return result;
            }
            else
            {
                // TODO(Salads): User Feedback
                Trace.WriteLine($"Request Failed: {response.ReasonPhrase}");
                return null;
            }
        }
    }
}
