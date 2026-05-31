using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Newtonsoft.Json;

namespace AlmondHousing.Util
{
    public class Location
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float rotation { get; set; }
        public string size { get; set; } = "";
        public string entranceLayout { get; set; } = "";

        public Location() { }

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
        private static Dictionary<string, Dictionary<string, Location>> _map;

        public static Dictionary<string, Dictionary<string, Location>> Map
        {
            get
            {
                if (_map == null)
                {
                    var diskPath = Path.Combine(AlmondHousing.PluginDirectory, "Data", "plots.json");
                    var json = EmbeddedResource.ReadAllText("AlmondHousing.Data.plots.json", diskPath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        _map = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Location>>>(json)
                               ?? new Dictionary<string, Dictionary<string, Location>>();
                    }
                    else
                    {
                        DalamudApi.PluginLog.Error("plots.json not found!");
                        _map = new Dictionary<string, Dictionary<string, Location>>();
                    }
                }
                return _map;
            }
        }
    }
}