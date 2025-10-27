using Microsoft.VisualStudio.TestTools.UnitTesting;
using PragueParking.Domain.Entities;
using PragueParking.Domain.Models;

namespace PragueParking.Tests
{
    [TestClass]
    public class AllocationTests
    {
        [TestMethod]
        public void Bus_Requires_Four_Contiguous_Spots_Within_First_50()
        {
            var cfg = new Config { SpotCount = 60, BusAllowedMaxIndex = 50 };
            var garage = new ParkingGarage(cfg);
            var bus = new Bus("BUS1");
            var ok = garage.Park(bus, out var spots);
            Assert.IsTrue(ok);
            Assert.AreEqual(4, spots.Count);
            Assert.IsTrue(spots.All(i => i <= 50));
        }
    }
}
