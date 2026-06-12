using System.Collections.Generic;
using Lumina.Excel.Sheets;

namespace AlmondHousing.Util
{
    public static class StainNameProvider
    {
        private static readonly Dictionary<uint, string> _namesEn = new();
        private static readonly Dictionary<uint, string> _namesJa = new();
        private static readonly Dictionary<uint, string> _namesDe = new();
        private static readonly Dictionary<uint, string> _namesFr = new();

        static StainNameProvider()
        {
            void Load(Dictionary<uint, string> dict, Dalamud.Game.ClientLanguage lang)
            {
                var sheet = DalamudApi.DataManager.GetExcelSheet<Stain>(lang);
                if (sheet == null) return;
                foreach (var row in sheet)
                {
                    if (row.RowId == 0) continue;
                    var name = row.Name.ToString();
                    if (!string.IsNullOrEmpty(name))
                        dict[row.RowId] = name;
                }
            }

            Load(_namesEn, Dalamud.Game.ClientLanguage.English);
            Load(_namesJa, Dalamud.Game.ClientLanguage.Japanese);
            Load(_namesDe, Dalamud.Game.ClientLanguage.German);
            Load(_namesFr, Dalamud.Game.ClientLanguage.French);
        }

        public static string GetName(uint stainId, string langCode)
        {
            return langCode switch
            {
                "zh"     => _namesJa.TryGetValue(stainId, out var z) ? z : _namesEn.GetValueOrDefault(stainId, "???"),
                "zh_TW"  => _namesJa.TryGetValue(stainId, out var t) ? t : _namesEn.GetValueOrDefault(stainId, "???"),
                "ko"     => _namesJa.TryGetValue(stainId, out var k) ? k : _namesEn.GetValueOrDefault(stainId, "???"),
                "ja"     => _namesJa.TryGetValue(stainId, out var j) ? j : _namesEn.GetValueOrDefault(stainId, "???"),
                "de"     => _namesDe.TryGetValue(stainId, out var d) ? d : _namesEn.GetValueOrDefault(stainId, "???"),
                "fr"     => _namesFr.TryGetValue(stainId, out var f) ? f : _namesEn.GetValueOrDefault(stainId, "???"),
                _        => _namesEn.TryGetValue(stainId, out var e) ? e : "???",
            };
        }
    }
}
