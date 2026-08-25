using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MainApp.Models
{
    public struct JSONGeoCoordinates
    {
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public double lat { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public double lon { get; set; }
    }
}
