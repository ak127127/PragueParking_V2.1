namespace PragueParking.Domain.Entities
{
    public class Bike : Vehicle
    {
        public override string Type => "Bike";
        public override int SizeUnits => 1;
        public Bike(string regNo, DateTime? checkInUtc = null) : base(regNo, checkInUtc) {}
    }
}
