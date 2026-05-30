using System;
using System.Numerics;
using Dalamud.Bindings.ImGui;
using Dalamud.Bindings.ImGuizmo;
using Dalamud.Interface.Utility;
using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;

namespace AlmondHousing
{
    public static class AdvancedTuningUI
    {
        private static ActionSnapshot currentAction = null;
        private static bool wasDraggingGizmo = false;

        private static readonly Vector4 RED = new Vector4(1.0f, 0.3f, 0.3f, 1.0f);
        private static readonly Vector4 GREEN = new Vector4(0.3f, 1.0f, 0.3f, 1.0f);
        private static readonly Vector4 BLUE = new Vector4(0.3f, 0.6f, 1.0f, 1.0f);
        private static readonly Vector4 ACCENT_COLOR = new Vector4(0.9f, 0.7f, 0.3f, 1.0f);

        public static unsafe void Draw()
        {
            var plugin = AlmondHousing.Instance;
            if (plugin == null || !plugin.Config.IsActivated || !AlmondHousing.UseEmbeddedBDTH) return;

            var mem = Memory.Instance;
            if (mem == null || !mem.CanEditItem()) return;

            var activeItem = mem.HousingStructure->ActiveItem;
            if (activeItem == null) return;

            var io = ImGui.GetIO();
            if (io.KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.Z) && !io.WantTextInput) { TuningManager.Undo(); }
            if (io.KeyCtrl && ImGui.IsKeyPressed(ImGuiKey.Y) && !io.WantTextInput) { TuningManager.Redo(); }

            DrawGizmoSection(plugin, mem, activeItem, io);
ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 8.0f);
            ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0.05f, 0.05f, 0.05f, 0.95f)); 
            ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0.15f, 0.15f, 0.15f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, new Vector4(0.22f, 0.22f, 0.22f, 0.8f));
            ImGui.PushStyleColor(ImGuiCol.FrameBgActive, new Vector4(0.28f, 0.28f, 0.28f, 1.0f));
            
            if (ImGui.Begin("AlmondTuningPanel", ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoDocking))
            {
                ImGui.PushFont(UiBuilder.IconFont); 
                ImGui.TextColored(ACCENT_COLOR, FontAwesomeIcon.Crosshairs.ToIconString()); 
                ImGui.PopFont(); 
                ImGui.SameLine();
                string title = AlmondHousing.Lang.GetText("QuantumTuningTitle");
                ImGui.TextColored(ACCENT_COLOR, title);

                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                ImGui.TextDisabled(FontAwesomeIcon.QuestionCircle.ToIconString());
                ImGui.PopFont();
                if (ImGui.IsItemHovered())
                {
                    string helpText = AlmondHousing.Lang.GetText("QuantumTuningHelp");
                    ImGui.SetTooltip(helpText);
                }

                ImGui.Separator();
                ImGui.Dummy(new Vector2(0, 3));
                
                float drag = plugin.Config.Drag > 0 ? plugin.Config.Drag : 0.05f;
                Vector3 pos = activeItem->Position;
                bool changed = false;

                void DrawCoord(string label, Vector4 col, ref float val)
                {
                    ImGui.TextColored(col, label); ImGui.SameLine(30f); 
                    ImGui.PushItemWidth(160f); 
                    
                    if (ImGui.DragFloat($"##drag_{label}", ref val, drag, 0, 0, "%.3f")) changed = true;
                    
                    if (ImGui.IsItemActivated()) {
                        currentAction = new ActionSnapshot();
                        currentAction.Changes.Add(new TransformChange { ItemPtr = (nint)activeItem, OldPos = activeItem->Position });
                        foreach(var ptr in TuningManager.GroupedItems) {
                            if (ptr != (nint)activeItem) currentAction.Changes.Add(new TransformChange { ItemPtr = ptr, OldPos = *(Vector3*)(ptr + Constants.Offsets.ItemActivePosition) });
                        }
                    }
                    if (ImGui.IsItemDeactivatedAfterEdit() && currentAction != null) {
                        foreach(var c in currentAction.Changes) c.NewPos = *(Vector3*)(c.ItemPtr + Constants.Offsets.ItemActivePosition);
                        TuningManager.PushUndo(currentAction); 
                        currentAction = null;
                    }
                    ImGui.PopItemWidth();
                }

                DrawCoord("X", RED, ref pos.X); ImGui.Dummy(new Vector2(0, 2)); 
                DrawCoord("Y", GREEN, ref pos.Y); ImGui.Dummy(new Vector2(0, 2));
                DrawCoord("Z", BLUE, ref pos.Z);

                if (changed)
                {
                    Vector3 diff = pos - activeItem->Position;
                    if (plugin.Config.AbsoluteSnap && drag > 0)
                    {
                        pos.X = MathF.Round(pos.X / drag) * drag;
                        pos.Y = MathF.Round(pos.Y / drag) * drag;
                        pos.Z = MathF.Round(pos.Z / drag) * drag;
                        diff = pos - activeItem->Position;
                    }

                    activeItem->Position = pos;
                    mem.HousingLayoutModelUpdate?.Invoke((nint)activeItem + Constants.Offsets.ItemModelRefresh);

                    foreach(var ptr in TuningManager.GroupedItems) {
                        if (ptr != (nint)activeItem) {
                            *(Vector3*)(ptr + Constants.Offsets.ItemActivePosition) += diff;
                            mem.HousingLayoutModelUpdate?.Invoke(ptr + Constants.Offsets.ItemModelRefresh);
                        }
                    }
                }

                ImGui.Dummy(new Vector2(0, 5));
                ImGui.Separator();
                ImGui.Dummy(new Vector2(0, 3));
                
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.20f, 0.20f, 0.20f, 1f));
                ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.35f, 0.35f, 0.35f, 1f));
                ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.45f, 0.45f, 0.45f, 1f));

                ImGui.PushFont(UiBuilder.IconFont); ImGui.TextColored(ACCENT_COLOR, FontAwesomeIcon.Magnet.ToIconString()); ImGui.PopFont(); ImGui.SameLine();
                bool absSnap = plugin.Config.AbsoluteSnap;
                string tAbsSnap = AlmondHousing.Lang.GetText("AbsoluteSnap");
                if (ImGui.Checkbox(tAbsSnap, ref absSnap)) { plugin.Config.AbsoluteSnap = absSnap; plugin.Config.Save(); }
                if (ImGui.IsItemHovered())
                {
                    string tAbsHelp = AlmondHousing.Lang.GetText("AbsoluteSnapHelp");
                    ImGui.SetTooltip(tAbsHelp);
                }

                ImGui.Dummy(new Vector2(0, 2));

                ImGui.PushFont(UiBuilder.IconFont); ImGui.TextColored(ACCENT_COLOR, FontAwesomeIcon.Link.ToIconString()); ImGui.PopFont(); ImGui.SameLine();
                string tAddGrp = AlmondHousing.Lang.GetText("AddGroup");
                if (ImGui.Button(tAddGrp)) { TuningManager.GroupedItems.Add((nint)activeItem); }
                ImGui.SameLine();
                string tClrGrp = AlmondHousing.Lang.GetText("ClearGroup");
                if (ImGui.Button(tClrGrp)) { TuningManager.ClearGroup(); }
                ImGui.SameLine();
                string tGrpCnt = AlmondHousing.Lang.GetText("GroupCount");
                ImGui.TextDisabled($"{(tGrpCnt)}: {TuningManager.GroupedItems.Count}");

                ImGui.Dummy(new Vector2(0, 2));

                ImGui.PushFont(UiBuilder.IconFont); ImGui.TextColored(ACCENT_COLOR, FontAwesomeIcon.History.ToIconString()); ImGui.PopFont(); ImGui.SameLine();
                string tUndo = AlmondHousing.Lang.GetText("UndoBtn");
                if (ImGui.Button(tUndo)) { TuningManager.Undo(); }
                ImGui.SameLine();
                string tRedo = AlmondHousing.Lang.GetText("RedoBtn");
                if (ImGui.Button(tRedo)) { TuningManager.Redo(); }
                ImGui.SameLine();
                string tUndoHelp = AlmondHousing.Lang.GetText("UndoHelp");
                ImGui.TextDisabled(tUndoHelp);


            }
            ImGui.End();
            ImGui.PopStyleColor(4); 
            ImGui.PopStyleVar(2);
        }

        private static unsafe void DrawGizmoSection(AlmondHousing plugin, Memory mem, HousingItemStruct* activeItem, ImGuiIOPtr io)
        {
            if (!plugin.Config.UseGizmo) return;
            var sceneCam = Memory.Camera;
            if (sceneCam == null || sceneCam->RenderCamera == null) return;
            
            var renderCam = sceneCam->RenderCamera;
            var view = sceneCam->ViewMatrix;
            var proj = renderCam->ProjectionMatrix;

            var far = renderCam->FarPlane;
            var near = renderCam->NearPlane;
            var clip = far / (far - near);
            proj.M43 = -(clip * near);
            proj.M33 = -((far + near) / (far - near));
            view.M44 = 1.0f;

            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            
            if (ImGui.Begin("AlmondGizmo", ImGuiWindowFlags.NoBackground | ImGuiWindowFlags.NoDocking | ImGuiWindowFlags.NoNavFocus | ImGuiWindowFlags.NoNavInputs | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoInputs))
            {
                ImGui.SetWindowSize(io.DisplaySize);
                ImGuizmo.BeginFrame();
                ImGuizmo.SetDrawlist();
                ImGuizmo.Enable(mem.HousingStructure->Rotating);
                ImGuizmo.SetID(1337);
                ImGuizmo.SetOrthographic(false);
                ImGuizmo.SetRect(0, 0, io.DisplaySize.X, io.DisplaySize.Y);

                Vector3 translate = activeItem->Position;
                Vector3 rot = Vector3.Zero;
                Vector3 scale2 = Vector3.One;
                Matrix4x4 matrix = Matrix4x4.Identity;
                
                ImGuizmo.RecomposeMatrixFromComponents(ref translate.X, ref rot.X, ref scale2.X, ref matrix.M11);

                float snapVal = plugin.Config.DoSnap ? plugin.Config.Drag : 0f;
                Vector3 snap = new Vector3(snapVal, snapVal, snapVal);
                Matrix4x4 deltaMatrix = Matrix4x4.Identity;

                bool isDraggingGizmo = ImGuizmo.IsUsing();
                if (isDraggingGizmo && !wasDraggingGizmo) {
                    currentAction = new ActionSnapshot();
                    currentAction.Changes.Add(new TransformChange { ItemPtr = (nint)activeItem, OldPos = activeItem->Position });
                    foreach(var ptr in TuningManager.GroupedItems) {
                        if (ptr != (nint)activeItem) currentAction.Changes.Add(new TransformChange { ItemPtr = ptr, OldPos = *(Vector3*)(ptr + Constants.Offsets.ItemActivePosition) });
                    }
                }

                if (ImGuizmo.Manipulate(ref view.M11, ref proj.M11, ImGuizmoOperation.Translate, ImGuizmoMode.World, ref matrix.M11, ref deltaMatrix.M11, ref snap.X))
                {
                    ImGuizmo.DecomposeMatrixToComponents(ref matrix.M11, ref translate.X, ref rot.X, ref scale2.X);
                    
                    if (plugin.Config.AbsoluteSnap && snapVal > 0)
                    {
                        translate.X = MathF.Round(translate.X / snapVal) * snapVal;
                        translate.Y = MathF.Round(translate.Y / snapVal) * snapVal;
                        translate.Z = MathF.Round(translate.Z / snapVal) * snapVal;
                    }

                    Vector3 diff = translate - activeItem->Position;
                    activeItem->Position = translate;
                    mem.HousingLayoutModelUpdate?.Invoke((nint)activeItem + Constants.Offsets.ItemModelRefresh);

                    foreach(var ptr in TuningManager.GroupedItems) {
                        if (ptr != (nint)activeItem) {
                            *(Vector3*)(ptr + Constants.Offsets.ItemActivePosition) += diff;
                            mem.HousingLayoutModelUpdate?.Invoke(ptr + Constants.Offsets.ItemModelRefresh);
                        }
                    }
                }

                if (!isDraggingGizmo && wasDraggingGizmo && currentAction != null) {
                    foreach(var c in currentAction.Changes) c.NewPos = *(Vector3*)(c.ItemPtr + Constants.Offsets.ItemActivePosition);
                    TuningManager.PushUndo(currentAction);
                    currentAction = null;
                }
                ImGui.End();
            }
            ImGui.PopStyleVar();
        }

    }
}