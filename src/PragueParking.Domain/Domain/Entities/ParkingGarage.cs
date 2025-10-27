using PragueParking.Domain.Interfaces;
using PragueParking.Domain.Models;

namespace PragueParking.Domain.Entities
{
    public class ParkingGarage
    {
        public IReadOnlyList<ParkingSpot> Spots => _spots;
        private readonly List<ParkingSpot> _spots;
        private readonly Config _config;

        private readonly Dictionary<string, List<int>> _multiSpotVehicles = new(StringComparer.OrdinalIgnoreCase);

        public ParkingGarage(Config config)
        {
            _config = config;
            _spots = Enumerable.Range(1, config.SpotCount).Select(i => new ParkingSpot(i)).ToList();
        }

        public bool TryFindVehicle(string regNo, out int primarySpotIndex, out List<int> spotIndices)
        {
            regNo = regNo.ToUpperInvariant();
            if (_multiSpotVehicles.TryGetValue(regNo, out var indices))
            {
                primarySpotIndex = indices[0];
                spotIndices = indices.ToList();
                return true;
            }

            foreach (var s in _spots)
            {
                if (s.Vehicles.Any(v => v.RegNo.Equals(regNo, StringComparison.OrdinalIgnoreCase)))
                {
                    primarySpotIndex = s.Index;
                    spotIndices = new List<int> { s.Index };
                    return true;
                }
            }
            primarySpotIndex = -1; spotIndices = new();
            return false;
        }

        public bool Park(IVehicle v, out List<int> allocated)
        {
            allocated = new();
            if (v.SizeUnits <= 4)
            {
                foreach (var s in _spots)
                {
                    if (s.CanFit(v))
                    {
                        s.Park(v);
                        allocated = new List<int> { s.Index };
                        return true;
                    }
                }
                return false;
            }
            else
            {
                int needSpots = v.SizeUnits / 4;
                for (int i = 0; i <= _config.BusAllowedMaxIndex - needSpots; i++)
                {
                    var segment = _spots.Skip(i).Take(needSpots).ToList();
                    if (segment.Any(s => s.OccupiedUnits > 0 || s.IsReservedWholeSpot))
                        continue;

                    foreach (var s in segment) s.ReserveWholeSpot(v.RegNo);
                    _spots[i].Park(v);

                    var indices = segment.Select(s => s.Index).ToList();
                    _multiSpotVehicles[v.RegNo] = indices;
                    allocated = indices;
                    return true;
                }
                return false;
            }
        }

        public bool Remove(string regNo, out string type, out DateTime checkInUtc, out List<int> spotIndices)
        {
            type = string.Empty; checkInUtc = default; spotIndices = new();
            if (!TryFindVehicle(regNo, out var primary, out var indices)) return false;

            var first = _spots[primary - 1];
            var v = first.Vehicles.First(x => x.RegNo.Equals(regNo, StringComparison.OrdinalIgnoreCase));
            type = v.Type; checkInUtc = v.CheckInUtc; spotIndices = indices;

            foreach (var idx in indices)
            {
                var s = _spots[idx - 1];
                s.Remove(regNo);
                s.ClearReservation();
            }
            _multiSpotVehicles.Remove(regNo);
            return true;
        }

        public bool Move(string regNo, int targetSpotIndex)
        {
            if (!TryFindVehicle(regNo, out var primary, out var indices)) return false;
            if (indices.Count > 1) return false;

            var current = _spots[primary - 1];
            var vehicle = current.Vehicles.First(v => v.RegNo.Equals(regNo, StringComparison.OrdinalIgnoreCase));

            var target = _spots[targetSpotIndex - 1];
            if (!target.CanFit(vehicle)) return false;

            current.Remove(regNo);
            target.Park(vehicle);
            return true;
        }

        public ParkingData ExportData()
        {
            var list = new List<ParkedVehicleRecord>();
            var recorded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var kvp in _multiSpotVehicles)
            {
                var reg = kvp.Key;
                var idxs = kvp.Value;
                var first = _spots[idxs[0] - 1];
                var v = first.Vehicles.First(x => x.RegNo.Equals(reg, StringComparison.OrdinalIgnoreCase));
                list.Add(new ParkedVehicleRecord { RegNo = v.RegNo, Type = v.Type, CheckInUtc = v.CheckInUtc, SpotIndices = idxs.ToList() });
                recorded.Add(reg);
            }

            foreach (var s in _spots)
            {
                foreach (var v in s.Vehicles)
                {
                    if (recorded.Contains(v.RegNo)) continue;
                    list.Add(new ParkedVehicleRecord { RegNo = v.RegNo, Type = v.Type, CheckInUtc = v.CheckInUtc, SpotIndices = new List<int> { s.Index } });
                }
            }
            return new ParkingData { Vehicles = list };
        }

        public void ImportData(ParkingData data, Func<string, string, DateTime, IVehicle> vehicleFactory)
        {
            foreach (var s in _spots)
            {
                foreach (var v in s.Vehicles.ToList()) s.Remove(v.RegNo);
                s.ClearReservation();
            }
            _multiSpotVehicles.Clear();

            foreach (var r in data.Vehicles)
            {
                var v = vehicleFactory(r.Type, r.RegNo, r.CheckInUtc);
                if (r.SpotIndices.Count == 1)
                {
                    var s = _spots[r.SpotIndices[0] - 1];
                    s.Park(v);
                }
                else
                {
                    var seg = r.SpotIndices.Select(i => _spots[i - 1]).ToList();
                    foreach (var s in seg) s.ReserveWholeSpot(r.RegNo);
                    seg[0].Park(v);
                    _multiSpotVehicles[r.RegNo] = r.SpotIndices.ToList();
                }
            }
        }
    }
}
