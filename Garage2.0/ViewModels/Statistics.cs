using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Garage2._0.Models;

namespace Garage2._0.ViewModels
{
    public class Statistics
    {
        public Statistics()
        {
            ColorStatistics = new Dictionary<Vehicle.VehicleColor, int>();
            TypeStatistics = new Dictionary<Vehicle.VehicleType, int>();
            TypeColorStatistics = new Dictionary<Vehicle.VehicleType, Dictionary<Vehicle.VehicleColor, int>>();
        }
        [Display(Name = "Statistics by Vehicle Color")]
        public Dictionary<Vehicle.VehicleColor, int> ColorStatistics { get; set; }
        [Display(Name = "Statistics by Vehicle Type & Color")]
        public Dictionary<Vehicle.VehicleType, Dictionary<Vehicle.VehicleColor, int>> TypeColorStatistics { get; set; }
        [Display(Name = "Statistics by Vehicle Type")]
        public Dictionary<Vehicle.VehicleType, int> TypeStatistics { get; set; }
        [Display(Name = "Total Wheel Count")]
        public int TotalWheels { get; set; }
        [DisplayFormat(DataFormatString = "{0:c}")]
        [Display(Name = "Total Costs")]
        public decimal TotalCost { get; set; }
        [Display(Name = "Total Vehicles")]
        public int TotalVehicles { get; set; }

        public void Update(Vehicle vehicle, DateTime now, decimal pricePerMinute)
        {
            TotalVehicles += 1;
            TotalWheels += vehicle.NumberOfWheels;
            TotalCost += (decimal)Math.Round((now - vehicle.Timestamp).TotalMinutes) * pricePerMinute;

            // Vehicle Color statistics
            if (!ColorStatistics.ContainsKey(vehicle.Color))
                ColorStatistics[vehicle.Color] = 1;
            else
                ColorStatistics[vehicle.Color] += 1;

            // Vehicle Type statistics
            if (!TypeStatistics.ContainsKey(vehicle.Type))
                TypeStatistics[vehicle.Type] = 1;
            else
                TypeStatistics[vehicle.Type] += 1;

            // Vehicle Type & Color statistics
            if (!TypeColorStatistics.ContainsKey(vehicle.Type))
                TypeColorStatistics[vehicle.Type] = new Dictionary<Vehicle.VehicleColor, int>();
            var tcs = TypeColorStatistics[vehicle.Type];
            if (!tcs.ContainsKey(vehicle.Color))
                tcs[vehicle.Color] = 1;
            else
                tcs[vehicle.Color] += 1;
        }
    }
}
