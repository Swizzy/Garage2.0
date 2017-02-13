using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        [Display(Name = "Checkout time")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
        public DateTime CheckoutTime { get; set; }
        [Display(Name = "Checkout Cost")]
        [DisplayFormat(DataFormatString = "{0:c}")]
        public decimal Cost { get; set; }
        [NotMapped]
        [Display(Name = "Parking Spot")]
        public string ParkingSpot {
            get {
                switch (Type) {
                    case VehicleType.Airplane:
                    case VehicleType.Boat:
                    case VehicleType.Bus:
                        return $"{ParkingUnit / 3 + 1} & {(ParkingUnit + 3) / 3 + 1}";
                    case VehicleType.Car:
                        return (ParkingUnit / 3 + 1).ToString();
                    case VehicleType.Motorcycle:
                        return $"{ParkingUnit / 3 + 1}.{ParkingUnit % 3 + 1}";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(Type));
                }
            }
        }
        public long ParkingUnit { get; set; }
        [NotMapped]
        public int Units => GetUnitSpace(Type);
        public static int GetUnitSpace(VehicleType type) {
            switch (type)
            {
                case VehicleType.Airplane:
                case VehicleType.Boat:
                case VehicleType.Bus:
                    return 6;
                case VehicleType.Car:
                    return 3;
                case VehicleType.Motorcycle:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Type));
            }
        }
    }
}
