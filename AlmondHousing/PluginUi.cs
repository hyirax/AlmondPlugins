using System;
using AlmondHousing.Gui;
using Dalamud.Interface.Windowing;

namespace AlmondHousing
{
    public class PluginUi : IDisposable
    {
        private readonly AlmondHousing _plugin;
        public ConfigurationWindow ConfigWindow { get; }
        
        // 🚀 新增：卫月原生的窗口管理系统引擎
        public WindowSystem WindowSystem { get; }

        public PluginUi(AlmondHousing plugin)
        {
            _plugin = plugin;
            ConfigWindow = new ConfigurationWindow(plugin);
            WindowSystem = new WindowSystem("AlmondHousing");

            // 将你的主面板注册到系统引擎中
            WindowSystem.AddWindow(ConfigWindow);

            DalamudApi.PluginInterface.UiBuilder.Draw += Draw;
            DalamudApi.PluginInterface.UiBuilder.OpenConfigUi += OnOpenConfigUi;
            DalamudApi.PluginInterface.UiBuilder.OpenMainUi += OnOpenConfigUi;
        }

        private void Draw()
        {
            // 1. 让 WindowSystem 自动接管并绘制带有齿轮菜单的界面！
            WindowSystem.Draw();

            // 2. 将 3D 屏幕家具标签渲染剥离出来独立执行
            ConfigWindow.DrawItemOnScreen();
        }
        
        private void OnOpenConfigUi()
        {
            ConfigWindow.IsOpen = true; // 在 WindowSystem 中，控制显示隐藏的属性叫 IsOpen
            ConfigWindow.CanUpload = false;
            ConfigWindow.CanImport = false;
        }

        public void Dispose()
        {
            WindowSystem.RemoveAllWindows();
            DalamudApi.PluginInterface.UiBuilder.Draw -= Draw;
            DalamudApi.PluginInterface.UiBuilder.OpenConfigUi -= OnOpenConfigUi;
            DalamudApi.PluginInterface.UiBuilder.OpenMainUi -= OnOpenConfigUi;
        }
    }
}