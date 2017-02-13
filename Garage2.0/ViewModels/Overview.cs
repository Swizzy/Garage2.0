namespace Garage2._0.ViewModels
{
    public class Overview
    {
        public Overview(int id)
        {
            Id = id;
        }
        public Overview(int id, int vehicleId)
        {
            Id = id;
            VehicleId = vehicleId;
            IsTaken = true;
        }

        public bool IsTaken { get; }
        public int Id { get; }
        public int VehicleId { get; }
    }
}
