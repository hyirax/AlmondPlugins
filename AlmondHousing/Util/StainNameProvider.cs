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

        // Chinese stain names from CafeMaker (cafemaker.wakingsands.com)
        private static readonly Dictionary<uint, string> _namesZh = new()
        {
            {1, "素雪白"},
            {2, "苍白灰"},
            {3, "古菩灰"},
            {4, "石板灰"},
            {5, "木炭灰"},
            {6, "煤烟黑"},
            {7, "玫瑰粉"},
            {8, "丁香紫"},
            {9, "罗兰莓"},
            {10, "卫月红"},
            {11, "铁锈红"},
            {12, "果酒红"},
            {13, "珊瑚粉"},
            {14, "鲜血红"},
            {15, "鲑鱼粉"},
            {16, "日落橙"},
            {17, "台地红"},
            {18, "树皮棕"},
            {19, "巧克力"},
            {20, "铁锈棕"},
            {21, "钴铁棕"},
            {22, "软木棕"},
            {23, "卢恩棕"},
            {24, "奥猴棕"},
            {25, "山羊棕"},
            {26, "南瓜橙"},
            {27, "橡果棕"},
            {28, "果园棕"},
            {29, "山栗棕"},
            {30, "哥布林"},
            {31, "页岩棕"},
            {32, "鼹鼠棕"},
            {33, "沃土棕"},
            {34, "骸骨白"},
            {35, "黄沙棕"},
            {36, "陆行鸟黄"},
            {37, "蜂蜜黄"},
            {38, "玉米黄"},
            {39, "猛豹黄"},
            {40, "奶油黄"},
            {41, "日影黄"},
            {42, "萄干棕"},
            {43, "泥沼绿"},
            {44, "妖精绿"},
            {45, "青柠绿"},
            {46, "苔藓绿"},
            {47, "牧草绿"},
            {48, "橄榄绿"},
            {49, "沼泽绿"},
            {50, "苹果绿"},
            {51, "仙人掌"},
            {52, "猎人绿"},
            {53, "口花绿"},
            {54, "金龟绿"},
            {55, "地神绿"},
            {56, "深林绿"},
            {57, "天上蓝"},
            {58, "绿松蓝"},
            {59, "魔花绿"},
            {60, "寒冰蓝"},
            {61, "天空蓝"},
            {62, "海雾蓝"},
            {63, "孔雀蓝"},
            {64, "罗海蓝"},
            {65, "腐尸蓝"},
            {66, "青磷蓝"},
            {67, "靛青蓝"},
            {68, "油墨蓝"},
            {69, "盗龙蓝"},
            {70, "东洲蓝"},
            {71, "风暴蓝"},
            {72, "虚空蓝"},
            {73, "皇室蓝"},
            {74, "午夜蓝"},
            {75, "阴影蓝"},
            {76, "深渊蓝"},
            {77, "薰衣草"},
            {78, "忧郁紫"},
            {79, "醋栗紫"},
            {80, "鸢尾紫"},
            {81, "葡萄紫"},
            {82, "莲花粉"},
            {83, "蜂鸟粉"},
            {84, "仙子梅"},
            {85, "帝王紫"},
            {86, "宝石红"},
            {87, "樱桃粉"},
            {88, "丝雀黄"},
            {89, "香草黄"},
            {90, "龙骑蓝"},
            {91, "松石蓝"},
            {92, "炮铜黑"},
            {93, "珍珠白"},
            {94, "金属铜"},
            {95, "胭脂红"},
            {96, "霓虹粉"},
            {97, "明亮橙"},
            {98, "霓虹黄"},
            {99, "霓虹绿"},
            {100, "碧空蓝"},
            {101, "无瑕白"},
            {102, "煤玉黑"},
            {103, "柔彩粉"},
            {104, "黑暗红"},
            {105, "黑暗棕"},
            {106, "柔彩绿"},
            {107, "黑暗绿"},
            {108, "柔彩蓝"},
            {109, "黑暗蓝"},
            {110, "柔彩紫"},
            {111, "黑暗紫"},
            {112, "闪耀银"},
            {113, "闪耀金"},
            {114, "金属红"},
            {115, "金属橙"},
            {116, "金属黄"},
            {117, "金属绿"},
            {118, "金属蓝"},
            {119, "金属靛"},
            {120, "金属紫"},
            {121, "罗兰紫"},
            {122, "金属粉"},
            {123, "金属宝石红"},
            {124, "金属钴铁绿"},
            {125, "金属黑暗蓝"}
        };

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
                "zh"     => _namesZh.TryGetValue(stainId, out var z) ? z : _namesEn.GetValueOrDefault(stainId, "???"),
                "zh_TW"  => _namesZh.TryGetValue(stainId, out var t) ? t : _namesEn.GetValueOrDefault(stainId, "???"),
                "ko"     => _namesJa.TryGetValue(stainId, out var k) ? k : _namesEn.GetValueOrDefault(stainId, "???"),
                "ja"     => _namesJa.TryGetValue(stainId, out var j) ? j : _namesEn.GetValueOrDefault(stainId, "???"),
                "de"     => _namesDe.TryGetValue(stainId, out var d) ? d : _namesEn.GetValueOrDefault(stainId, "???"),
                "fr"     => _namesFr.TryGetValue(stainId, out var f) ? f : _namesEn.GetValueOrDefault(stainId, "???"),
                _        => _namesEn.TryGetValue(stainId, out var e) ? e : "???",
            };
        }
    }
}
