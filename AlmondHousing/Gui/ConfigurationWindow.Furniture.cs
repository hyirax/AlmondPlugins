using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Lumina.Excel.Sheets;
using AlmondHousing.Objects;
using AlmondHousing.Util;
using static AlmondHousing.AlmondHousing;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;

namespace AlmondHousing.Gui
{
    public partial class ConfigurationWindow
    {
private int _prevItemCount = -1;
        private float _countPulseAnim = 0f;

        private void DrawFurnitureTab()
        {
            DrawInlineIconColored(FontAwesomeIcon.Search, ACCENT_COLOR); ImGui.SameLine();
            ImGui.SetNextItemWidth(-1);
            ImGui.InputTextWithHint("##SearchBox", Lang.GetText("Search furniture name..."), ref searchQuery, 256);
            ImGui.Dummy(new Vector2(0, 5));

            DrawInlineIconColored(FontAwesomeIcon.Filter, ACCENT_COLOR); ImGui.SameLine();
            ImGui.TextColored(ACCENT_COLOR, Lang.GetText("Smart Filter:"));
            ImGui.SameLine();

            DrawInlineIconColored(FontAwesomeIcon.ExclamationTriangle, ACCENT_COLOR); ImGui.SameLine();
            ImGui.Checkbox(Lang.GetText("Only show misplaced furniture"), ref showOnlyMisplaced);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Check to show only items that are not in the correct position or rotation."));

            ImGui.SameLine();
            ImGui.Spacing();
            ImGui.SameLine();

            DrawInlineIconColored(FontAwesomeIcon.ShoppingCart, ACCENT_COLOR); ImGui.SameLine();
            ImGui.Checkbox(Lang.GetText("Only show missing furniture"), ref showOnlyMissing);
            if (ImGui.IsItemHovered()) ImGui.SetTooltip(Lang.GetText("Check to show only items that are missing from your inventory."));

            ImGui.Separator();
            ImGui.Dummy(new Vector2(0, 5));

            CustomUI.AnimatedCollapsingHeader("##interior_furn", Lang.GetText("Interior Furniture"), ref _interiorFurnitureOpen, 300f); if (_interiorFurnitureOpen)
            {
                ImGui.PushID("interior");
                DrawItemList(Plugin.InteriorItemList);
                ImGui.PopID();
            }
            CustomUI.AnimatedCollapsingHeader("##exterior_furn", Lang.GetText("Exterior Furniture"), ref _exteriorFurnitureOpen, 200f); if (_exteriorFurnitureOpen)
            {
                ImGui.PushID("exterior");
                DrawItemList(Plugin.ExteriorItemList);
                ImGui.PopID();
            }
            CustomUI.AnimatedCollapsingHeader("##unused_furn", Lang.GetText("Unused Furniture"), ref _unusedFurnitureOpen, 150f); if (_unusedFurnitureOpen)
            {
                ImGui.PushID("unused");
                DrawItemList(Plugin.UnusedItemList, true);
                ImGui.PopID();
            }
        }

        private void DrawFixtureTab()
        {
            CustomUI.AnimatedCollapsingHeader("##interior_fix", Lang.GetText("Interior Fixtures"), ref _interiorFixturesOpen, 150f); if (_interiorFixturesOpen)
            {
                DrawFixtureList(Plugin.Layout.interiorFixture);
            }
            CustomUI.AnimatedCollapsingHeader("##exterior_fix", Lang.GetText("Exterior Fixtures"), ref _exteriorFixturesOpen, 100f); if (_exteriorFixturesOpen)
            {
                DrawFixtureList(Plugin.Layout.exteriorFixture);
            }
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
                        
                        if (showOnlyMisplaced && housingItem.CorrectLocation && housingItem.CorrectRotation) continue;
                        if (showOnlyMissing && InventoryScanner.GetOwnedCount(housingItem.ItemKey) > 0) continue;

                        int originalIndex = itemList.IndexOf(housingItem);
                        if (itemSheet.HasRow(housingItem.ItemKey)) { DrawIcon(itemSheet.GetRow(housingItem.ItemKey).Icon, new Vector2(20, 20)); ImGui.SameLine(); }
                        if (housingItem.ItemStruct == nint.Zero) ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.5f, 0.5f, 0.5f, 1));
                        ImGui.Text(housingItem.Name); ImGui.NextColumn();
                        DrawRow(originalIndex, housingItem, !isUnused);
                        if (housingItem.ItemStruct == nint.Zero) ImGui.PopStyleColor();
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
                if (housingItem.ItemStruct != nint.Zero) {
                    if (ImGui.Button(Lang.GetText("Set") + "##" + i)) { Plugin.MatchLayout(); SetItemPosition(housingItem); }
                }
                ImGui.NextColumn();
            }
        }

        public unsafe void DrawItemOnScreen()
        {
            if (!Config.DrawScreen) return;
            if (Memory.Instance == null) return;
            var itemList = Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors ? Plugin.InteriorItemList : Plugin.ExteriorItemList;
            
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 6.0f); 
            ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, 4.0f); 
            
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.02f, 0.02f, 0.02f, 0.92f)); 
            ImGui.PushStyleColor(ImGuiCol.Border, new Vector4(0.0f, 0.0f, 0.0f, 0.0f)); 
            
            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.15f, 0.15f, 0.15f, 0.6f));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.45f, 0.45f, 0.45f, 1.0f));

            for (int i = 0; i < itemList.Count; i++)
            {
                var playerPos = DalamudApi.ObjectTable.LocalPlayer.Position;
                var housingItem = itemList[i];
                if (housingItem.ItemStruct == nint.Zero) continue;
                
                var itemStruct = (HousingItemStruct*)housingItem.ItemStruct;
                var itemPos = new Vector3(itemStruct->Position.X, itemStruct->Position.Y, itemStruct->Position.Z);
                
                if (Config.HiddenScreenItemHistory.IndexOf(i) >= 0) continue;
                if (Config.DrawDistance > 0 && (playerPos - itemPos).Length() > Config.DrawDistance) continue;
                
                if (DalamudApi.GameGui.WorldToScreen(itemPos, out var screenCoords))
                {
                    if (!_stableScreenCoords.TryGetValue(i, out var stableCoords))
                    {
                        stableCoords = screenCoords;
                        _stableScreenCoords[i] = stableCoords;
                    }

                    if (Vector2.Distance(screenCoords, stableCoords) > 2.5f)
                    {
                        stableCoords = screenCoords;
                        _stableScreenCoords[i] = stableCoords;
                    }

                    float snappedX = (float)Math.Floor(stableCoords.X);
                    float snappedY = (float)Math.Floor(stableCoords.Y);
                    
                    ImGui.SetNextWindowPos(new Vector2(snappedX, snappedY));
                    
                    if (ImGui.Begin("HousingItem" + i, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoNav))
                    {
                        float distance = Vector3.Distance(playerPos, itemPos);
                        Vector4 textColor;
                        FontAwesomeIcon icon;

                        if (housingItem.CorrectLocation && housingItem.CorrectRotation)
                        {
                            textColor = new Vector4(0.5f, 0.5f, 0.5f, 0.8f); 
                            icon = FontAwesomeIcon.CheckCircle;
                        }
                        else
                        {
                            textColor = new Vector4(1.0f, 0.65f, 0.0f, 1.0f); 
                            icon = FontAwesomeIcon.Crosshairs;
                        }

                        DrawInlineIconColored(icon, textColor);
                        ImGui.SameLine();
                        ImGui.TextColored(textColor, $"{housingItem.Name} [{distance:F0}m]");
                        
                        ImGui.SameLine();
                        if (ImGui.Button(Lang.GetText("Set") + "##ScreenItem" + i.ToString()))
                        {
                            if (!Memory.Instance.CanEditItem()) continue;
                            SetItemPosition(housingItem); Config.HiddenScreenItemHistory.Add(i); Config.Save();
                        }
                        ImGui.End();
                    }
                }
            }

            ImGui.PopStyleColor(5);
            ImGui.PopStyleVar(3);
        }
    }
}
