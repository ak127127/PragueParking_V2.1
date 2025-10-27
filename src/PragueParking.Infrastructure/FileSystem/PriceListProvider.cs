namespace PragueParking.Infrastructure.FileSystem
{
    public class PriceListProvider
    {
        private readonly string _path;
        public PriceListProvider(string path) => _path = path;

        public Dictionary<string, decimal> Load()
        {
            if (!File.Exists(_path)) return new();
            var dict = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            foreach (var raw in File.ReadAllLines(_path))
            {
                var line = raw.Split('#')[0].Trim();
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;
                if (decimal.TryParse(parts[1].Trim(), out var price))
                {
                    dict[parts[0].Trim()] = price;
                }
            }
            return dict;
        }
    }
}
