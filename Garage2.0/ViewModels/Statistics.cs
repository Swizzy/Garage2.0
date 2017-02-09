using System.Collections.Generic;
using Garage2._0.Models;

namespace Garage2._0.ViewModels
{
    public class VehicleTypeStatistics
    {
        public Dictionary<Vehicle.VehicleType, int> Statistics = new Dictionary<Vehicle.VehicleType, int>();
    }

    public class VehicleColorStatistics
    {
        public Dictionary<Vehicle.VehicleColor, int> Statistics = new Dictionary<Vehicle.VehicleColor, int>();
    }

    public class VehicleTypeColorStatistics
    {
        public Dictionary<Vehicle.VehicleType, Dictionary<Vehicle.VehicleColor, int>> Statistics =
            new Dictionary<Vehicle.VehicleType, Dictionary<Vehicle.VehicleColor, int>>();
    }
}
