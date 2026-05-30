using System;
using System.Collections.Generic;
using System.Numerics;
using Dalamud.Bindings.ImGui;

namespace AlmondHousing.Gui
{
    public static class ToastManager
    {
        private struct ToastItem
        {
            public string Message;
            public float CreatedAt;
            public float Duration;
            public Vector4 Color;
        }

        private static readonly List<ToastItem> _toasts = new();
        private static float _time;

        public static void Show(string message, Vector4? color = null)
        {
            _toasts.Add(new ToastItem
            {
                Message = message,
                CreatedAt = _time,
                Duration = 3.0f,
                Color = color ?? new Vector4(0.2f, 0.8f, 1.0f, 1.0f)
            });
        }

        public static void ShowError(string message)
        {
            _toasts.Add(new ToastItem
            {
                Message = message,
                CreatedAt = _time,
                Duration = 4.0f,
                Color = new Vector4(1.0f, 0.35f, 0.35f, 1.0f)
            });
        }

        public static void Update(float deltaTime)
        {
            _time += deltaTime;
            _toasts.RemoveAll(t => _time - t.CreatedAt > t.Duration);
        }

        public static void Draw()
        {
            if (_toasts.Count == 0) return;

            var viewport = ImGui.GetMainViewport();
            var drawList = ImGui.GetForegroundDrawList();
            float baseY = 60f;
            float toastHeight = 32f;
            float maxWidth = 400f;

            for (int i = _toasts.Count - 1; i >= 0; i--)
            {
                var toast = _toasts[i];
                float elapsed = _time - toast.CreatedAt;
                if (elapsed > toast.Duration) continue;

                // Animation: slide in from right, fade out near end
                float fadeIn = Math.Min(elapsed * 5f, 1.0f);
                float fadeOut = Math.Max(0f, (toast.Duration - elapsed) / 0.5f);
                float alpha = Math.Min(fadeIn, fadeOut);

                float xOffset = (1f - fadeIn) * 60f;
                float y = baseY + (toast.Duration > 0 ? 0 : 0);

                string text = toast.Message.Length > 60 ? toast.Message[..57] + "..." : toast.Message;
                var textSize = ImGui.CalcTextSize(text);
                float width = Math.Min(textSize.X + 28f, maxWidth);
                float x = viewport.WorkPos.X + viewport.WorkSize.X - width - 20f + xOffset;

                var bgColor = new Vector4(0.06f, 0.06f, 0.08f, 0.92f * alpha);
                var borderColor = new Vector4(toast.Color.X, toast.Color.Y, toast.Color.Z, 0.6f * alpha);
                var textColor = new Vector4(toast.Color.X, toast.Color.Y, toast.Color.Z, alpha);

                uint bgU32 = ImGui.ColorConvertFloat4ToU32(bgColor);
                uint borderU32 = ImGui.ColorConvertFloat4ToU32(borderColor);
                uint textU32 = ImGui.ColorConvertFloat4ToU32(textColor);

                var min = new Vector2(x, y);
                var max = new Vector2(x + width, y + toastHeight);

                drawList.AddRectFilled(min, max, bgU32, 6f);
                drawList.AddRect(min, max, borderU32, 6f);
                drawList.AddText(new Vector2(x + 14f, y + 7f), textU32, text);

                y += toastHeight + 8f;
            }
        }
    }
}
