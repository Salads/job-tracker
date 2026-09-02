using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;

namespace MainApp.Services
{
    internal class HaversineDistanceCalculator : IDistanceCalculatorService
    {
        public float GetDistanceBetween(GeoCoordinate a, GeoCoordinate b)
        {
            float metersToMiles = 1609.344f;
            float fDistance = (float)a.GetDistanceTo(b) / metersToMiles;

            return fDistance;
        }
    }
}
