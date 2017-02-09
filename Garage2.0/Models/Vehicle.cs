using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Garage2._0.Models
{
    public class Vehicle
    {
        public enum VehicleType { Airplane, Boat, Bus, Car, Motorcycle};
        public enum VehicleColor { Black, Blue, Brown, Green, Red, White};

        public int Id { get; set; }
        [Display(Name = "Vehicle type")]
        [Required]
        public VehicleType Type { get; set; }
        [Display(Name = "Registration")]
        [Required]
        public string RegNumber { get; set; }
        [Display(Name = "Vehicle color")]
        [Required]
        public VehicleColor Color { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        [Display(Name = "Number of wheels")]
        [Required]
        public int NumberOfWheels { get; set; }
        [Display(Name = "Checkin time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        [Required]
        public DateTime Timestamp { get; set; }

    }
}