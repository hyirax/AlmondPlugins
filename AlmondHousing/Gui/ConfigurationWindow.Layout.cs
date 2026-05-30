using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Utility;
using static AlmondHousing.AlmondHousing;

namespace AlmondHousing.Gui
{
    public partial class ConfigurationWindow
    {
        private void DrawLayoutFileTab()
        {
            DrawInlineIconColored(FontAwesomeIcon.FolderOpen, ACCENT_COLOR); ImGui.SameLine();
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Layout & Export"));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 10));

            if (AlmondHousing.Session.IsActive && AlmondHousing.Session.TotalCount > 0)
            {
                float progress = 1.0f - ((float)AlmondHousing.Session.PendingItems.Count / AlmondHousing.Session.TotalCount);
                
                DrawInlineIconColored(FontAwesomeIcon.Tools, new Vector4(0.2f, 0.8f, 1.0f, 1.0f)); ImGui.SameLine();
                ImGui.TextColored(new Vector4(0.2f, 0.8f, 1.0f, 1.0f), Lang.GetText("Construction in progress..."));
                
                ImGui.ProgressBar(progress, new Vector2(-1, 24), string.Format(Lang.GetText("{0} / {1} Completed"), AlmondHousing.Session.TotalCount - AlmondHousing.Session.PendingItems.Count, AlmondHousing.Session.TotalCount));
                ImGui.Spacing(); ImGui.Separator(); ImGui.Spacing();
            }

            DrawInlineIconColored(FontAwesomeIcon.Save, ACCENT_COLOR); ImGui.SameLine();
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Save Layout"));

            bool hasPermission = Memory.Instance != null && Memory.Instance.IsHousingMode();

            if (hasPermission)
            {
                if (!Config.SaveLocation.IsNullOrEmpty())
                {
                    ImGui.TextWrapped($"{Lang.GetText("Current Path:")} {Config.SaveLocation}");
                    
                    if (CustomUI.AnimatedButton("btn_save", Lang.GetText("Save"), new Vector2(180, 36), FontAwesomeIcon.Save))
                        SaveLayoutToFile();
                    ImGui.SameLine();
                }
                
                if (CustomUI.AnimatedButton("btn_save_as", Lang.GetText("Save As"), new Vector2(180, 36), FontAwesomeIcon.FolderOpen))
                    ShowSaveDialog();
            }
            else
            {
                ImGui.Dummy(new Vector2(0, 5));
                DrawInlineIconColored(FontAwesomeIcon.Lock, new Vector4(0.6f, 0.6f, 0.6f, 1.0f)); ImGui.SameLine();
                ImGui.TextDisabled(Lang.GetText("LockMsg"));
            }

            ImGui.Dummy(new Vector2(0, 10));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 10));

            DrawInlineIconColored(FontAwesomeIcon.FileImport, ACCENT_COLOR); ImGui.SameLine();
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Apply & Load Layout"));

            float buttonWidth = 200f; 

            void DrawHelpText(string text)
            {
                ImGui.SameLine(0, 15);
                DrawInlineIconColored(FontAwesomeIcon.InfoCircle, ACCENT_COLOR);
                ImGui.SameLine();
                ImGui.TextDisabled(text);
            }

            if (!Config.SaveLocation.IsNullOrEmpty())
            {
                if (CustomUI.AnimatedButton("btn_apply_curr", Lang.GetText("Apply Current"), new Vector2(buttonWidth, 36), FontAwesomeIcon.PlayCircle)) { CreateAutoBackup(); LoadLayoutFromFile(true); }
                DrawHelpText(Lang.GetText("Read from current path and place immediately"));
            }

            if (CustomUI.AnimatedButton("btn_apply_file", Lang.GetText("Apply from File"), new Vector2(buttonWidth, 36), FontAwesomeIcon.FileImport)) { ShowLoadDialog(true); }
            DrawHelpText(Lang.GetText("Select a file and start placing immediately"));

            if (!Config.SaveLocation.IsNullOrEmpty())
            {
                if (CustomUI.AnimatedButton("btn_load_curr", Lang.GetText("Load Current"), new Vector2(buttonWidth, 36), FontAwesomeIcon.Sync)) { LoadLayoutFromFile(false); }
                DrawHelpText(Lang.GetText("Update list only, do not move furniture"));
            }

            if (CustomUI.AnimatedButton("btn_load_file", Lang.GetText("Load from File"), new Vector2(buttonWidth, 36), FontAwesomeIcon.Folder)) { ShowLoadDialog(false); }
            DrawHelpText(Lang.GetText("Select file and update list, do not move"));

            ImGui.Dummy(new Vector2(0, 20));
            
            DrawInlineIconColored(FontAwesomeIcon.ShoppingCart, ACCENT_COLOR); ImGui.SameLine();
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Export Shopping List"));
            ImGui.Separator();

            ImGui.SetNextItemWidth(180); 
            if (ImGui.BeginCombo("##ExportLang", currentExportLang.ToString()))
            {
                foreach (var lang in new[] { Dalamud.Game.ClientLanguage.English, Dalamud.Game.ClientLanguage.Japanese, Dalamud.Game.ClientLanguage.German, Dalamud.Game.ClientLanguage.French })
                {
                    if (ImGui.Selectable(lang.ToString(), currentExportLang == lang)) currentExportLang = lang;
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            
            if (CustomUI.AnimatedButton("btn_exp_tc", Lang.GetText("Export for Teamcraft"), new Vector2(220, 32), FontAwesomeIcon.Clipboard)) ExportToTeamcraft(currentExportLang);
            ImGui.SameLine();
            if (CustomUI.AnimatedButton("btn_exp_csv", Lang.GetText("Export to CSV"), new Vector2(180, 32), FontAwesomeIcon.FileCsv)) ExportToCSV(currentExportLang);

            if (_isDeveloperModeUnlocked)
            {
                ImGui.Dummy(new Vector2(0, 20));
                ImGui.Separator();
                ImGui.Dummy(new Vector2(0, 10));

                float pulseAlpha = (float)(Math.Sin(ImGui.GetTime() * 5.0) + 1.0) / 2.0f;
                Vector4 warningColor = new Vector4(1.0f, 0.2f, 0.2f, 0.5f + (pulseAlpha * 0.5f));
                
                DrawInlineIconColored(FontAwesomeIcon.UserSecret, warningColor); ImGui.SameLine();
                ImGui.TextColored(warningColor, Lang.GetText("OverrideActivated"));

                ImGui.Dummy(new Vector2(0, 5));
                ImGui.TextDisabled(Lang.GetText("DevPrivilegesMsg"));

                ImGui.Dummy(new Vector2(0, 5));
                
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.1f, 0.4f, 1f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.5f, 0.2f, 0.7f, 1f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.7f, 0.3f, 1.0f, 1f));

                if (ImGui.Button(Lang.GetText("ForceExtractSave") + " ##cheat_clone", new Vector2(-1, 40)))
                {
                    Plugin.GetGameLayout(); 
                    ShowSaveDialog();       
                    Log(Lang.GetText("DataExtractedMsg"));
                }
                
                ImGui.PopStyleColor(3);
            }
        }

        private bool CheckModeForSave() => true;

        private bool CheckModeForLoad(bool shouldApply)
        {
            if (shouldApply && !Memory.Instance.CanEditItem())
            {
                LogError(Lang.GetText("Unable to set position outside of Rotate Layout mode"));
                return false;
            }
            if (!shouldApply && !Memory.Instance.IsHousingMode())
            {
                LogError(Lang.GetText("Unable to load layouts outside of Layout mode"));
                return false;
            }
            return true;
        }

        private void CreateAutoBackup()
        {
            try
            {
                Plugin.GetGameLayout();
                string backupDir = Path.Combine(AlmondHousing.PluginDirectory, "Backups");
                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);
                string backupFileName = $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}.almond";
                string backupPath = Path.Combine(backupDir, backupFileName);
                string originalSaveLocation = Config.SaveLocation;
                Config.SaveLocation = backupPath;
                AlmondHousing.LayoutManager.ExportLayout();
                Config.SaveLocation = originalSaveLocation;
                Log(string.Format(Lang.GetText("Auto-backup created: {0}"), backupFileName));
            }
            catch (Exception e) { LogError(string.Format(Lang.GetText("Backup Error: {0}"), e.Message)); }
        }

        private void ShowSaveDialog()
        {
            string saveName = Config.SaveLocation.IsNullOrEmpty() ? "save" : Path.GetFileNameWithoutExtension(Config.SaveLocation);
            FileDialogManager.SaveFileDialog(Lang.GetText("Select a Save Location"), ".almond", saveName, "almond", (bool ok, string res) =>
            {
                if (!ok) return;
                Config.SaveLocation = res; Config.Save(); SaveLayoutToFile();
            }, Path.GetDirectoryName(Config.SaveLocation));
        }

        private void ShowLoadDialog(bool shouldApply)
        {
            FileDialogManager.OpenFileDialog(Lang.GetText("Select a Layout File"), ".almond,.json", (bool ok, List<string> res) =>
            {
                if (!ok) return;
                if (shouldApply) CreateAutoBackup(); 
                Config.SaveLocation = res.FirstOrDefault(""); 
                Config.Save(); 
                LoadLayoutFromFile(shouldApply);
            }, 1, Path.GetDirectoryName(Config.SaveLocation));
        }

        private void SaveLayoutToFile()
        {
            try { Plugin.GetGameLayout(); AlmondHousing.LayoutManager.ExportLayout(); }
            catch (Exception e) { LogError(string.Format(Lang.GetText("Save Error: {0}"), e.Message), e.StackTrace ?? ""); }
        }

        private void LoadLayoutFromFile(bool shouldApply)
        {
            if (!CheckModeForLoad(shouldApply)) return;
            try { 
                SaveLayoutManager.ImportLayout(Config.SaveLocation); 
                Log(string.Format(Lang.GetText("Imported {0} items"), Plugin.InteriorItemList.Count + Plugin.ExteriorItemList.Count));
                
                Plugin.MatchLayout(); 
                Config.ResetRecord(); 

                InvalidateMaterialCache();
                
                if (shouldApply) Plugin.ApplyLayout(); 
            }
            catch (Exception e) { LogError(string.Format(Lang.GetText("Load Error: {0}"), e.Message), e.StackTrace ?? ""); }
        }
    }
}