using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using AlmondHousing.Util;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace AlmondHousing.Gui
{
    public static class CustomUI
    {
        private static Dictionary<string, float> _animStates = new Dictionary<string, float>();
        private static float _timeCounter = 0f;
        private static readonly Dictionary<string, float> _headerAnim = new();
        private const float HeaderAnimSpeed = 8.0f;

        private static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return new Vector4(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t,
                a.W + (b.W - a.W) * t
            );
        }

        public static bool AnimatedButton(string id, string text, Vector2 size, FontAwesomeIcon icon = FontAwesomeIcon.None)
        {
            if (!_animStates.ContainsKey(id)) _animStates[id] = 0f;

            var pos = ImGui.GetCursorScreenPos();
            float deltaTime = ImGui.GetIO().DeltaTime;
            _timeCounter += deltaTime;

            bool isHovered = ImGui.IsMouseHoveringRect(pos, pos + size);
            bool isActive = isHovered && ImGui.IsMouseDown(ImGuiMouseButton.Left);

            ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0,0,0,0));
            ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0,0,0,0));
            ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0,0,0,0));
            bool clicked = ImGui.Button($"###{id}", size);
            ImGui.PopStyleColor(3);

            float targetState = isHovered ? 1f : 0f;
            _animStates[id] += (targetState - _animStates[id]) * (deltaTime * 12.0f);
            float animProgress = _animStates[id]; 

            Vector4 colorNormal = Constants.Colors.ThemeBase;
            Vector4 colorHover = Constants.Colors.AccentGold; 
            Vector4 colorClick = new Vector4(1.0f, 0.85f, 0.4f, 1.0f); 
            Vector4 textColorNormal = new Vector4(0.8f, 0.8f, 0.8f, 1.0f);
            Vector4 textColorHover = new Vector4(0.1f, 0.1f, 0.1f, 1.0f); 

            Vector4 currentColor = Lerp(colorNormal, colorHover, animProgress);
            Vector4 currentTextColor = Lerp(textColorNormal, textColorHover, animProgress);

            Vector2 renderPos = pos;
            if (isActive) 
            {
                currentColor = colorClick;
                renderPos.Y += 2; 
            }

            var drawList = ImGui.GetWindowDrawList();

            if (animProgress > 0.05f)
            {
                float breathingPulse = (float)(Math.Sin(_timeCounter * 4.0f) + 1.0f) / 2.0f; 
                float glowSpread = 2.0f + (breathingPulse * 3.0f * animProgress); 
                Vector4 glowColor = new Vector4(colorHover.X, colorHover.Y, colorHover.Z, animProgress * 0.4f); 
                drawList.AddRectFilled(renderPos - new Vector2(glowSpread), renderPos + size + new Vector2(glowSpread), ImGui.ColorConvertFloat4ToU32(glowColor), 6.0f);
            }

            drawList.AddRectFilled(renderPos, renderPos + size, ImGui.ColorConvertFloat4ToU32(currentColor), 6.0f);

            Vector2 iconSize = Vector2.Zero;
            string iconString = "";
            
            if (icon != FontAwesomeIcon.None)
            {
                iconString = icon.ToIconString();
                ImGui.PushFont(UiBuilder.IconFont);
                iconSize = ImGui.CalcTextSize(iconString);
                ImGui.PopFont();
            }

            Vector2 textSize = ImGui.CalcTextSize(text);
            float spacing = icon != FontAwesomeIcon.None ? 8.0f : 0.0f;
            float totalWidth = iconSize.X + spacing + textSize.X;
            
            float startX = renderPos.X + (size.X - totalWidth) / 2;
            float iconY = renderPos.Y + (size.Y - iconSize.Y) / 2;
            float textY = renderPos.Y + (size.Y - textSize.Y) / 2;

            if (icon != FontAwesomeIcon.None)
            {
                ImGui.PushFont(UiBuilder.IconFont);
                drawList.AddText(new Vector2(startX, iconY), ImGui.ColorConvertFloat4ToU32(currentTextColor), iconString);
                ImGui.PopFont();
                startX += iconSize.X + spacing; 
            }

            drawList.AddText(new Vector2(startX, textY), ImGui.ColorConvertFloat4ToU32(currentTextColor), text);

            return clicked;
        }

        public static bool AnimatedCollapsingHeader(string id, string label, ref bool open, float estimatedHeight = 300f)
        {
            if (!_headerAnim.TryGetValue(id, out float progress))
                _headerAnim[id] = progress = open ? 1f : 0f;

            float target = open ? 1f : 0f;
            progress += (target - progress) * (ImGui.GetIO().DeltaTime * HeaderAnimSpeed);
            if (Math.Abs(progress - target) < 0.001f) progress = target;
            _headerAnim[id] = progress;

            var pos = ImGui.GetCursorScreenPos();
            var drawList = ImGui.GetWindowDrawList();
            float headerHeight = 28f;
            Vector2 headerSize = new Vector2(ImGui.GetContentRegionAvail().X, headerHeight);
            bool hovered = ImGui.IsMouseHoveringRect(pos, pos + headerSize);
            bool clicked = hovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left);

            if (clicked) open = !open;

            // Header background
            float bgAlpha = hovered ? 0.10f : 0.04f;
            drawList.AddRectFilled(pos, pos + headerSize, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, bgAlpha)), 4f);

            // Arrow: triangle that rotates from right (closed) to down (open)
            float arrowAngle = progress * (float)Math.PI * 0.5f;
            var arrowCenter = pos + new Vector2(14f, headerHeight / 2);
            float aLen = 5f;
            float cosA = (float)Math.Cos(arrowAngle);
            float sinA = (float)Math.Sin(arrowAngle);
            Vector2 Rot(Vector2 v) => new(v.X * cosA - v.Y * sinA, v.X * sinA + v.Y * cosA);
            Vector2 p1 = Rot(new Vector2(aLen, 0));       // tip (right when closed)
            Vector2 p2 = Rot(new Vector2(-aLen, -aLen));  // top arm
            Vector2 p3 = Rot(new Vector2(-aLen, aLen));   // bottom arm
            var arrowU32 = ImGui.ColorConvertFloat4ToU32(new Vector4(0.75f, 0.75f, 0.75f, 1f));
            drawList.AddTriangleFilled(arrowCenter + p1, arrowCenter + p2, arrowCenter + p3, arrowU32);

            // Label
            drawList.AddText(pos + new Vector2(26f, 5f), ImGui.ColorConvertFloat4ToU32(new Vector4(0.9f, 0.9f, 0.9f, 1f)), label);

            // Move cursor past header
            ImGui.SetCursorScreenPos(pos + new Vector2(0, headerHeight + 2f));

            // Clip content during animation
            if (progress > 0.001f)
            {
                var contentPos = ImGui.GetCursorScreenPos();
                var clipMin = contentPos;
                var clipMax = contentPos + new Vector2(ImGui.GetContentRegionAvail().X, estimatedHeight);
                ImGui.PushClipRect(clipMin, clipMax, true);
            }

            return progress > 0.99f;
        }
    }

    public struct ScopedColor : IDisposable
    {
        private readonly int _count;
        public ScopedColor(ImGuiCol col, Vector4 color) { ImGui.PushStyleColor(col, color); _count = 1; }
        public ScopedColor(ImGuiCol c1, Vector4 v1, ImGuiCol c2, Vector4 v2) { ImGui.PushStyleColor(c1, v1); ImGui.PushStyleColor(c2, v2); _count = 2; }
        public ScopedColor(ImGuiCol c1, Vector4 v1, ImGuiCol c2, Vector4 v2, ImGuiCol c3, Vector4 v3) { ImGui.PushStyleColor(c1, v1); ImGui.PushStyleColor(c2, v2); ImGui.PushStyleColor(c3, v3); _count = 3; }
        public void Dispose() { if (_count > 0) ImGui.PopStyleColor(_count); }
    }

    public struct ScopedVar : IDisposable
    {
        private readonly int _count;
        public ScopedVar(ImGuiStyleVar var, float val) { ImGui.PushStyleVar(var, val); _count = 1; }
        public ScopedVar(ImGuiStyleVar var, Vector2 val) { ImGui.PushStyleVar(var, val); _count = 1; }
        public ScopedVar(ImGuiStyleVar v1, float f1, ImGuiStyleVar v2, float f2) { ImGui.PushStyleVar(v1, f1); ImGui.PushStyleVar(v2, f2); _count = 2; }
        public ScopedVar(ImGuiStyleVar v1, float f1, ImGuiStyleVar v2, float f2, ImGuiStyleVar v3, float f3) { ImGui.PushStyleVar(v1, f1); ImGui.PushStyleVar(v2, f2); ImGui.PushStyleVar(v3, f3); _count = 3; }
        public void Dispose() { if (_count > 0) ImGui.PopStyleVar(_count); }
    }
}

