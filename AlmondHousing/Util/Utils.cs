using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using AlmondHousing.Objects;

namespace AlmondHousing
{
    public static class Utils
    {
        public static string GetExteriorPartDescriptor(ExteriorPartsType partsType)
        {
            return partsType switch
            {
                ExteriorPartsType.Roof => AlmondHousing.Lang.GetText("Roof"),
                ExteriorPartsType.Walls => AlmondHousing.Lang.GetText("Exterior Wall"),
                ExteriorPartsType.Windows => AlmondHousing.Lang.GetText("Window"),
                ExteriorPartsType.Door => AlmondHousing.Lang.GetText("Door"),
                ExteriorPartsType.RoofOpt => AlmondHousing.Lang.GetText("Roof Decor"),
                ExteriorPartsType.WallOpt => AlmondHousing.Lang.GetText("Exterior Wall Decor"),
                ExteriorPartsType.SignOpt => AlmondHousing.Lang.GetText("Placard"),
                ExteriorPartsType.Fence => AlmondHousing.Lang.GetText("Fence"),
                _ => AlmondHousing.Lang.GetText("Unknown")
            };
        }

        public static string GetInteriorPartDescriptor(InteriorPartsType partsType)
        {
            return partsType switch
            {
                InteriorPartsType.Walls => AlmondHousing.Lang.GetText("Wall"),
                InteriorPartsType.Windows => AlmondHousing.Lang.GetText("Window"),
                InteriorPartsType.Door => AlmondHousing.Lang.GetText("Door"),
                InteriorPartsType.Floor => AlmondHousing.Lang.GetText("Floor"),
                InteriorPartsType.Light => AlmondHousing.Lang.GetText("Light"),
                _ => AlmondHousing.Lang.GetText("Unknown")
            };
        }

        public static string GetFloorDescriptor(InteriorFloor floor)
        {
            return floor switch
            {
                InteriorFloor.Ground => AlmondHousing.Lang.GetText("Ground Floor"),
                InteriorFloor.Basement => AlmondHousing.Lang.GetText("Basement"),
                InteriorFloor.Upstairs => AlmondHousing.Lang.GetText("Upper Floor"),
                InteriorFloor.External => AlmondHousing.Lang.GetText("Main"),
                _ => AlmondHousing.Lang.GetText("Unknown")
            };
        }

        public static float DistanceFromPlayer(HousingGameObject obj, Vector3 playerPos)
        {
            return Distance(new Vector3(playerPos.X, playerPos.Y, playerPos.Z), new Vector3(obj.X, obj.Y, obj.Z));
        }

        public static double FastDistance(Vector3 v1, Vector3 v2)
        {
            var x1 = Math.Pow(v2.X - v1.X, 2);
            var y1 = Math.Pow(v2.Y - v1.Y, 2);
            var z1 = Math.Pow(v2.Z - v1.Z, 2);
            return x1 + y1 + z1;
        }

        public static float Distance(Vector3 v1, Vector3 v2)
        {
            return (float)Math.Sqrt(FastDistance(v1, v2));
        }

        public static void StainButton(string id, Stain color, Vector2 size)
        {
            var floatColor = StainToVector4(color.Color);
            ImGui.ColorButton("##" + id, floatColor, ImGuiColorEditFlags.NoTooltip, size);
        }

        public static Vector4 StainToVector4(uint stainColor)
        {
            var s = 1.0f / 255.0f;
            return new Vector4()
            {
                X = ((stainColor >> 16) & 0xFF) * s,
                Y = ((stainColor >> 8) & 0xFF) * s,
                Z = ((stainColor >> 0) & 0xFF) * s,
                W = ((stainColor >> 24) & 0xFF) * s
            };
        }

        public static HousingItem GetNearestHousingItem(IEnumerable<HousingItem> items, Vector3 position)
        {
            return items
                .OrderBy(item => FastDistance(position, new Vector3(item.X, item.Y, item.Z)))
                .FirstOrDefault();
        }
    }
}
