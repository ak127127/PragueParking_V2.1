namespace PragueParking.Domain.Interfaces
{
    public interface IVehicle
    {
        string RegNo { get; }
        string Type { get; }
        int SizeUnits { get; }
        DateTime CheckInUtc { get; }
    }
}
