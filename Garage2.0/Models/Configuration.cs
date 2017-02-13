using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Garage2._0.Models
{
    public class Configuration
    {
        public int Id { get; set; }

        [Required]
        [Range(minimum: 1, maximum: int.MaxValue)]
        [Display(Name = "Parking Spaces")]
        public int ParkingSpaces { get; set; }

        [Required]
        [Range(minimum: 0, maximum: int.MaxValue)]
        [Display(Name = "Price Per Minute")]
        public int PricePerMinute { get; set; }

        public bool IsConfigured { get; set; }
        [NotMapped]
        public long MaxUnits => ParkingSpaces * 3;
    }
}
