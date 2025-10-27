using Spectre.Console;
using PragueParking.App.Services;

namespace PragueParking.App.Ui
{
    public class Menu
    {
        private readonly ParkingService _svc;
        private readonly PricingService _pricing;
        public Menu(ParkingService svc, PricingService pricing) { _svc = svc; _pricing = pricing; }

        public void Run()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("[bold]Prague Parking 2.1[/] - Choose action:")
                    .AddChoices("Park vehicle", "Find vehicle", "Move vehicle", "Check-out vehicle", "Show map", "Reload config/prices", "Save & Exit"));

                try
                {
                    switch (choice)
                    {
                        case "Park vehicle": ParkVehicle(); break;
                        case "Find vehicle": FindVehicle(); break;
                        case "Move vehicle": MoveVehicle(); break;
                        case "Check-out vehicle": CheckoutVehicle(); break;
                        case "Show map": MapRenderer.Render(_svc.Garage); break;
                        case "Reload config/prices": ReloadConfig(); break;
                        case "Save & Exit": _svc.SaveData(); return;
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Error:[/] {Markup.Escape(ex.Message)}");
                }
            }
        }

        private void ParkVehicle()
        {
            var type = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Vehicle type:")
                .AddChoices(_svc.Config.VehicleSizes.Keys.OrderBy(x => x)));
            var reg = AnsiConsole.Ask<string>("Registration number:").Trim().ToUpperInvariant();

            // Check if vehicle with this registration number already exists
            if (_svc.Garage.TryFindVehicle(reg, out _, out _))
            {
                AnsiConsole.MarkupLine($"[red]Error:[/] A vehicle with registration number {reg} is already parked!");
                return;
            }

            var vehicle = _svc.CreateVehicle(type, reg);
            if (_svc.Garage.Park(vehicle, out var spots))
            {
                _svc.SaveData();
                AnsiConsole.MarkupLine($"[green]Parked[/] {type} {reg} at spots: {string.Join(",", spots)}");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]No suitable space available.[/]");
            }
        }

        private void FindVehicle()
        {
            var reg = AnsiConsole.Ask<string>("Registration number:").Trim();
            if (_svc.Garage.TryFindVehicle(reg, out var primary, out var indices))
            {
                AnsiConsole.MarkupLine($"[green]Found[/] at spot(s): {string.Join(",", indices)} (primary: {primary})");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Not found[/]");
            }
        }

        private void MoveVehicle()
        {
            var reg = AnsiConsole.Ask<string>("Registration number:");
            var target = AnsiConsole.Ask<int>("Target spot index:");
            if (_svc.Garage.Move(reg, target))
            {
                _svc.SaveData();
                AnsiConsole.MarkupLine("[green]Moved successfully[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Move failed (bus or no space).[/]");
            }
        }

        private void CheckoutVehicle()
        {
            var reg = AnsiConsole.Ask<string>("Registration number:");
            if (_svc.Garage.Remove(reg, out var type, out var checkIn, out var indices))
            {
                var cost = _pricing.CalculatePrice(type, checkIn, DateTime.UtcNow);
                _svc.SaveData();
                AnsiConsole.MarkupLine($"Checked out {type} {reg}. Spots freed: {string.Join(",", indices)}. Price: [bold]{cost} CZK[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Not found[/]");
            }
        }

        private void ReloadConfig()
        {
            _svc.ReloadConfigFromDisk();
            _pricing.Update(_svc.Config);
            AnsiConsole.MarkupLine("[green]Configuration & prices reloaded[/]");
        }
    }
}
