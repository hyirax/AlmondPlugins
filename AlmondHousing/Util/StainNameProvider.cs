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
                    if (!string.IsNullOrEmpty(name) && !dict.ContainsKey(row.RowId))
                        dict[row.RowId] = name;
                }
            }

            Load(_namesEn, Dalamud.Game.ClientLanguage.English);
            Load(_namesJa, Dalamud.Game.ClientLanguage.Japanese);
            Load(_namesDe, Dalamud.Game.ClientLanguage.German);
            Load(_namesFr, Dalamud.Game.ClientLanguage.French);
        }

        /// <summary>
        /// Returns the stain name in the given language code (zh, en, ja, ko, de, fr, zh_TW).
        /// Falls back to English for unsupported languages.
        /// </summary>
        public static string GetName(uint stainId, string langCode)
        {
            return langCode switch
            {
                "ja" => _namesJa.TryGetValue(stainId, out var j) ? j : GetFallback(stainId),
                "de" => _namesDe.TryGetValue(stainId, out var d) ? d : GetFallback(stainId),
                "fr" => _namesFr.TryGetValue(stainId, out var f) ? f : GetFallback(stainId),
                _ => GetFallback(stainId),
            };
        }

        private static string GetFallback(uint stainId)
        {
            return _namesEn.TryGetValue(stainId, out var e) ? e : "Unknown";
        }
    }
}
