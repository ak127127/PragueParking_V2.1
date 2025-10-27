namespace PragueParking.Domain.Models
{
    public class Config
    {
        public int SpotCount { get; set; } = 100;
        public int BusAllowedMaxIndex { get; set; } = 50; // spots 1..50
        public int FreeMinutes { get; set; } = 10;
        public Dictionary<string, int> VehicleSizes { get; set; } = new()
        {
            ["Bike"] = 1,
            ["Motorcycle"] = 2,
            ["Car"] = 4,
            ["Bus"] = 16
        };
        public Dictionary<string, decimal> PricesPerHour { get; set; } = new()
        {
            ["Bike"] = 5m,
            ["Motorcycle"] = 10m,
            ["Car"] = 20m,
            ["Bus"] = 80m
        };
    }
}
