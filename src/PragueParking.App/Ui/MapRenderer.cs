using Spectre.Console;
using PragueParking.Domain.Entities;

namespace PragueParking.App.Ui
{
    public static class MapRenderer
    {
        public static void Render(ParkingGarage garage)
        {
            var table = new Table().Centered().Border(TableBorder.Rounded);
            table.AddColumn("Spot");
            table.AddColumn("Status");
            table.AddColumn("Vehicles");

            foreach (var s in garage.Spots)
            {
                var status = s.IsReservedWholeSpot ? "[magenta]BUS RESERVED[/]" :
                             s.OccupiedUnits == 0 ? "[green]EMPTY[/]" :
                             s.OccupiedUnits < s.CapacityUnits ? "[yellow]PARTIAL[/]" : "[red]FULL[/]";
                
                string veh;
                if (s.Vehicles.Count > 0)
                {
                    veh = string.Join(", ", s.Vehicles.Select(v => $"{v.Type}:{v.RegNo}"));
                }
                else if (s.IsReservedWholeSpot && !string.IsNullOrEmpty(s.ReservedByRegNo))
                {
                    veh = $"Bus:{s.ReservedByRegNo}";
                }
                else
                {
                    veh = "-";
                }
                
                table.AddRow($"{s.Index}", status, veh);
            }
            AnsiConsole.Write(table);
        }
    }
}
