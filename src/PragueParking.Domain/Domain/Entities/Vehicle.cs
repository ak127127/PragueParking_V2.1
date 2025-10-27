using PragueParking.Domain.Interfaces;

namespace PragueParking.Domain.Entities
{
    public abstract class Vehicle : IVehicle
    {
        public string RegNo { get; protected set; }
        public abstract string Type { get; }
        public abstract int SizeUnits { get; }
        public DateTime CheckInUtc { get; protected set; }

        protected Vehicle(string regNo, DateTime? checkInUtc = null)
        {
            RegNo = regNo.ToUpperInvariant();
            CheckInUtc = checkInUtc ?? DateTime.UtcNow;
        }
    }
}
