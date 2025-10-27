using PragueParking.Domain.Interfaces;

namespace PragueParking.Domain.Interfaces
{
    public interface IParkingSpot
    {
        int Index { get; }
        int CapacityUnits { get; }
        int OccupiedUnits { get; }
        bool IsReservedWholeSpot { get; }
        string? ReservedByRegNo { get; }
        IReadOnlyList<IVehicle> Vehicles { get; }

        bool CanFit(IVehicle v);
        void Park(IVehicle v);
        bool Remove(string regNo);
    }
}
