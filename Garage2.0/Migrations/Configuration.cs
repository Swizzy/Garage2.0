using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using Garage2._0.Controllers;
using Garage2._0.DAL;
using Garage2._0.Models;

namespace Garage2._0.Migrations
{
    public sealed class Configuration : DbMigrationsConfiguration<GarageContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        private long FindFirstFreeMcSpot(IEnumerable<Vehicle> vehicles)
        {
            var lastMc = vehicles.OrderBy(v => v.ParkingUnit).LastOrDefault(v => v.Type == Vehicle.VehicleType.Motorcycle);
            if (lastMc != null)
            {
                if (lastMc.ParkingUnit % 3 == 0 || lastMc.ParkingUnit % 3 == 1)
                    return lastMc.ParkingUnit + 1;
            }
            return FindFirstFreeSpot(vehicles);
        }

        private long FindFirstFreeSpot(IEnumerable<Vehicle> vehicles)
        {
            var lastVehicle = vehicles.OrderBy(v => v.ParkingUnit).LastOrDefault();
            if (lastVehicle != null)
            {
                var ret = lastVehicle.ParkingUnit + lastVehicle.Units;
                if (ret % 3 != 0)
                {
                    ret += 3 - ret % 3;
                }
                return ret;
            }
            return 0;
        }

        protected override void Seed(GarageContext context)
        {
            context.Vehicles.RemoveRange(context.Vehicles.ToArray());
            context.Configurations.RemoveRange(context.Configurations.ToArray());
            context.Configurations.Add(new Models.Configuration()
            {
                Id = 1,
                IsConfigured = true,
                ParkingSpaces = 100,
                PricePerMinute = 1
            });
            var list = new List<Vehicle>(100);
            var rand = new Random();
            for (var i = 0; i < list.Capacity; i++)
            {
                var vehicleType = (Vehicle.VehicleType) rand.Next(minValue: 0, maxValue: 5);
                var vehicleColor = (Vehicle.VehicleColor) rand.Next(minValue: 0, maxValue: 6);
                var vehicle = new Vehicle
                {
                    Color = vehicleColor,
                    Type = vehicleType,
                    Timestamp = DateTime.Now - TimeSpan.FromMinutes(rand.Next(0, 1440)),
                    CheckoutTime = DateTime.Now,
                    RegNumber =
                        $"{vehicleColor} {vehicleType} #{list.Count(v => v.Type == vehicleType && v.Color == vehicleColor) + 1}"
                };
                vehicle.ParkingUnit = vehicle.Type == Vehicle.VehicleType.Motorcycle ? FindFirstFreeMcSpot(list) : FindFirstFreeSpot(list);
                if (list.Sum(v => v.Units) + vehicle.Units < 300) {
                    list.Add(vehicle);
                }
            }
            context.Vehicles.AddRange(list);
        }
    }
}
