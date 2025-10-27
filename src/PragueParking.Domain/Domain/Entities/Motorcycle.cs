namespace PragueParking.Domain.Entities
{
    public class Motorcycle : Vehicle
    {
        public override string Type => "Motorcycle";
        public override int SizeUnits => 2;
        public Motorcycle(string regNo, DateTime? checkInUtc = null) : base(regNo, checkInUtc) {}
    }
}
