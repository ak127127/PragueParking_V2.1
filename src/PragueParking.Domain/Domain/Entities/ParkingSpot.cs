using PragueParking.Domain.Interfaces;

namespace PragueParking.Domain.Entities
{
    public class ParkingSpot : IParkingSpot
    {
        private readonly List<IVehicle> _vehicles = new();
        public int Index { get; }
        public int CapacityUnits { get; } = 4;
        public int OccupiedUnits => _vehicles.Sum(v => v.SizeUnits);
        public bool IsReservedWholeSpot { get; private set; }
        public string? ReservedByRegNo { get; private set; }
        public IReadOnlyList<IVehicle> Vehicles => _vehicles;

        public ParkingSpot(int index)
        {
            Index = index;
        }

        public bool CanFit(IVehicle v)
        {
            if (IsReservedWholeSpot && ReservedByRegNo != v.RegNo) return false;
            if (IsReservedWholeSpot && ReservedByRegNo == v.RegNo) return true; // Allow the vehicle that reserved this spot
            return OccupiedUnits + v.SizeUnits <= CapacityUnits;
        }

        public void Park(IVehicle v)
        {
            if (!CanFit(v)) throw new InvalidOperationException($"Spot {Index} cannot fit {v.Type} {v.RegNo}.");
            _vehicles.Add(v);
        }

        public bool Remove(string regNo)
        {
            var i = _vehicles.FindIndex(v => v.RegNo.Equals(regNo, StringComparison.OrdinalIgnoreCase));
            if (i >= 0)
            {
                _vehicles.RemoveAt(i);
                if (IsReservedWholeSpot && !_vehicles.Any())
                {
                    IsReservedWholeSpot = false;
                    ReservedByRegNo = null;
                }
                return true;
            }
            return false;
        }

        public void ReserveWholeSpot(string regNo)
        {
            IsReservedWholeSpot = true;
            ReservedByRegNo = regNo.ToUpperInvariant();
        }

        public void ClearReservation()
        {
            IsReservedWholeSpot = false;
            ReservedByRegNo = null;
        }
    }
}
