using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Garage2._0.DAL;
using Garage2._0.Models;

namespace Garage2._0.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<VehiclesContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(VehiclesContext context)
        {
            context.Vehicles.RemoveRange(context.Vehicles.ToArray());
            var list = new List<Vehicle>(100);
            var rand = new Random();
            for (var i = 0; i < list.Capacity; i++)
            {
                var vehicleType = (Vehicle.VehicleType) rand.Next(minValue: 0, maxValue: 4);
                var vehicleColor = (Vehicle.VehicleColor) rand.Next(minValue: 0, maxValue: 5);
                list.Add(new Vehicle()
                {
                    Color = vehicleColor,
                    Type = vehicleType,
                    Timestamp = DateTime.Now - TimeSpan.FromMinutes(rand.Next(0, 1440)),
                    RegNumber = $"{vehicleColor} {vehicleType} #{list.Count(v => v.Type == vehicleType && v.Color == vehicleColor) + 1}"
                });
            }
            context.Vehicles.AddRange(list);
        }
    }
}
