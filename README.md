<p align="center">
  <img src="icons/AlmondHousing.png" width="96" alt="AlmondHousing">
</p>

<h1 align="center">AlmondHousing · 杏仁造境</h1>
<p align="center">FFXIV 房屋装修的布局、家具与物料工具</p>

**简体中文** · [繁體中文](docs/zh_TW/README.md) · [文言文](docs/lzh/README.md) · [English](docs/en/README.md) · [日本語](docs/ja/README.md) · [한국어](docs/ko/README.md) · [Deutsch](docs/de/README.md) · [Français](docs/fr/README.md) · [Русский](docs/ru/README.md)

## 关于

**AlmondHousing** 是由 **AlmondCookie** 独立设计与维护的 FFXIV 房屋装修工具。它把家具浏览、布局管理、物料盘点和图纸分享集中在一个清晰的界面中。

## 免费使用

**插件本体免费，禁止转售或设置付费下载。** 如果你通过付费渠道获得了本插件，请申请退款并举报销售者。

装修布局可能包含他人的创作成果。只分享或交易你有权处理的数据，并遵守原设计者的授权要求。

## 功能

- **家具与部件**：实时搜索、分类和筛选室内家具、庭具与房屋部件。
- **布局管理**：读取、匹配、应用和导出房屋或庭院布局；导出不要求装修权限。
- **物料盘点**：扫描背包与陆行鸟鞍囊，对比所需、已有和缺少数量，并导出 Teamcraft 或 CSV 清单。
- **图纸兼容**：读取 MakePlace、ReMakePlace 的 `.json` 文件，并使用 `.almond` 保存分享数据。
- **安全分享**：支持标准加密和可选自定义密码；导入受密码保护的文件时会自动提示输入密码。
- **精细调节**：提供坐标拖拽、吸附、分组移动、撤销/重做和 3D 操作轴。

## 安装

1. 使用 [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/) 启动游戏。
2. 在聊天框输入 `/xlsettings`，打开 Dalamud 设置。
3. 进入 **实验性功能**，在 **自定义插件仓库** 中添加：

   `https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json`

4. 启用该仓库并保存设置。
5. 在 `/xlplugins` 中搜索 **AlmondHousing** 并安装。

## 快速开始

1. 输入 `/almond` 打开插件。
2. 进入任意可读取的房屋或庭院，在 **布局与导出** 中选择 **另存为**。
3. 打开 **物料盘点**，检查缺件并按需导出采购清单。
4. 应用布局前，将需要的家具放入当前房屋，开启装修旋转模式，再选择 **从文件应用**。

插件只会匹配并移动当前房屋中已经放置的家具。应用期间请保持旋转模式，不要手动移动家具，也不要离开房屋。

## 图纸与密码

- 密码留空时，`.almond` 使用标准加密。
- 自定义密码至少需要 6 个字符，且只在当前插件会话中保留。
- 导入带密码的 `.almond` 文件时，插件会弹出密码输入窗口；普通文件不会询问密码。

## 致谢

Drakansoul · NotNite · jawslouis
