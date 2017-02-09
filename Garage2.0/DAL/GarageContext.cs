using System.Data.Entity;
using System.Linq;
using Garage2._0.Models;

namespace Garage2._0.DAL
{
    public class GarageContext : DbContext
    {
        public GarageContext() : base("Garage2.0")
        {
        }

        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Configuration> Configurations { get; set; }

        public Configuration GarageConfiguration
        {
            get { return Configurations.FirstOrDefault() ?? new Configuration(); }
        }
    }
}
