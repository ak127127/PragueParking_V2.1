namespace PragueParking.Domain.Models
{
    public class ParkedVehicleRecord
    {
        public string RegNo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime CheckInUtc { get; set; }
        public List<int> SpotIndices { get; set; } = new();
    }

    public class ParkingData
    {
        public List<ParkedVehicleRecord> Vehicles { get; set; } = new();
    }
}
