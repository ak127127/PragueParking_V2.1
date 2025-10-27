namespace PragueParking.Domain.Entities
{
    public class Car : Vehicle
    {
        public override string Type => "Car";
        public override int SizeUnits => 4;
        public Car(string regNo, DateTime? checkInUtc = null) : base(regNo, checkInUtc) {}
    }
}
