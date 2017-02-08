using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Garage2._0.DAL
{
    public class VehiclesContext : DbContext
    {
        public DbSet<Models.Vehicle> Vehicles { get; set; }
        public VehiclesContext() : base("DefaultConnection") { }

    }
}