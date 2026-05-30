using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;

namespace AlmondHousing.Util
{
    struct Location
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float rotation { get; set; }
        public string size { get; set; } = "";
        public string entranceLayout { get; set; } = "";

        public Location(float _x, float _y, float _z, float rot, string _size, string layout)
        {
            x = _x; y = _y; z = _z;
            rotation = rot;
            size = _size;
            entranceLayout = layout;
        }

        public Vector3 ToVector() => new Vector3(x, y, z);
    }

    class Plots
    {
        private static Dictionary<string, Dictionary<int, Location>> _map;

        public static Dictionary<string, Dictionary<int, Location>> Map
        {
            get
            {
                if (_map == null)
                {
                    var path = Path.Combine(AlmondHousing.PluginDirectory, "Data", "plots.json");
                    if (File.Exists(path))
                    {
                        var json = File.ReadAllText(path);
                        var opts = new JsonSerializerOptions { IncludeFields = true };
                        _map = JsonSerializer.Deserialize<Dictionary<string, Dictionary<int, Location>>>(json, opts)
                               ?? new Dictionary<string, Dictionary<int, Location>>();
                    }
                    else
                    {
                        DalamudApi.PluginLog.Error("plots.json not found!");
                        _map = new Dictionary<string, Dictionary<int, Location>>();
                    }
                }
                return _map;
            }
        }
    }
}
