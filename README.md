# Almond Plugins · 杏仁插件

AlmondCookie 的 FFXIV Dalamud 插件发布目录。一个订阅链接集中提供当前与后续插件的安装和更新。

Official FFXIV Dalamud plugin releases by AlmondCookie. One repository URL provides installation and updates for current and future plugins.

## 安装 / Install

在 Dalamud 设置的 **实验性功能 > 自定义插件仓库** 中添加：

Add this URL under **Dalamud Settings > Experimental > Custom Plugin Repositories**:

```text
https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json
```

保存后，在 `/xlplugins` 中搜索插件名称即可安装或更新。

Save the settings, then search for the plugin name in `/xlplugins`.

## 插件 / Plugins

### AlmondHousing · 杏仁造境

FFXIV 房屋装修工具，提供家具检索、布局管理、物料盘点、加密分享与精细摆放。

An FFXIV housing tool for furniture search, layout management, material auditing, protected sharing, and precision placement.

| 版本 / Version | 命令 / Command | 下载 / Download |
| --- | --- | --- |
| 1.1.6 | `/almond` | [AlmondHousing 1.1.6](https://github.com/hyirax/AlmondPlugins/releases/tag/AlmondHousing-v1.1.6) |

[简体中文](plugins/AlmondHousing/README.md) · [繁體中文](plugins/AlmondHousing/docs/zh_TW/README.md) · [文言文](plugins/AlmondHousing/docs/lzh/README.md) · [English](plugins/AlmondHousing/docs/en/README.md) · [日本語](plugins/AlmondHousing/docs/ja/README.md) · [한국어](plugins/AlmondHousing/docs/ko/README.md) · [Deutsch](plugins/AlmondHousing/docs/de/README.md) · [Français](plugins/AlmondHousing/docs/fr/README.md) · [Русский](plugins/AlmondHousing/docs/ru/README.md)

### AlmondZoom

轻量的 FFXIV 镜头工具，提供视距、视野、相机高度、镜头碰撞和平滑过渡调节。

A lightweight FFXIV camera tool for zoom distance, FOV, camera height, collision control, and smooth transitions.

| 版本 / Version | 命令 / Command | 下载 / Download |
| --- | --- | --- |
| 1.0.3 | `/az` | [AlmondZoom 1.0.3](https://github.com/hyirax/AlmondPlugins/releases/tag/AlmondZoom-v1.0.3) |

### AlmondChatTranslated · 杏仁秒译

基于 DeepSeek API 的 FFXIV 极速实时聊天与招募板翻译插件，内置本地语言预检、上下文缓存与暗金三栏界面。

A fast FFXIV real-time chat and Party Finder translator powered by DeepSeek API, featuring local language pre-filtering, prompt caching, and a clean Almond UI.

| 版本 / Version | 命令 / Command | 下载 / Download |
| --- | --- | --- |
| 1.0.0 | `/act` | [AlmondChatTranslated 1.0.0](https://github.com/hyirax/AlmondPlugins/releases/tag/AlmondChatTranslated-v1.0.0) |

[说明文档](plugins/AlmondChatTranslated/README.md) · [AGPL-3.0 LICENSE](plugins/AlmondChatTranslated/LICENSE.md) · [第三方声明](plugins/AlmondChatTranslated/THIRD-PARTY-NOTICES.txt)

### BossMod Reborn (CN) · 自用简体中文汉化版

基于官方开源项目 `FFXIV-CombatReborn/BossmodReborn` 打包的自用简体中文（`zh-CN`）汉化分支，包含完整界面、设置项、全版本副本/Boss/FATE/任务名称、职业循环与战斗机制提示汉化，并默认开启 `WIP` Boss 模块加载以便查看技能雷达提示。

Personal Simplified Chinese (`zh-CN`) localization build based on upstream `FFXIV-CombatReborn/BossmodReborn`, featuring localized UI, settings, duties/bosses/FATEs, autorotation and combat hints, with `WIP` boss modules enabled by default.

| 版本 / Version | 命令 / Command | 下载 / Download |
| --- | --- | --- |
| 7.5.8.9-cn | `/bmr` | [BossMod Reborn (CN) 7.5.8.9-cn](https://github.com/hyirax/AlmondPlugins/releases/tag/BossModReborn-v7.5.8.9-cn) |

[说明文档](plugins/BossModReborn/README.md) · [BSD-3-Clause LICENSE](plugins/BossModReborn/LICENSE)

### Rotation Solver Reborn (CN) · 自用简体中文汉化版

基于官方开源项目 `FFXIV-CombatReborn/RotationSolverReborn` 打包的自用简体中文（`zh-CN`）全量汉化分支，覆盖插件设置主窗口、悬浮控制台、技能时间轴、首次启动新手教程、全部全局配置及各职业/特殊探索副本循环，内置国服官方技能与状态名称词库并支持中英实时切换与跨服 CJK 字体兼容。

Personal Simplified Chinese (`zh-CN`) localization build based on upstream `FFXIV-CombatReborn/RotationSolverReborn`, featuring full UI, settings, job/duty rotation, official CN Lumina game data, runtime CN/EN toggle, and cross-client CJK font support.

| 版本 / Version | 命令 / Command | 下载 / Download |
| --- | --- | --- |
| 7.5.8.7-cn | `/rotation` | [Rotation Solver Reborn (CN) 7.5.8.7-cn](https://github.com/hyirax/AlmondPlugins/releases/tag/RotationSolver-v7.5.8.7-cn) |

[说明文档](plugins/RotationSolver/README.md) · [GPL-3.0 COPYING](plugins/RotationSolver/COPYING) · [LGPL-3.0 COPYING.LESSER](plugins/RotationSolver/COPYING.LESSER)

## 发布说明 / Distribution

本仓库只公开插件目录、说明、图标与编译后的发布包，不包含插件源码。插件本体免费，禁止转售或设置付费下载。

This repository contains only the plugin catalog, documentation, icons, and compiled release packages. Plugin binaries are free and must not be resold or placed behind a paywall.
