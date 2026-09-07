using MainApp.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;

namespace MainApp.Services
{
    public interface IDistanceCalculatorService
    {
        public float GetDistanceBetween(GeoCodingResponse a, GeoCodingResponse b);
    }
}
