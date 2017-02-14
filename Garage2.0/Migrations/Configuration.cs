using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
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

        protected override void Seed(GarageContext context)
        {
            context.Vehicles.RemoveRange(context.Vehicles.ToArray());
            context.Configurations.Add(new Models.Configuration()
            {
                Id = 1,
                IsConfigured = true,
                ParkingSpaces = 100,
                PricePerMinute = 60
            });
            var list = new List<Vehicle>(100);
            var rand = new Random();
            for (var i = 0; i < list.Capacity; i++)
            {
                var vehicleType = (Vehicle.VehicleType) rand.Next(minValue: 0, maxValue: 5);
                var vehicleColor = (Vehicle.VehicleColor) rand.Next(minValue: 0, maxValue: 6);
                list.Add(new Vehicle()
                {
                    Color = vehicleColor,
                    Type = vehicleType,
                    Timestamp = DateTime.Now - TimeSpan.FromMinutes(rand.Next(0, 1440)),
                    CheckoutTime = DateTime.Now,
                    RegNumber = $"{vehicleColor} {vehicleType} #{list.Count(v => v.Type == vehicleType && v.Color == vehicleColor) + 1}"
                });
            }
            context.Vehicles.AddRange(list);
        }
    }
}
