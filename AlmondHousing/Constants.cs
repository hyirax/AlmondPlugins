using System.Numerics;

namespace AlmondHousing
{
    public static class Constants
    {
        // 🎨【补全】主题色彩中心大统一
        public static class Colors
        {
            public static readonly Vector4 ThemeBase = new Vector4(0.20f, 0.20f, 0.20f, 1f);     
            public static readonly Vector4 ThemeHover = new Vector4(0.35f, 0.35f, 0.35f, 1f);    
            public static readonly Vector4 ThemeActive = new Vector4(0.45f, 0.45f, 0.45f, 1f);   
            public static readonly Vector4 ThemeHeader = new Vector4(0.15f, 0.15f, 0.15f, 0.8f);
            public static readonly Vector4 AccentGold = new Vector4(0.9f, 0.7f, 0.3f, 1.0f);     // 标志性的杏仁金
        }

        // 🛰️ 游戏底层内存扫描特征码大本营 (Signatures)
        public static class Signatures
        {
            public const string PlaceAnywhere = "C6 83 ?? ?? ?? ?? ?? 0F 29 44 24";
            public const string WallAnywhere = "48 85 C0 74 ?? C6 87 ?? ?? 00 00 00";
            public const string WallmountAnywhere = "c6 87 83 01 00 00 00 48 83 c4 ??";
            public const string ShowcasePlace = "C6 87 ?? ?? 00 00 00 48 8B BC 24 ?? ?? ?? ?? 48";
            public const string ShowcaseRotate = "88 87 ?? ?? 00 00 0F 28 74 24 ?? 48 8B";
            
            public const string HousingModule = "48 8B 05 ?? ?? ?? ?? 8B 52";
            public const string LayoutWorld = "48 8B D1 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 0A";
            public const string GetInventoryContainer = "E8 ?? ?? ?? ?? 40 38 78 18";
            public const string HousingLayoutModelUpdate = "48 89 6C 24 ?? 48 89 74 24 ?? 48 89 7C 24 ?? 41 56 48 83 EC 50 48 8B E9 48 8B 49";

            public const string HookSelectItem = "48 85 D2 0F 84 ?? ?? ?? ?? 53 41 56 48 83 EC ?? 48 89 6C 24";
            public const string HookClickItem = "48 89 5C 24 10 48 89 74  24 18 57 48 83 EC 20 4c 8B 41 18 33 FF 0F B6 F2";
            public const string HookGetGameObject = "E8 ?? ?? ?? ?? EB ?? 48 3D";
            public const string HookGetObjectFromIndex = "E8 ?? ?? ?? ?? EB ?? 41 0F B7 D0";
            public const string HookGetYardIndex = "E8 ?? ?? ?? ?? 44 0F B7 D8";
            public const string HookMaybePlace = "40 55 56 57 48 8D AC 24 70 FF FF FF 48 81 EC 90 01 00 00 48 8B";
            public const string HookResetItemPlacement = "48 89 5C 24 08 57 48 83  EC 20 48 83 79 18 00 0F";
            public const string HookFinalizeHousing = "40 55 56 41 56 48 83 EC 20 48 63 EA 48 8B F1 8B";
            public const string HookPlaceCall = "40 53 48 83 Ec 20 48 8B 51 18 48 8B D9 48 85 D2 0F 84 B1 00 00 00";
        }

        // 🧠 游戏底层结构体魔法偏移量大本营 (Offsets)
        public static class Offsets
        {
            public const int PlaceAnywhereAdjustment = 6;
            public const int WallAnywhereAdjustment = 11;
            public const int WallmountAnywhereAdjustment = 6;
            public const int LayoutWorldInstructionLength = 3;

            public const int ItemActivePosition = 0x50;  
            public const int ItemModelRefresh = 0x80;    
        }

        // 🎨 插件通用基础配置定义
        public static class Housing
        {
            public const float LocationTolerance = 0.0001f;
            public const float RotationTolerance = 0.001f;
            public const int MaxHistorySize = 50; 
            public const float MaxExteriorHeight = 0; 
            public const float LargePlotX = 0; 
            public const float LargePlotZ = 0;
            public const float MediumPlotX = 0;
            public const float MediumPlotZ = 0;
            public const float SmallPlotX = 0;
            public const float SmallPlotZ = 0;
            public const float BasementThreshold = 0; 
            public const float UpperFloorThreshold = 0;
        }
    }
}