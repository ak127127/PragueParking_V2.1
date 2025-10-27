using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.App.Services;
using PragueParking.Domain.Models;

namespace PragueParking.Tests
{
    [TestClass]
    public class PricingTests
    {
        [TestMethod]
        public void Price_Respects_FreeMinutes_And_RoundsUp_PerHour()
        {
            var cfg = new Config { FreeMinutes = 10, PricesPerHour = new() { ["Car"] = 20m } };
            var svc = new PricingService(cfg);
            var start = DateTime.UtcNow.AddMinutes(-65);
            var price = svc.CalculatePrice("Car", start, DateTime.UtcNow);
            Assert.AreEqual(20m, price);
        }
    }
}
