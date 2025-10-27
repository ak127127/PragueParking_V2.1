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
            var billable = Math.Max(0, totalMinutes - _cfg.FreeMinutes);
            if (!_cfg.PricesPerHour.TryGetValue(type, out var rate)) rate = 0m;
            if (billable == 0) return 0m;
            var hours = (int)Math.Ceiling(billable / 60.0);
            return rate * hours;
        }
    }
}
