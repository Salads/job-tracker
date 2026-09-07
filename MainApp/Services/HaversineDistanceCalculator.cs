using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;

namespace MainApp.Services
{
    internal class HaversineDistanceCalculator : IDistanceCalculatorService
    {
        public float GetDistanceBetween(GeoCodingResponse a, GeoCodingResponse b)
        {
            if (a.IsRemote || b.IsRemote)
            {
                return 0;
            }
            else
            {
                if (a.Result == ResponseResult.OK && b.Result == ResponseResult.OK)
                {
                    float metersToMiles = 1609.344f;
                    float fDistance = (float)a.Coords.GetDistanceTo(b.Coords) / metersToMiles;

                    return fDistance;
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
