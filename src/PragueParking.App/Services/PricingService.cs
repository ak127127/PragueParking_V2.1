using PragueParking.Domain.Models;

namespace PragueParking.App.Services
{
    public class PricingService
    {
        private Config _cfg;
        public PricingService(Config cfg) => _cfg = cfg;
        public void Update(Config cfg) => _cfg = cfg;

        public decimal CalculatePrice(string type, DateTime checkInUtc, DateTime nowUtc)
        {
            var totalMinutes = (int)Math.Max(0, (nowUtc - checkInUtc).TotalMinutes);
            
            // First 10 minutes are free
            if (totalMinutes <= _cfg.FreeMinutes)
                return 0m;
            
            if (!_cfg.PricesPerHour.TryGetValue(type, out var rate)) 
                rate = 0m;
            
            // Calculate billable hours from minute 11 onwards
            var billableMinutes = totalMinutes - _cfg.FreeMinutes;
            var hours = (int)Math.Ceiling(billableMinutes / 60.0);
            return rate * hours;
        }
    }
}
