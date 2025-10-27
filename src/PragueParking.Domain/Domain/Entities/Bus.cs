namespace PragueParking.Domain.Entities
{
    public class Bus : Vehicle
    {
        public override string Type => "Bus";
        public override int SizeUnits => 16; // requires 4 full spots
        public Bus(string regNo, DateTime? checkInUtc = null) : base(regNo, checkInUtc) {}
    }
}
