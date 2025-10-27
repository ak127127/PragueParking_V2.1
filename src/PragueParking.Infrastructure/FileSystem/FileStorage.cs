using System.Text.Json;
using PragueParking.Domain.Models;

namespace PragueParking.Infrastructure.FileSystem
{
    public class FileStorage
    {
        private readonly string _dataPath;
        public FileStorage(string dataPath) => _dataPath = dataPath;

        public ParkingData LoadOrCreate(Func<ParkingData> seed)
        {
            if (!File.Exists(_dataPath))
            {
                var data = seed();
                Save(data);
                return data;
            }
            var json = File.ReadAllText(_dataPath);
            return JsonSerializer.Deserialize<ParkingData>(json) ?? new ParkingData();
        }

        public void Save(ParkingData data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions{ WriteIndented = true });
            Directory.CreateDirectory(Path.GetDirectoryName(_dataPath)!);
            File.WriteAllText(_dataPath, json);
        }
    }
}
