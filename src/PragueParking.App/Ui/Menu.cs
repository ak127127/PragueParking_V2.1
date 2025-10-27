using Spectre.Console;
using PragueParking.App.Services;
using System.Text.RegularExpressions;

namespace PragueParking.App.Ui
{
    public class Menu
    {
        private readonly ParkingService _svc;
        private readonly PricingService _pricing;
        public Menu(ParkingService svc, PricingService pricing) { _svc = svc; _pricing = pricing; }

        private static bool IsValidSwedishRegNo(string regNo)
        {
            // Swedish format: 3 letters + 3 digits (e.g., ABC123)
            return Regex.IsMatch(regNo, @"^[A-Z]{3}\d{3}$");
        }

        private static string CleanRegistrationNumber(string input)
        {
            // Remove all characters except letters and digits
            var cleaned = Regex.Replace(input.ToUpperInvariant(), @"[^A-Z0-9]", "");
            
            // Extract letters and digits separately
            var letters = Regex.Match(cleaned, @"[A-Z]+").Value;
            var digits = Regex.Match(cleaned, @"\d+").Value;
            
            // Take first 3 letters and first 3 digits
            var letterPart = letters.Length >= 3 ? letters.Substring(0, 3) : letters;
            var digitPart = digits.Length >= 3 ? digits.Substring(0, 3) : digits;
            
            return letterPart + digitPart;
        }

        private string AskForRegistrationNumber(string prompt = "Registration number:", bool requireValidFormat = true)
        {
            while (true)
            {
                var input = AnsiConsole.Ask<string>(prompt).Trim();
                
                if (!requireValidFormat)
                {
                    // For searching/removing existing vehicles, just clean up and return
                    return input.ToUpperInvariant();
                }
                
                var reg = CleanRegistrationNumber(input);
                
                if (IsValidSwedishRegNo(reg))
                {
                    if (reg != input.ToUpperInvariant())
                    {
                        AnsiConsole.MarkupLine($"[yellow]Cleaned to:[/] {reg}");
                    }
                    return reg;
                }
                AnsiConsole.MarkupLine("[red]Invalid format![/] Must contain at least 3 letters and 3 digits (e.g., ABC123)");
            }
        }

        public void Run()
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("[bold]Prague Parking 2.1[/] - Choose action:")
                    .AddChoices("Park vehicle", "Find vehicle", "Move vehicle", "Check-out vehicle", "Show map", "Show prices", "Reload config/prices", "Save & Exit"));

                try
                {
                    switch (choice)
                    {
                        case "Park vehicle": ParkVehicle(); break;
                        case "Find vehicle": FindVehicle(); break;
                        case "Move vehicle": MoveVehicle(); break;
                        case "Check-out vehicle": CheckoutVehicle(); break;
                        case "Show map": MapRenderer.Render(_svc.Garage); break;
                        case "Show prices": ShowPrices(); break;
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
            var typeChoices = _svc.Config.VehicleSizes.Keys.OrderBy(x => x).ToList();
            typeChoices.Add("← Back");
            
            var type = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Vehicle type:")
                .AddChoices(typeChoices));
            
            if (type == "← Back") return;
            
            var reg = AskForRegistrationNumber();

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
            var action = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Find vehicle - Continue?")
                .AddChoices("Continue", "← Back"));
            
            if (action == "← Back") return;
            
            var reg = AskForRegistrationNumber(requireValidFormat: false);
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
            var action = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Move vehicle - Continue?")
                .AddChoices("Continue", "← Back"));
            
            if (action == "← Back") return;
            
            var reg = AskForRegistrationNumber(requireValidFormat: false);
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
            var action = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Check-out vehicle - Continue?")
                .AddChoices("Continue", "← Back"));
            
            if (action == "← Back") return;
            
            var reg = AskForRegistrationNumber(requireValidFormat: false);
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

        private void ShowPrices()
        {
            var table = new Table().Centered().Border(TableBorder.Rounded);
            table.AddColumn("[bold]Vehicle Type[/]");
            table.AddColumn("[bold]Price per Hour[/]");
            table.AddColumn("[bold]Vehicle Size[/]");

            foreach (var vehicleType in _svc.Config.VehicleSizes.Keys.OrderBy(x => x))
            {
                var price = _svc.Config.PricesPerHour.TryGetValue(vehicleType, out var p) ? p : 0m;
                var size = _svc.Config.VehicleSizes[vehicleType];
                table.AddRow(vehicleType, $"[green]{price} CZK[/]", $"{size} units");
            }

            AnsiConsole.Write(table);
            AnsiConsole.MarkupLine($"\n[yellow]Info:[/] First {_svc.Config.FreeMinutes} minutes are free");
            AnsiConsole.MarkupLine("[yellow]Info:[/] Each started hour is charged");
        }
    }
}
