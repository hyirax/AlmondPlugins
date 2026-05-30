using Dalamud.Bindings.ImGui;
using System;
using System.Numerics;

namespace AlmondHousing.Gui
{
    public partial class ConfigurationWindow
    {
        private readonly ImGuiKey[] _konamiCode = new ImGuiKey[]
        {
            ImGuiKey.UpArrow, ImGuiKey.UpArrow,
            ImGuiKey.DownArrow, ImGuiKey.DownArrow,
            ImGuiKey.LeftArrow, ImGuiKey.RightArrow,
            ImGuiKey.LeftArrow, ImGuiKey.RightArrow,
            ImGuiKey.B, ImGuiKey.A
        };

        private int _konamiProgress = 0;
        private bool _isDeveloperModeUnlocked = false;

        private void ListenForCheatCode()
        {
            ImGuiKey[] keysToCheck = { ImGuiKey.UpArrow, ImGuiKey.DownArrow, ImGuiKey.LeftArrow, ImGuiKey.RightArrow, ImGuiKey.B, ImGuiKey.A };

            foreach (var key in keysToCheck)
            {
                if (ImGui.IsKeyPressed(key))
                {
                    if (key == _konamiCode[_konamiProgress])
                    {
                        _konamiProgress++;

                        if (_konamiProgress >= _konamiCode.Length)
                        {
                            _isDeveloperModeUnlocked = !_isDeveloperModeUnlocked;
                            _konamiProgress = 0;

                            if (_isDeveloperModeUnlocked)
                            {
                                ToastManager.Show(">>> DEV MODE ACTIVATED <<<", new Vector4(1f, 0.85f, 0.3f, 1f));
                                BeepSuccess();
                            }
                            else
                            {
                                ToastManager.Show("Dev mode disabled.", new Vector4(0.5f, 0.5f, 0.5f, 1f));
                                BeepFailure();
                            }
                        }
                    }
                    else
                    {
                        _konamiProgress = (key == _konamiCode[0]) ? 1 : 0;
                    }
                    break;
                }
            }
        }

        private static void BeepSuccess()
        {
            try { Console.Beep(523, 100); Console.Beep(659, 100); Console.Beep(784, 100); Console.Beep(1046, 300); }
            catch { }
        }

        private static void BeepFailure()
        {
            try { Console.Beep(1046, 100); Console.Beep(784, 100); Console.Beep(523, 300); }
            catch { }
        }
    }
}
