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
using Dalamud.Interface;

namespace AlmondHousing.Gui
{
    public class ConfigurationWindow : Window<AlmondHousing>
    {
        public Configuration Config => Plugin.Config;

        private string CustomTag = string.Empty;
        private readonly Dictionary<uint, uint> iconToFurniture = new() { };

        // === 🎨 现代感配色：深空灰 + 杏仁金 ===
        private readonly Vector4 THEME_BASE = new(0.20f, 0.20f, 0.20f, 1f);     
        private readonly Vector4 THEME_HOVER = new(0.35f, 0.35f, 0.35f, 1f);    
        private readonly Vector4 THEME_ACTIVE = new(0.45f, 0.45f, 0.45f, 1f);   
        private readonly Vector4 THEME_HEADER = new(0.15f, 0.15f, 0.15f, 0.8f);
        private readonly Vector4 ACCENT_COLOR = new(0.9f, 0.7f, 0.3f, 1.0f); // 杏仁金

        private FileDialogManager FileDialogManager { get; }
        private Dalamud.Game.ClientLanguage currentExportLang = Dalamud.Game.ClientLanguage.English;
        private string searchQuery = string.Empty;

        // === 🧭 导航控制 ===
        private int selectedTab = 0;

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

            // --- 🚀 动态计算侧边栏宽度，完美适应多语言超长文本 ---
            float maxTextWidth = ImGui.CalcTextSize(Lang.GetText("Plugin Interface Language / 插件语言")).X;
            maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Interior Furniture")).X);
            maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Interior Fixtures")).X);
            maxTextWidth = Math.Max(maxTextWidth, ImGui.CalcTextSize(Lang.GetText("Layout")).X);

            float sidebarWidth = maxTextWidth + 40f; 
            if (sidebarWidth < 180f) sidebarWidth = 180f; 

            // 1. 左侧：导航面板
            ImGui.BeginChild("AlmondSidebar", new Vector2(sidebarWidth, 0), true);
            {
                DrawSidebarItem(Lang.GetText("Interior Furniture"), 0);
                DrawSidebarItem(Lang.GetText("Interior Fixtures"), 1);
                DrawSidebarItem(Lang.GetText("Layout"), 2);
                DrawSidebarItem(Lang.GetText("Plugin Interface Language / 插件语言"), 3);
            }
            ImGui.EndChild();

            ImGui.SameLine();

            // 2. 右侧：内容区域
            ImGui.BeginChild("AlmondContent", new Vector2(0, 0), false);
            {
                switch (selectedTab)
                {
                    case 0: DrawFurnitureTab(); break;
                    case 1: DrawFixtureTab(); break;
                    case 2: DrawLayoutFileTab(); break;
                    case 3: DrawSettingsTab(); break;
                }
            }
            ImGui.EndChild();

            this.FileDialogManager.Draw();
        }

        private void DrawSidebarItem(string label, int index)
        {
            bool isSelected = selectedTab == index;
            float height = 32f;
            Vector2 startPos = ImGui.GetCursorPos();

            // 1. 绘制底层的 Selectable（透明点击栏）
            if (ImGui.Selectable($"##{index}_{label}", isSelected, ImGuiSelectableFlags.None, new Vector2(0, height)))
            {
                selectedTab = index;
            }
            Vector2 endPos = ImGui.GetCursorPos();

            // 2. 绘制选中状态的高亮强调线
            if (isSelected) 
            {
                var min = ImGui.GetItemRectMin();
                var max = ImGui.GetItemRectMax();
                ImGui.GetWindowDrawList().AddLine(
                    new Vector2(min.X + 2, min.Y + 4), 
                    new Vector2(min.X + 2, max.Y - 4), 
                    ImGui.ColorConvertFloat4ToU32(ACCENT_COLOR), 
                    4.0f);
            }

            // 3. 覆盖绘制文字：固定向右偏移 15 像素，实现完美对齐
            if (isSelected) ImGui.PushStyleColor(ImGuiCol.Text, ACCENT_COLOR);
            ImGui.SetCursorPos(new Vector2(startPos.X + 15, startPos.Y + (height - ImGui.GetTextLineHeight()) / 2));
            ImGui.Text(label);
            if (isSelected) ImGui.PopStyleColor();

            ImGui.SetCursorPos(endPos);
            ImGui.Spacing();
        }

        #region 选项卡内容渲染区

        private void DrawFurnitureTab()
        {
            ImGui.SetNextItemWidth(-1);
            ImGui.InputTextWithHint("##SearchBox", Lang.GetText("Search furniture name..."), ref searchQuery, 256);
            ImGui.Dummy(new Vector2(0, 5));

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
            if (ImGui.CollapsingHeader(Lang.GetText("Unused Furniture"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.PushID("unused");
                DrawItemList(Plugin.UnusedItemList, true);
                ImGui.PopID();
            }

            ImGui.PopStyleColor(3);
        }

        private void DrawFixtureTab()
        {
            ImGui.PushStyleColor(ImGuiCol.Header, THEME_HEADER);
            if (ImGui.CollapsingHeader(Lang.GetText("Interior Fixtures"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawFixtureList(Plugin.Layout.interiorFixture);
            }
            if (ImGui.CollapsingHeader(Lang.GetText("Exterior Fixtures"), ImGuiTreeNodeFlags.DefaultOpen))
            {
                DrawFixtureList(Plugin.Layout.exteriorFixture);
            }
            ImGui.PopStyleColor();
        }

        private void DrawLayoutFileTab()
        {
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Layout"));
            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));

            if (!Config.SaveLocation.IsNullOrEmpty())
            {
                ImGui.TextWrapped($"{Lang.GetText("Current file location:")}\n{Config.SaveLocation}");
                ImGui.Dummy(new Vector2(0, 5));
                if (ImGui.Button(Lang.GetText("Save"), new Vector2(100, 30))) SaveLayoutToFile();
                ImGui.SameLine();
                if (ImGui.Button(Lang.GetText("Load"), new Vector2(100, 30))) { CreateAutoBackup(); LoadLayoutFromFile(); }
            }

            if (ImGui.Button(Lang.GetText("Save As"), new Vector2(100, 30))) ShowSaveDialog();
            ImGui.SameLine();
            if (ImGui.Button(Lang.GetText("Load From"), new Vector2(100, 30))) ShowLoadDialog();

            ImGui.Dummy(new Vector2(0, 15));
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Export Shopping List (导出购物清单)"));
            ImGui.Separator();

            ImGui.SetNextItemWidth(120);
            if (ImGui.BeginCombo("##ExportLang", currentExportLang.ToString()))
            {
                foreach (var lang in new[] { Dalamud.Game.ClientLanguage.English, Dalamud.Game.ClientLanguage.Japanese, Dalamud.Game.ClientLanguage.German, Dalamud.Game.ClientLanguage.French })
                {
                    if (ImGui.Selectable(lang.ToString(), currentExportLang == lang)) currentExportLang = lang;
                }
                ImGui.EndCombo();
            }
            ImGui.SameLine();
            if (ImGui.Button(Lang.GetText("Export for Teamcraft"))) ExportToTeamcraft(currentExportLang);
            ImGui.SameLine();
            if (ImGui.Button(Lang.GetText("Export to CSV"))) ExportToCSV(currentExportLang);
        }

        private void DrawSettingsTab()
        {
            ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1.0f, 0.3f, 0.3f, 1.0f)); 
            ImGui.TextWrapped(Lang.GetText("Anti-Resell Warning"));
            ImGui.PopStyleColor(); 
            ImGui.Dummy(new Vector2(0, 10));

            string[] supportedLangNames = { "简体中文", "繁體中文", "English", "日本語", "한국어", "Deutsch", "Français" };
            string[] supportedLangs = { "zh", "zh_TW", "en", "ja", "ko", "de", "fr" };
            int currentLangIndex = Math.Max(0, Array.IndexOf(supportedLangs, Config.UILanguage));

            ImGui.Text(Lang.GetText("Plugin Interface Language / 插件语言"));
            ImGui.SetNextItemWidth(150);
            if (ImGui.BeginCombo("##UILang", supportedLangNames[currentLangIndex]))
            {
                for (int i = 0; i < supportedLangs.Length; i++)
                {
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

            if (ImGui.Checkbox(Lang.GetText("Apply Layout"), ref Config.ApplyLayout)) Config.Save();
            if (ImGui.Checkbox(Lang.GetText("Label Furniture"), ref Config.DrawScreen)) Config.Save();
            if (ImGui.Checkbox(Lang.GetText("Show Tooltips"), ref Config.ShowTooltips)) Config.Save();

            ImGui.SetNextItemWidth(100);
            if (ImGui.InputInt(Lang.GetText("Placement Interval (ms)"), ref Config.LoadInterval)) Config.Save();

            if (Memory.Instance != null && Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors && Memory.Instance.GetIndoorHouseSize() != "Apartment")
            {
                ImGui.Dummy(new Vector2(0, 5));
                ImGui.Text(Lang.GetText("Selected Floors"));
                if (ImGui.Checkbox(Lang.GetText("Basement"), ref Config.Basement)) Config.Save();
                ImGui.SameLine();
                if (ImGui.Checkbox(Lang.GetText("Ground Floor"), ref Config.GroundFloor)) Config.Save();
                ImGui.SameLine();
                if (Memory.Instance.HasUpperFloor() && ImGui.Checkbox(Lang.GetText("Upper Floor"), ref Config.UpperFloor)) Config.Save();
            }
        }

        #endregion

        protected override void DrawUi()
        {
            ImGui.PushStyleColor(ImGuiCol.TitleBgActive, THEME_BASE);
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, THEME_HOVER);
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, THEME_ACTIVE);
            ImGui.SetNextWindowSize(new Vector2(800, 550), ImGuiCond.FirstUseEver); 
            DrawAllUi();
            ImGui.PopStyleColor(3);
            ImGui.End();
        }

        #region 核心功能逻辑区

        public static void DrawIcon(ushort icon, Vector2 size)
        {
            if (icon < 65000)
            {
                var iconTexture = DalamudApi.TextureProvider.GetFromGameIcon(new GameIconLookup(icon));
                ImGui.Image(iconTexture.GetWrapOrEmpty().Handle, size);
            }
        }

        private bool CheckModeForSave() => true;

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
                LogError(Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Island ? Lang.GetText("(Manage Furnishings -> Place Furnishing Glamours)") : Lang.GetText("(Housing -> Indoor/Outdoor Furnishings)"));
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
                string backupFileName = $"AutoBackup_{DateTime.Now:yyyyMMdd_HHmmss}.json";
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
            FileDialogManager.SaveFileDialog(Lang.GetText("Select a Save Location"), ".json", saveName, "json", (bool ok, string res) =>
            {
                if (!ok) return;
                Config.SaveLocation = res; Config.Save(); SaveLayoutToFile();
            }, Path.GetDirectoryName(Config.SaveLocation));
        }

        private void ShowLoadDialog()
        {
            FileDialogManager.OpenFileDialog(Lang.GetText("Select a Layout File"), ".json", (bool ok, List<string> res) =>
            {
                if (!ok) return;
                CreateAutoBackup(); Config.SaveLocation = res.FirstOrDefault(""); Config.Save(); LoadLayoutFromFile();
            }, 1, Path.GetDirectoryName(Config.SaveLocation));
        }

        private void SaveLayoutToFile()
        {
            try { Plugin.GetGameLayout(); AlmondHousing.LayoutManager.ExportLayout(); }
            catch (Exception e) { LogError(string.Format(Lang.GetText("Save Error: {0}"), e.Message), e.StackTrace ?? ""); }
        }

        private void LoadLayoutFromFile()
        {
            if (!CheckModeForLoad()) return;
            try { 
                SaveLayoutManager.ImportLayout(Config.SaveLocation); 
                Log(string.Format(Lang.GetText("Imported {0} items"), Plugin.InteriorItemList.Count + Plugin.ExteriorItemList.Count));
                Plugin.MatchLayout(); 
                Config.ResetRecord(); 
                if (Config.ApplyLayout) Plugin.ApplyLayout(); 
            }
            catch (Exception e) { LogError(string.Format(Lang.GetText("Load Error: {0}"), e.Message), e.StackTrace ?? ""); }
        }

        private void DrawFixtureList(List<Fixture> fixtureList)
        {
            if (ImGui.Button(Lang.GetText("Clear") + "##Fixture")) { fixtureList.Clear(); Config.Save(); }
            ImGui.Columns(3, "FixtureList", true);
            ImGui.Separator();
            ImGui.Text(Lang.GetText("Level")); ImGui.NextColumn();
            ImGui.Text(Lang.GetText("Fixture")); ImGui.NextColumn();
            ImGui.Text(Lang.GetText("Item")); ImGui.NextColumn();
            ImGui.Separator();
            var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>();
            foreach (var fixture in fixtureList)
            {
                ImGui.Text(Lang.GetText(fixture.level ?? "")); ImGui.NextColumn();
                ImGui.Text(Lang.GetText(fixture.type ?? "")); ImGui.NextColumn();
                if (itemSheet.HasRow(fixture.itemId)) { DrawIcon(itemSheet.GetRow(fixture.itemId).Icon, new Vector2(20, 20)); ImGui.SameLine(); }
                ImGui.Text(fixture.name); ImGui.NextColumn();
                ImGui.Separator();
            }
            ImGui.Columns(1);
        }

        private void DrawItemList(List<HousingItem> itemList, bool isUnused = false)
        {
            var filteredList = itemList.Where(x => string.IsNullOrEmpty(searchQuery) || x.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            if (ImGui.Button(Lang.GetText("Sort")))
            {
                itemList.Sort((x, y) => {
                    if (x.Name.CompareTo(y.Name) != 0) return x.Name.CompareTo(y.Name);
                    return x.X.CompareTo(y.X);
                });
                Config.Save();
            }
            ImGui.SameLine();
            if (ImGui.Button(Lang.GetText("Clear") + "##ItemClear")) { itemList.Clear(); Config.Save(); }

            var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>();
            var groupedItems = filteredList.GroupBy(housingItem => {
                if (itemSheet.HasRow(housingItem.ItemKey)) {
                    string catName = itemSheet.GetRow(housingItem.ItemKey).ItemUICategory.Value.Name.ToString();
                    return string.IsNullOrEmpty(catName) ? Lang.GetText("Unknown") : catName;
                }
                return Lang.GetText("Unknown");
            }).OrderBy(g => g.Key).ToList();

            foreach (var group in groupedItems) {
                ImGui.PushStyleColor(ImGuiCol.Text, ACCENT_COLOR);
                if (ImGui.TreeNodeEx($"{group.Key} ({group.Count()})###cat_{group.Key}_{isUnused}", ImGuiTreeNodeFlags.DefaultOpen)) {
                    ImGui.PopStyleColor();
                    ImGui.Columns(isUnused ? 4 : 5, $"Table_{group.Key}", true);
                    ImGui.Separator();
                    ImGui.Text(Lang.GetText("Item")); ImGui.NextColumn();
                    ImGui.Text(Lang.GetText("Position (X,Y,Z)")); ImGui.NextColumn();
                    ImGui.Text(Lang.GetText("Rotation")); ImGui.NextColumn();
                    ImGui.Text(Lang.GetText("Dye/Material")); ImGui.NextColumn();
                    if (!isUnused) { ImGui.Text(Lang.GetText("Set Position")); ImGui.NextColumn(); }
                    ImGui.Separator();
                    foreach (var housingItem in group) {
                        int originalIndex = itemList.IndexOf(housingItem);
                        if (itemSheet.HasRow(housingItem.ItemKey)) { DrawIcon(itemSheet.GetRow(housingItem.ItemKey).Icon, new Vector2(20, 20)); ImGui.SameLine(); }
                        if (housingItem.ItemStruct == IntPtr.Zero) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                        ImGui.Text(housingItem.Name); ImGui.NextColumn();
                        DrawRow(originalIndex, housingItem, !isUnused);
                        if (housingItem.ItemStruct == IntPtr.Zero) ImGui.PopStyleColor();
                        ImGui.Separator();
                    }
                    ImGui.Columns(1); ImGui.TreePop();
                } else ImGui.PopStyleColor();
            }
        }

        private void DrawRow(int i, HousingItem housingItem, bool showSetPosition = true)
        {
            if (!housingItem.CorrectLocation) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
            ImGui.Text($"{housingItem.X:N4}, {housingItem.Y:N4}, {housingItem.Z:N4}"); 
            if (!housingItem.CorrectLocation) ImGui.PopStyleColor();
            ImGui.NextColumn();
            if (!housingItem.CorrectRotation) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
            ImGui.Text($"{housingItem.Rotate:N3}"); 
            if (!housingItem.CorrectRotation) ImGui.PopStyleColor();
            ImGui.NextColumn();
            var stainSheet = DalamudApi.DataManager.GetExcelSheet<Stain>();
            if (housingItem.Stain != 0 && stainSheet.HasRow(housingItem.Stain)) {
                Utils.StainButton("dye_" + i, stainSheet.GetRow(housingItem.Stain), new Vector2(20)); ImGui.SameLine();
                if (!housingItem.DyeMatch) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                ImGui.Text(stainSheet.GetRow(housingItem.Stain).Name.ToString());
                if (!housingItem.DyeMatch) ImGui.PopStyleColor();
            } else if (housingItem.MaterialItemKey != 0) {
                var it = DalamudApi.DataManager.GetExcelSheet<Item>();
                if (it.HasRow(housingItem.MaterialItemKey)) { 
                    if (!housingItem.DyeMatch) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                    DrawIcon(it.GetRow(housingItem.MaterialItemKey).Icon, new Vector2(20, 20)); ImGui.SameLine(); 
                    ImGui.Text(it.GetRow(housingItem.MaterialItemKey).Name.ToString()); 
                    if (!housingItem.DyeMatch) ImGui.PopStyleColor();
                }
            }
            ImGui.NextColumn();
            if (showSetPosition) {
                if (housingItem.ItemStruct != IntPtr.Zero) {
                    if (ImGui.Button(Lang.GetText("Set") + "##" + i)) { Plugin.MatchLayout(); SetItemPosition(housingItem); }
                }
                ImGui.NextColumn();
            }
        }

        protected override void DrawScreen() { if (Config.DrawScreen) DrawItemOnScreen(); }

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
                if (Config.DrawDistance > 0 && (playerPos - itemPos).Length() > Config.DrawDistance) continue;
                if (DalamudApi.GameGui.WorldToScreen(itemPos, out var screenCoords))
                {
                    ImGui.PushID("HousingItemWindow" + i);
                    ImGui.SetNextWindowPos(new Vector2(screenCoords.X, screenCoords.Y));
                    ImGui.SetNextWindowBgAlpha(0.8f);
                    if (ImGui.Begin("HousingItem" + i, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav))
                    {
                        ImGui.Text(housingItem.Name); ImGui.SameLine();
                        if (ImGui.Button(Lang.GetText("Set") + "##ScreenItem" + i.ToString()))
                        {
                            if (!Memory.Instance.CanEditItem()) continue;
                            SetItemPosition(housingItem); Config.HiddenScreenItemHistory.Add(i); Config.Save();
                        }
                        ImGui.End();
                    }
                    ImGui.PopID();
                }
            }
        }

        private void ExportToTeamcraft(Dalamud.Game.ClientLanguage targetLang)
        {
            var aggregatedItems = AggregateItems(targetLang);
            if (aggregatedItems.Count == 0) return;
            string exportText = aggregatedItems.Aggregate("", (current, item) => current + $"{item.Value.Name} x{item.Value.Count}\n");
            ImGui.SetClipboardText(exportText); Log(Lang.GetText("List copied to clipboard! Paste it into Teamcraft."));
        }

        private void ExportToCSV(Dalamud.Game.ClientLanguage targetLang)
        {
            var aggregatedItems = AggregateItems(targetLang);
            if (aggregatedItems.Count == 0) return;
            string exportText = $"{Lang.GetText("Item ID")},{Lang.GetText("Item Name")},{Lang.GetText("Quantity")},{Lang.GetText("Dye/Stain")}\n" + aggregatedItems.Aggregate("", (current, item) => current + $"{item.Value.ItemId},{item.Value.Name},{item.Value.Count},{(item.Value.Dye == "" ? Lang.GetText("None") : item.Value.Dye)}\n");
            ImGui.SetClipboardText(exportText); Log(Lang.GetText("CSV copied to clipboard! Paste it into Excel."));
        }

        private Dictionary<string, (uint ItemId, string Name, int Count, string Dye)> AggregateItems(Dalamud.Game.ClientLanguage targetLang)
        {
            var results = new Dictionary<string, (uint ItemId, string Name, int Count, string Dye)>();
            var allItems = Plugin.InteriorItemList.Concat(Plugin.ExteriorItemList).ToList();
            var itemSheet = DalamudApi.DataManager.GetExcelSheet<Item>(targetLang);
            var stainSheet = DalamudApi.DataManager.GetExcelSheet<Stain>(targetLang);
            foreach (var housingItem in allItems)
            {
                if (!itemSheet.HasRow(housingItem.ItemKey)) continue;
                var itemRow = itemSheet.GetRow(housingItem.ItemKey);
                string dyeName = (housingItem.Stain != 0 && stainSheet.HasRow(housingItem.Stain)) ? stainSheet.GetRow(housingItem.Stain).Name.ToString() : "";
                string uniqueKey = $"{itemRow.RowId}_{dyeName}";
                if (results.ContainsKey(uniqueKey)) results[uniqueKey] = (itemRow.RowId, itemRow.Name.ToString(), results[uniqueKey].Count + 1, dyeName);
                else results[uniqueKey] = (itemRow.RowId, itemRow.Name.ToString(), 1, dyeName);
            }
            return results;
        }

        #endregion
    }
}