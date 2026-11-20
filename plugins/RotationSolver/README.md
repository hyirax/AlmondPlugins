# Rotation Solver Reborn (CN) · 自用简体中文汉化版

本插件为基于开源项目 [FFXIV-CombatReborn/RotationSolverReborn](https://github.com/FFXIV-CombatReborn/RotationSolverReborn) 制作的**自用简体中文（`zh-CN`）全量汉化版本**，通过 [AlmondPlugins](https://github.com/hyirax/AlmondPlugins) 统一打包分发编译产物。

## 主要特性与本地调整

- **全量简体中文汉化**：覆盖设置主窗口、精简/完整悬浮控制台、技能时间轴、首次启动交互式新手引导、版本更新说明、全部全局配置项、各职业/特殊探索副本循环（含新月岛辅助职业、多变迷宫、博兹雅、怪猎联动等）、循环专属配置及状态面板。
- **内置国服官方数据词库 (`zh.names.json`)**：完整映射国服官方技能、状态、特性、特职、物品、副本、区域与 NPC 中文名称，国际服英文客户端也可直接查看国服官方译名。
- **中英实时无感切换 & 双语检索/宏兼容**：可在 **界面 -> 信息 -> 界面与提示语言** 或窗口顶栏随时在 **简体中文** 与 **English** 之间即时切换，无需重启插件；支持中英双语搜索及 `/rotation ToggleActions`、`/rotation DoActions` 聊天指令匹配，同时保持英文配置键名兼容。
- **内置 CJK 字体兼容 (`CJKFontHook`)**：自动注入中文字形至 `RebornMaterial (M3)` 自定义字号，国际服与国服客户端均可正常显示中文。

## 安装与使用

1. 在 Dalamud 设置 → **实验性功能 > 自定义插件仓库** 添加总库地址：
   ```text
   https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json
   ```
2. 保存后打开 `/xlplugins`，搜索 **Rotation Solver Reborn (CN)** 并安装。
3. 在游戏中输入 `/rotation` 或 `/rsr` 打开设置与控制面板。

## 开源协议与声明

- 上游原项目：`FFXIV-CombatReborn/RotationSolverReborn`（GNU Lesser General Public License v3.0 `LGPL-3.0`，详见同目录 [COPYING](COPYING) 与 [COPYING.LESSER](COPYING.LESSER) 及压缩包内附带的许可文件）。
- **完全免费、仅限个人自用与亲友免费分享，严禁转售或设置付费下载**。
