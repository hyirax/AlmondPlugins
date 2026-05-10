using Dalamud.Interface.ImGuiFileDialog;
using Dalamud.Utility;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using AlmondHousing.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using static AlmondHousing.AlmondHousing;
using Dalamud.Interface.Textures;

namespace AlmondHousing.Gui
{
    public class ConfigurationWindow : Window<AlmondHousing>
    {
        public Configuration Config => Plugin.Config;

        private string CustomTag = string.Empty;
        private readonly Dictionary<uint, uint> iconToFurniture = new() { };

        // === 🎨 全新配色方案：高级深空灰 (代替原来的紫色) ===
        private readonly Vector4 THEME_BASE = new(0.20f, 0.20f, 0.20f, 1f);     // 基础深灰
        private readonly Vector4 THEME_HOVER = new(0.35f, 0.35f, 0.35f, 1f);    // 悬停浅灰
        private readonly Vector4 THEME_ACTIVE = new(0.45f, 0.45f, 0.45f, 1f);   // 点击亮灰
        private readonly Vector4 THEME_HEADER = new(0.15f, 0.15f, 0.15f, 0.8f); // 标题栏透明灰

        private FileDialogManager FileDialogManager { get; }

        private Dalamud.Game.ClientLanguage currentExportLang = Dalamud.Game.ClientLanguage.English;

        public ConfigurationWindow(AlmondHousing plugin) : base(plugin)
        {
            this.FileDialogManager = new FileDialogManager
            {
                AddedWindowFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDocking,
            };
        }

        protected void DrawAllUi()
        {
            if (!ImGui.Begin($"{Plugin.Name}", ref WindowVisible, ImGuiWindowFlags.NoScrollWithMouse))
            {
                return;
            }
            if (ImGui.BeginChild("##SettingsRegion"))
            {
                DrawGeneralSettings();
                if (ImGui.BeginChild("##ItemListRegion"))
                {
                    // 应用新的灰色主题到折叠面板
                    ImGui.PushStyleColor(ImGuiCol.Header, THEME_HEADER);
                    ImGui.PushStyleColor(ImGuiCol.HeaderHovered, THEME_HOVER);
                    ImGui.PushStyleColor(ImGuiCol.HeaderActive, THEME_ACTIVE);

                    if (ImGui.CollapsingHeader(Lang.GetText("Interior Furniture"), ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("interior");
                        DrawItemList(Plugin.InteriorItemList);
                        ImGui.PopID();
                    }
                    if (ImGui.CollapsingHeader(Lang.GetText("Exterior Furniture"), ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("exterior");
                        DrawItemList(Plugin.ExteriorItemList);
                        ImGui.PopID();
                    }

                    if (ImGui.CollapsingHeader(Lang.GetText("Interior Fixtures"), ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("interiorFixture");
                        DrawFixtureList(Plugin.Layout.interiorFixture);
                        ImGui.PopID();
                    }

                    if (ImGui.CollapsingHeader(Lang.GetText("Exterior Fixtures"), ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("exteriorFixture");
                        DrawFixtureList(Plugin.Layout.exteriorFixture);
                        ImGui.PopID();
                    }
                    if (ImGui.CollapsingHeader(Lang.GetText("Unused Furniture"), ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        ImGui.PushID("unused");
                        DrawItemList(Plugin.UnusedItemList, true);
                        ImGui.PopID();
                    }

                    ImGui.PopStyleColor(3);
                    ImGui.EndChild();
                }
                ImGui.EndChild();
            }

            this.FileDialogManager.Draw();
        }

        protected override void DrawUi()
        {
            // 应用新的灰色主题到主窗口和按钮
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, THEME_BASE);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, THEME_HOVER);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, THEME_ACTIVE);

            // 稍微调宽一点窗口，防止一行放不下
            ImGui.SetNextWindowSize(new Vector2(580, 450), ImGuiCond.FirstUseEver);

            DrawAllUi();

            ImGui.PopStyleColor(3);
            ImGui.End();
        }

        #region Helper Functions
        public static void DrawIcon(ushort icon, Vector2 size)
        {
            if (icon < 65000)
            {
                var iconTexture = DalamudApi.TextureProvider.GetFromGameIcon(new GameIconLookup(icon));
                ImGui.Image(iconTexture.GetWrapOrEmpty().Handle, size);
            }
        }
        #endregion

        #region Basic UI

        private void LogLayoutMode()
        {
            if (Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Island)
            {
                LogError(Lang.GetText("(Manage Furnishings -> Place Furnishing Glamours)"));
            }
            else
            {
                LogError(Lang.GetText("(Housing -> Indoor/Outdoor Furnishings)"));
            }
        }

        private bool CheckModeForSave()
        {
            return true; // LOL, LMAO
        }

        private bool CheckModeForLoad()
        {
            if (Config.ApplyLayout && !Memory.Instance.CanEditItem())
            {
                LogError(Lang.GetText("Unable to load and apply layouts outside of Rotate Layout mode"));
                return false;
            }

            if (!Config.ApplyLayout && !Memory.Instance.IsHousingMode())
            {
                LogError(Lang.GetText("Unable to load layouts outside of Layout mode"));
                LogLayoutMode();
                return false;
            }

            return true;
        }

        private void SaveLayoutToFile()
        {
            if (!CheckModeForSave())
            {
                return;
            }

            try
            {
                Plugin.GetGameLayout();
                AlmondHousing.LayoutManager.ExportLayout();
            }
            catch (Exception e)
            {
                LogError(string.Format(Lang.GetText("Save Error: {0}"), e.Message), e.StackTrace);
            }
        }

        private void LoadLayoutFromFile()
        {

            if (!CheckModeForLoad()) return;

            try
            {
                SaveLayoutManager.ImportLayout(Config.SaveLocation);
                Log(string.Format(Lang.GetText("Imported {0} items"), Plugin.InteriorItemList.Count + Plugin.ExteriorItemList.Count));

                Plugin.MatchLayout();
                Config.ResetRecord();

                if (Config.ApplyLayout)
                {
                    Plugin.ApplyLayout();
                }

            }
            catch (Exception e)
            {
                LogError(string.Format(Lang.GetText("Load Error: {0}"), e.Message), e.StackTrace);
            }
        }

        unsafe private void DrawGeneralSettings()
        {
            // === 防倒卖大字报 开始 ===
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.2f, 0.2f, 1.0f));
            ImGui.TextWrapped("严正声明：本汉化插件完全免费且开源！如果你是在闲鱼等平台花钱买的，请立刻退款并反手举报！闲鱼倒卖死妈！");
            ImGui.PopStyleColor();
            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));
            // === 防倒卖大字报 结束 ===

            // ================= 🚀 全新紧凑控制台：语言与导出合并 =================
            string[] supportedLangs = { "zh", "zh_TW", "en", "ja", "ko", "de", "fr" };
            string[] supportedLangNames = { "简体中文", "繁體中文", "English", "日本語", "한국어", "Deutsch", "Français" };
            int currentLangIndex = Array.IndexOf(supportedLangs, Config.UILanguage);
            if (currentLangIndex == -1) currentLangIndex = 0;

            var availableLanguages = new[] {
                Dalamud.Game.ClientLanguage.English,
                Dalamud.Game.ClientLanguage.Japanese,
                Dalamud.Game.ClientLanguage.German,
                Dalamud.Game.ClientLanguage.French
            };

            // 【第一列】：界面语言
            ImGui.SetNextItemWidth(100);
            if (ImGui.BeginCombo("##UILangCombo", supportedLangNames[currentLangIndex]))
            {
                for (int i = 0; i < supportedLangs.Length; i++)
                {
                    bool isSelected = (currentLangIndex == i);
                    if (ImGui.Selectable(supportedLangNames[i], isSelected))
                    {
                        Config.UILanguage = supportedLangs[i];
                        Config.Save();
                        Lang.SetLanguage(Config.UILanguage);
                    }
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Plugin Interface Language / 插件语言"));

            ImGui.SameLine();

            // 【第二列】：导出语言
            ImGui.SetNextItemWidth(100);
            if (ImGui.BeginCombo("##ExportLangCombo", currentExportLang.ToString()))
            {
                foreach (var lang in availableLanguages)
                {
                    bool isSelected = (currentExportLang == lang);
                    if (ImGui.Selectable(lang.ToString(), isSelected))
                    {
                        currentExportLang = lang;
                    }
                    if (isSelected) ImGui.SetItemDefaultFocus();
                }
                ImGui.EndCombo();
            }
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Export Language"));

            ImGui.SameLine();

            // 【第三列】：导出 Teamcraft
            if (ImGui.Button(Lang.GetText("Export for Teamcraft")))
            {
                ExportToTeamcraft(currentExportLang);
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered())
                ImGui.SetTooltip(Lang.GetText("Copy to clipboard for Teamcraft 'Import from text'"));

            ImGui.SameLine();

            // 【第四列】：导出 CSV
            if (ImGui.Button(Lang.GetText("Export to CSV")))
            {
                ExportToCSV(currentExportLang);
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered())
                ImGui.SetTooltip(Lang.GetText("Copy as CSV format to paste into Excel"));

            ImGui.Dummy(new Vector2(0, 5));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));
            // ===================================================================

            if (ImGui.Checkbox(Lang.GetText("Label Furniture"), ref Config.DrawScreen)) Config.Save();
            if (Config.ShowTooltips && ImGui.IsItemHovered())
                ImGui.SetTooltip(Lang.GetText("Show furniture names on the screen"));

            ImGui.SameLine();
            ImGui.Dummy(new Vector2(10, 0));
            ImGui.SameLine();
            if (ImGui.Checkbox("##hideTooltipsOnOff", ref Config.ShowTooltips)) Config.Save();
            ImGui.SameLine();
            ImGui.TextUnformatted(Lang.GetText("Show Tooltips"));

            ImGui.Dummy(new Vector2(0, 5));


            ImGui.Text(Lang.GetText("Layout"));

            if (!Config.SaveLocation.IsNullOrEmpty())
            {
                ImGui.Text($"{Lang.GetText("Current file location:")} {Config.SaveLocation}");

                if (ImGui.Button(Lang.GetText("Save")))
                {
                    SaveLayoutToFile();
                }
                if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Save layout to current file location"));
                ImGui.SameLine();

            }

            if (ImGui.Button(Lang.GetText("Save As")))
            {
                if (CheckModeForSave())
                {
                    string saveName = "save";
                    if (!Config.SaveLocation.IsNullOrEmpty()) saveName = Path.GetFileNameWithoutExtension(Config.SaveLocation);

                    FileDialogManager.SaveFileDialog(Lang.GetText("Select a Save Location"), ".json", saveName, "json", (bool ok, string res) =>
                    {
                        if (!ok)
                        {
                            return;
                        }

                        Config.SaveLocation = res;
                        Config.Save();
                        SaveLayoutToFile();

                    }, Path.GetDirectoryName(Config.SaveLocation));
                }
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Save layout to file"));

            ImGui.SameLine(); ImGui.Dummy(new Vector2(20, 0)); ImGui.SameLine();

            if (!Config.SaveLocation.IsNullOrEmpty())
            {
                if (ImGui.Button(Lang.GetText("Load")))
                {
                    LoadLayoutFromFile();
                }
                if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Load layout from current file location"));
                ImGui.SameLine();
            }

            if (ImGui.Button(Lang.GetText("Load From")))
            {
                if (CheckModeForLoad())
                {
                    string saveName = "save";
                    if (!Config.SaveLocation.IsNullOrEmpty()) saveName = Path.GetFileNameWithoutExtension(Config.SaveLocation);

                    FileDialogManager.OpenFileDialog(Lang.GetText("Select a Layout File"), ".json", (bool ok, List<string> res) =>
                    {
                        if (!ok)
                        {
                            return;
                        }

                        Config.SaveLocation = res.FirstOrDefault("");
                        Config.Save();

                        LoadLayoutFromFile();

                    }, 1, Path.GetDirectoryName(Config.SaveLocation));
                }
            }
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Load layout from file"));

            ImGui.SameLine(); ImGui.Dummy(new Vector2(10, 0)); ImGui.SameLine();

            if (ImGui.Checkbox(Lang.GetText("Apply Layout"), ref Config.ApplyLayout))
            {
                Config.Save();
            }

            ImGui.SameLine(); ImGui.Dummy(new Vector2(10, 0)); ImGui.SameLine();

            ImGui.PushItemWidth(100);
            if (ImGui.InputInt(Lang.GetText("Placement Interval (ms)"), ref Config.LoadInterval))
            {
                Config.Save();
            }
            ImGui.PopItemWidth();
            if (Config.ShowTooltips && ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Time interval between furniture placements when applying a layout. If this is too low (e.g. 200 ms), some placements may be skipped over."));

            ImGui.Dummy(new Vector2(0, 10));

            bool hasFloors = Memory.Instance != null && Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors && Memory.Instance.GetIndoorHouseSize() != "Apartment";

            if (hasFloors)
            {

                ImGui.Text(Lang.GetText("Selected Floors"));

                if (ImGui.Checkbox(Lang.GetText("Basement"), ref Config.Basement))
                {
                    Config.Save();
                }
                ImGui.SameLine(); ImGui.Dummy(new Vector2(10, 0)); ImGui.SameLine();

                if (ImGui.Checkbox(Lang.GetText("Ground Floor"), ref Config.GroundFloor))
                {
                    Config.Save();
                }
                ImGui.SameLine(); ImGui.Dummy(new Vector2(10, 0)); ImGui.SameLine();

                if (Memory.Instance.HasUpperFloor() && ImGui.Checkbox(Lang.GetText("Upper Floor"), ref Config.UpperFloor))
                {
                    Config.Save();
                }

                ImGui.Dummy(new Vector2(0, 10));

            }

            ImGui.Dummy(new Vector2(0, 5));

        }

        private void DrawRow(int i, HousingItem housingItem, bool showSetPosition = true, int childIndex = -1)
        {
            if (!housingItem.CorrectLocation) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
            ImGui.Text($"{housingItem.X:N4}, {housingItem.Y:N4}, {housingItem.Z:N4}");
            if (!housingItem.CorrectLocation) ImGui.PopStyleColor();

            ImGui.NextColumn();

            if (!housingItem.CorrectRotation) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
            ImGui.Text($"{housingItem.Rotate:N3}"); ImGui.NextColumn();
            if (!housingItem.CorrectRotation) ImGui.PopStyleColor();

            var stainSheet = DalamudApi.DataManager.GetExcelSheet<Stain>();
            string colorName = "Unknown";
            Stain stain = new Stain();

            if (housingItem.Stain != 0 && stainSheet.HasRow(housingItem.Stain))
            {
                stain = stainSheet.GetRow(housingItem.Stain);
                colorName = stain.Name.ToString();

                Utils.StainButton("dye_" + i, stain, new Vector2(20));
                ImGui.SameLine();

                if (!housingItem.DyeMatch) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));

                ImGui.Text($"{colorName}");

                if (!housingItem.DyeMatch) ImGui.PopStyleColor();
            }
            else if (housingItem.MaterialItemKey != 0)
            {
                var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>();
                if (itemSheet.HasRow(housingItem.MaterialItemKey))
                {
                    var item = itemSheet.GetRow(housingItem.MaterialItemKey);

                    if (!housingItem.DyeMatch) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));

                    DrawIcon(item.Icon, new Vector2(20, 20));
                    ImGui.SameLine();
                    ImGui.Text(item.Name.ToString());

                    if (!housingItem.DyeMatch) ImGui.PopStyleColor();
                }

            }
            ImGui.NextColumn();

            if (showSetPosition)
            {
                string uniqueID = childIndex == -1 ? i.ToString() : i.ToString() + "_" + childIndex.ToString();

                bool noMatch = housingItem.ItemStruct == IntPtr.Zero;

                if (!noMatch)
                {
                    if (ImGui.Button(Lang.GetText("Set") + "##" + uniqueID))
                    {
                        Plugin.MatchLayout();

                        if (housingItem.ItemStruct != IntPtr.Zero)
                        {
                            SetItemPosition(housingItem);
                        }
                        else
                        {
                            LogError(string.Format(Lang.GetText("Unable to set position for {0}"), housingItem.Name));
                        }
                    }
                }

                ImGui.NextColumn();
            }

        }

        private void DrawFixtureList(List<Fixture> fixtureList)
        {
            try
            {
                if (ImGui.Button(Lang.GetText("Clear")))
                {
                    fixtureList.Clear();
                    Config.Save();
                }

                ImGui.Columns(3, "FixtureList", true);
                ImGui.Separator();

                ImGui.Text(Lang.GetText("Level")); ImGui.NextColumn();
                ImGui.Text(Lang.GetText("Fixture")); ImGui.NextColumn();
                ImGui.Text(Lang.GetText("Item")); ImGui.NextColumn();

                ImGui.Separator();

                var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>();

                foreach (var fixture in fixtureList)
                {
                    ImGui.Text(fixture.level); ImGui.NextColumn();
                    ImGui.Text(fixture.type); ImGui.NextColumn();

                    if (itemSheet.HasRow(fixture.itemId))
                    {
                        var item = itemSheet.GetRow(fixture.itemId);
                        DrawIcon(item.Icon, new Vector2(20, 20));
                        ImGui.SameLine();
                    }
                    ImGui.Text(fixture.name); ImGui.NextColumn();

                    ImGui.Separator();
                }

                ImGui.Columns(1);

            }
            catch (Exception e)
            {
                LogError(e.Message, e.StackTrace);
            }

        }

        private void DrawItemList(List<HousingItem> itemList, bool isUnused = false)
        {

            if (ImGui.Button(Lang.GetText("Sort")))
            {
                itemList.Sort((x, y) =>
                {
                    if (x.Name.CompareTo(y.Name) != 0)
                        return x.Name.CompareTo(y.Name);
                    if (x.X.CompareTo(y.X) != 0)
                        return x.X.CompareTo(y.X);
                    if (x.Y.CompareTo(y.Y) != 0)
                        return x.Y.CompareTo(y.Y);
                    if (x.Z.CompareTo(y.Z) != 0)
                        return x.Z.CompareTo(y.Z);
                    if (x.Rotate.CompareTo(y.Rotate) != 0)
                        return x.Rotate.CompareTo(y.Rotate);
                    return 0;
                });
                Config.Save();
            }
            ImGui.SameLine();
            if (ImGui.Button(Lang.GetText("Clear") + "##ItemClear"))
            {
                itemList.Clear();
                Config.Save();
            }

            if (!isUnused)
            {
                ImGui.SameLine();
                ImGui.Text(Lang.GetText("Note: Missing items, incorrect dyes, and items on unselected floors are grayed out"));
            }

            // name, position, r, color, set
            int columns = isUnused ? 4 : 5;


            ImGui.Columns(columns, "ItemList", true);
            ImGui.Separator();
            ImGui.Text(Lang.GetText("Item")); ImGui.NextColumn();
            ImGui.Text(Lang.GetText("Position (X,Y,Z)")); ImGui.NextColumn();
            ImGui.Text(Lang.GetText("Rotation")); ImGui.NextColumn();
            ImGui.Text(Lang.GetText("Dye/Material")); ImGui.NextColumn();

            if (!isUnused)
            {
                ImGui.Text(Lang.GetText("Set Position")); ImGui.NextColumn();
            }

            ImGui.Separator();
            var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>();

            for (int i = 0; i < itemList.Count(); i++)
            {
                var housingItem = itemList[i];
                var displayName = housingItem.Name;

                if (itemSheet.HasRow(housingItem.ItemKey))
                {
                    var item = itemSheet.GetRow(housingItem.ItemKey);
                    DrawIcon(item.Icon, new Vector2(20, 20));
                    ImGui.SameLine();
                }

                if (housingItem.ItemStruct == IntPtr.Zero)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                }

                ImGui.Text(displayName);

                ImGui.NextColumn();
                DrawRow(i, housingItem, !isUnused);

                if (housingItem.ItemStruct == IntPtr.Zero)
                {
                    ImGui.PopStyleColor();
                }

                ImGui.Separator();
            }

            ImGui.Columns(1);

        }

        #endregion


        #region Draw Screen
        protected override void DrawScreen()
        {
            if (Config.DrawScreen)
            {
                DrawItemOnScreen();
            }
        }

        private unsafe void DrawItemOnScreen()
        {

            if (Memory.Instance == null) return;

            var itemList = Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors ? Plugin.InteriorItemList : Plugin.ExteriorItemList;

            for (int i = 0; i < itemList.Count(); i++)
            {
                var playerPos = DalamudApi.ObjectTable.LocalPlayer.Position;
                var housingItem = itemList[i];

                if (housingItem.ItemStruct == IntPtr.Zero) continue;

                var itemStruct = (HousingItemStruct*)housingItem.ItemStruct;

                var itemPos = new Vector3(itemStruct->Position.X, itemStruct->Position.Y, itemStruct->Position.Z);
                if (Config.HiddenScreenItemHistory.IndexOf(i) >= 0) continue;
                if (Config.DrawDistance > 0 && (playerPos - itemPos).Length() > Config.DrawDistance)
                    continue;
                var displayName = housingItem.Name;
                if (DalamudApi.GameGui.WorldToScreen(itemPos, out var screenCoords))
                {
                    ImGui.PushID("HousingItemWindow" + i);
                    ImGui.SetNextWindowPos(new Vector2(screenCoords.X, screenCoords.Y));
                    ImGui.SetNextWindowBgAlpha(0.8f);
                    if (ImGui.Begin("HousingItem" + i,
                        ImGuiWindowFlags.NoDecoration |
                        ImGuiWindowFlags.AlwaysAutoResize |
                        ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove |
                        ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav))
                    {

                        ImGui.Text(displayName);

                        ImGui.SameLine();

                        if (ImGui.Button(Lang.GetText("Set") + "##ScreenItem" + i.ToString()))
                        {
                            if (!Memory.Instance.CanEditItem())
                            {
                                LogError(Lang.GetText("Unable to set position while not in rotate layout mode"));
                                continue;
                            }

                            SetItemPosition(housingItem);
                            Config.HiddenScreenItemHistory.Add(i);
                            Config.Save();
                        }

                        ImGui.SameLine();

                        ImGui.End();
                    }

                    ImGui.PopID();
                }
            }
        }
        #endregion

        #region Export Features

        private void ExportToTeamcraft(Dalamud.Game.ClientLanguage targetLang)
        {
            var aggregatedItems = AggregateItems(targetLang);
            if (aggregatedItems.Count == 0) return;

            string exportText = "";
            foreach (var item in aggregatedItems)
            {
                exportText += $"{item.Value.Name} x{item.Value.Count}\n";
            }
            ImGui.SetClipboardText(exportText);
            Log(Lang.GetText("List copied to clipboard! Paste it into Teamcraft."));
        }

        private void ExportToCSV(Dalamud.Game.ClientLanguage targetLang)
        {
            var aggregatedItems = AggregateItems(targetLang);
            if (aggregatedItems.Count == 0) return;

            // --- 🔧 已修复：完全本地化的 CSV 表头 ---
            string headerName = Lang.GetText("Item Name");
            string headerCount = Lang.GetText("Quantity");
            string headerDye = Lang.GetText("Dye/Stain");
            string noneText = Lang.GetText("None");

            string exportText = $"{headerName},{headerCount},{headerDye}\n";

            foreach (var item in aggregatedItems)
            {
                string dyeName = item.Value.Dye == "" ? noneText : item.Value.Dye;
                exportText += $"{item.Value.Name},{item.Value.Count},{dyeName}\n";
            }

            ImGui.SetClipboardText(exportText);
            Log(Lang.GetText("CSV copied to clipboard! Paste it into Excel."));
        }

        private Dictionary<string, (string Name, int Count, string Dye)> AggregateItems(Dalamud.Game.ClientLanguage targetLang)
        {
            var results = new Dictionary<string, (string Name, int Count, string Dye)>();
            var allItems = Plugin.InteriorItemList.Concat(Plugin.ExteriorItemList).ToList();

            var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>(targetLang);
            var stainSheet = DalamudApi.DataManager.GetExcelSheet<Stain>(targetLang);

            foreach (var housingItem in allItems)
            {
                if (!itemSheet.HasRow(housingItem.ItemKey)) continue;
                var itemRow = itemSheet.GetRow(housingItem.ItemKey);

                string itemName = itemRow.Name.ToString();

                string dyeName = "";
                if (housingItem.Stain != 0 && stainSheet.HasRow(housingItem.Stain))
                {
                    var stainRow = stainSheet.GetRow(housingItem.Stain);
                    dyeName = stainRow.Name.ToString();
                }

                string uniqueKey = $"{itemName}_{dyeName}";

                if (results.ContainsKey(uniqueKey))
                {
                    results[uniqueKey] = (itemName, results[uniqueKey].Count + 1, dyeName);
                }
                else
                {
                    results[uniqueKey] = (itemName, 1, dyeName);
                }
            }
            return results;
        }

        #endregion

    }
}