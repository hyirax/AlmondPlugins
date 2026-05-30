using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using static AlmondHousing.AlmondHousing;

namespace AlmondHousing.Gui
{
    public partial class ConfigurationWindow
    {
        private void DrawHomeTab()
        {
            DrawInlineIconColored(FontAwesomeIcon.Leaf, ACCENT_COLOR); ImGui.SameLine();
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Welcome to AlmondHousing!"));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 10));

            ImGui.BeginChild("HomeInfo", new Vector2(0, 0), false);
            {
                DrawInlineIconColored(FontAwesomeIcon.InfoCircle, ACCENT_COLOR); ImGui.SameLine();
                ImGui.TextColored(ACCENT_COLOR, Lang.GetText("About this Plugin"));
                ImGui.TextWrapped(Lang.GetText("HomeDesc1"));
                ImGui.Dummy(new Vector2(0, 10));

                DrawInlineIconColored(FontAwesomeIcon.Star, ACCENT_COLOR); ImGui.SameLine();
                ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Core Features"));
                ImGui.BulletText(Lang.GetText("Feat1"));
                ImGui.BulletText(Lang.GetText("Feat2"));
                ImGui.BulletText(Lang.GetText("Feat3"));
                ImGui.BulletText(Lang.GetText("Feat4"));
                ImGui.Dummy(new Vector2(0, 10));
                
                DrawInlineIconColored(FontAwesomeIcon.Heart, ACCENT_COLOR); ImGui.SameLine();
                ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Credits & Acknowledgements"));
                ImGui.TextWrapped(Lang.GetText("CreditDesc"));
                ImGui.Dummy(new Vector2(0, 5));
                ImGui.Indent(10f);
                ImGui.TextWrapped(Lang.GetText("Credit1"));
                ImGui.TextWrapped(Lang.GetText("Credit2"));
                ImGui.TextWrapped(Lang.GetText("Credit3"));
                ImGui.TextWrapped(Lang.GetText("Credit4"));
                ImGui.Unindent(10f);
                ImGui.Dummy(new Vector2(0, 15));

                DrawInlineIconColored(FontAwesomeIcon.Ban, new Vector4(1.0f, 0.4f, 0.4f, 1.0f)); ImGui.SameLine();
                ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.4f, 0.4f, 1.0f)); 
                ImGui.TextUnformatted(Lang.GetText("Strict Anti-Resell Warning"));
                ImGui.Separator();
                ImGui.TextWrapped(Lang.GetText("Warn1"));
                ImGui.TextWrapped(Lang.GetText("Warn2"));
                ImGui.TextWrapped(Lang.GetText("Warn3"));
                ImGui.PopStyleColor();
            }
            ImGui.EndChild();
        }
    }
}