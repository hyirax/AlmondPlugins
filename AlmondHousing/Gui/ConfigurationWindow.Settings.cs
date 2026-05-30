using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using static AlmondHousing.AlmondHousing;

namespace AlmondHousing.Gui
{
    public partial class ConfigurationWindow
    {
        private void DrawSettingsTab()
        {
            DrawInlineIconColored(FontAwesomeIcon.ExclamationTriangle, new Vector4(1.0f, 0.3f, 0.3f, 1.0f)); ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f)); 
            ImGui.TextWrapped(Lang.GetText("Anti-Resell Warning"));
            ImGui.PopStyleColor(); 
            ImGui.Dummy(new Vector2(0, 10));

            DrawInlineIconColored(FontAwesomeIcon.Globe, ACCENT_COLOR); ImGui.SameLine();
            ImGui.Text(Lang.GetText("Plugin Interface Language"));
            
            string[] supportedLangNames = { "简体中文", "繁體中文", "English", "日本語", "한국어", "Deutsch", "Français" };
            string[] supportedLangs = { "zh", "zh_TW", "en", "ja", "ko", "de", "fr" };
            int currentLangIndex = Math.Max(0, Array.IndexOf(supportedLangs, Config.UILanguage));

            ImGui.SetNextItemWidth(180); 
            if (ImGui.BeginCombo("##UILang", supportedLangNames[currentLangIndex]))
            {
                for (int i = 0; i < supportedLangs.Length; i++)
                {
                    DrawInlineIconColored(FontAwesomeIcon.Globe, ACCENT_COLOR); ImGui.SameLine();
                    if (ImGui.Selectable(supportedLangNames[i], currentLangIndex == i))
                    {
                        Config.UILanguage = supportedLangs[i];
                        Config.Save();
                        Lang.SetLanguage(Config.UILanguage);
                    }
                }
                ImGui.EndCombo();
            }

            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));

            DrawInlineIconColored(FontAwesomeIcon.CheckSquare, ACCENT_COLOR); ImGui.SameLine();
            if (ImGui.Checkbox(Lang.GetText("Apply Layout"), ref Config.ApplyLayout)) Config.Save();
            
            AlmondHousing.CheckBDTHCompatibility();
            if (AlmondHousing.UseEmbeddedBDTH)
            {
                DrawInlineIconColored(FontAwesomeIcon.UnlockAlt, ACCENT_COLOR); ImGui.SameLine();
                if (ImGui.Checkbox(Lang.GetText("EnableQuantumPlace"), ref Config.EnableQuantumPlace)) 
                {
                    Config.Save();
                    Memory.Instance.SetPlaceAnywhere(Config.EnableQuantumPlace); 
                }
                if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("QuantumPlaceHelp"));
            }
            else
            {
                DrawInlineIconColored(FontAwesomeIcon.Link, new Vector4(0.2f, 0.8f, 0.2f, 1.0f)); ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.2f, 0.8f, 0.2f, 1.0f), Lang.GetText("QuantumPlaceLinked"));
                if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("QuantumPlaceLinkedHelp"));
            }

            ImGui.Dummy(new Vector2(0, 5));
            DrawInlineIconColored(FontAwesomeIcon.Crosshairs, ACCENT_COLOR); ImGui.SameLine();
            if (ImGui.Checkbox(Lang.GetText("EnableGizmo"), ref Config.UseGizmo)) Config.Save();
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("GizmoHelp"));

            ImGui.SameLine(0, 20);
            DrawInlineIconColored(FontAwesomeIcon.Anchor, ACCENT_COLOR); ImGui.SameLine();
            if (ImGui.Checkbox(Lang.GetText("DragSnap"), ref Config.DoSnap)) Config.Save();

            DrawInlineIconColored(FontAwesomeIcon.Pen, ACCENT_COLOR); ImGui.SameLine();
            ImGui.SetNextItemWidth(150);
            if (ImGui.InputFloat(Lang.GetText("DragPrecision"), ref Config.Drag, 0.01f, 0.1f, "%.3f"))
            {
                Config.Drag = Math.Max(0.001f, Math.Min(Config.Drag, 10f));
                Config.Save();
            }
            
            ImGui.Dummy(new Vector2(0, 5));
            DrawInlineIconColored(FontAwesomeIcon.Tag, ACCENT_COLOR); ImGui.SameLine();
            if (ImGui.Checkbox(Lang.GetText("Label Furniture"), ref Config.DrawScreen)) Config.Save();
            
            DrawInlineIconColored(FontAwesomeIcon.InfoCircle, ACCENT_COLOR); ImGui.SameLine();
            if (ImGui.Checkbox(Lang.GetText("Show Tooltips"), ref Config.ShowTooltips)) Config.Save();

            DrawInlineIconColored(FontAwesomeIcon.Clock, ACCENT_COLOR); ImGui.SameLine();
            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt(Lang.GetText("Placement Interval (ms)"), ref Config.LoadInterval)) Config.Save();

            if (Memory.Instance != null && Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors && Memory.Instance.GetIndoorHouseSize() != "Apartment")
            {
                ImGui.Dummy(new Vector2(0, 5));
                DrawInlineIconColored(FontAwesomeIcon.LayerGroup, ACCENT_COLOR); ImGui.SameLine();
                ImGui.Text(Lang.GetText("Selected Floors"));
                
                if (ImGui.Checkbox(Lang.GetText("Basement"), ref Config.Basement)) Config.Save();
                ImGui.SameLine();
                if (ImGui.Checkbox(Lang.GetText("Ground Floor"), ref Config.GroundFloor)) Config.Save();
                ImGui.SameLine();
                if (Memory.Instance.HasUpperFloor() && ImGui.Checkbox(Lang.GetText("Upper Floor"), ref Config.UpperFloor)) Config.Save();
            }
        }
    }
}