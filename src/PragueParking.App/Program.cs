using System.Text.Json;
using PragueParking.App.Services;
using PragueParking.App.Ui;
using PragueParking.Infrastructure.FileSystem;
using Spectre.Console;

var settingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.paths.json");
var paths = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(settingsPath))!;

string cfgPath = Path.GetFullPath(paths["ConfigFile"]);
string dataPath = Path.GetFullPath(paths["DataFile"]);
string pricePath = Path.GetFullPath(paths["PriceListFile"]);

Directory.CreateDirectory(Path.GetDirectoryName(cfgPath)!);
Directory.CreateDirectory(Path.GetDirectoryName(dataPath)!);

var cfgProvider = new ConfigProvider(cfgPath);
var storage = new FileStorage(dataPath);
var priceProvider = new PriceListProvider(pricePath);

var service = new ParkingService(cfgProvider, storage, priceProvider);
var pricing = new PricingService(service.Config);

AnsiConsole.MarkupLine("[bold]Prague Parking 2.1[/] - Console UI (Spectre.Console)");

var menu = new Menu(service, pricing);
menu.Run();
