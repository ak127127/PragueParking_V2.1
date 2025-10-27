using PragueParking.Domain.Entities;
using PragueParking.Domain.Interfaces;
using PragueParking.Domain.Models;
using PragueParking.Infrastructure.FileSystem;

namespace PragueParking.App.Services
{
    public class ParkingService
    {
        private readonly ConfigProvider _configProvider;
        private readonly FileStorage _storage;
        private readonly PriceListProvider _priceListProvider;

        public Config Config { get; private set; }
        public ParkingGarage Garage { get; private set; }

        public ParkingService(ConfigProvider cp, FileStorage fs, PriceListProvider pp)
        {
            _configProvider = cp; _storage = fs; _priceListProvider = pp;
            Config = cp.Load();
            Garage = new ParkingGarage(Config);
            LoadData();
        }

        public IVehicle CreateVehicle(string type, string regNo, DateTime? checkIn = null)
        {
            return type switch
            {
                "Bike" => new Bike(regNo, checkIn),
                "Motorcycle" => new Motorcycle(regNo, checkIn),
                "Car" => new Car(regNo, checkIn),
                "Bus" => new Bus(regNo, checkIn),
                _ => throw new ArgumentException($"Unsupported vehicle type: {type}")
            };
        }

        public void LoadData()
        {
            var data = _storage.LoadOrCreate(SeedData);
            Garage.ImportData(data, (t, reg, dt) => CreateVehicle(t, reg, dt));
        }

        public void SaveData() => _storage.Save(Garage.ExportData());

        public void ReloadConfigFromDisk()
        {
            var newCfg = _configProvider.Load();
            var occupiedMaxIndex = Garage.Spots.Where(s => s.OccupiedUnits > 0).Select(s => s.Index).DefaultIfEmpty(0).Max();
            if (newCfg.SpotCount < occupiedMaxIndex)
                throw new InvalidOperationException($"Cannot reduce spots below {occupiedMaxIndex} while vehicles are parked.");

            var externalPrices = _priceListProvider.Load();
            foreach (var kv in externalPrices)
                newCfg.PricesPerHour[kv.Key] = kv.Value;

            Config = newCfg;
            var data = Garage.ExportData();
            Garage = new ParkingGarage(Config);
            Garage.ImportData(data, (t, reg, dt) => CreateVehicle(t, reg, dt));
        }

        private ParkingData SeedData()
        {
            var now = DateTime.UtcNow.AddMinutes(-37);
            return new ParkingData
            {
                Vehicles = new List<ParkedVehicleRecord>
                {
                    new() { RegNo = "CAR123", Type = "Car", CheckInUtc = now, SpotIndices = new List<int>{1} },
                    new() { RegNo = "MC777", Type = "Motorcycle", CheckInUtc = now.AddMinutes(-15), SpotIndices = new List<int>{2} },
                    new() { RegNo = "BIK9", Type = "Bike", CheckInUtc = now.AddMinutes(-5), SpotIndices = new List<int>{2} }
                }
            };
        }
    }
}
