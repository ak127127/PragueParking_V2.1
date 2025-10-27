using System.Text.Json;
using PragueParking.Domain.Models;

namespace PragueParking.Infrastructure.FileSystem
{
    public class ConfigProvider
    {
        private readonly string _configPath;
        public ConfigProvider(string configPath) => _configPath = configPath;

        public Config Load()
        {
            if (!File.Exists(_configPath))
            {
                var cfg = new Config();
                var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions{ WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(_configPath)!);
                File.WriteAllText(_configPath, json);
                return cfg;
            }
            var content = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<Config>(content) ?? new Config();
        }

        public void Save(Config cfg)
        {
            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions{ WriteIndented = true });
            File.WriteAllText(_configPath, json);
        }
    }
}
