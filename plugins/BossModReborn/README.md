# BossMod Reborn (CN) · 自用简体中文汉化版

本插件为基于开源项目 [FFXIV-CombatReborn/BossmodReborn](https://github.com/FFXIV-CombatReborn/BossmodReborn) 制作的**自用简体中文（`zh-CN`）汉化版本**，通过 [AlmondPlugins](https://github.com/hyirax/AlmondPlugins) 统一打包分发编译产物。

## 主要特性与本地调整

- **全量简体中文汉化**：覆盖设置面板、主窗口、全版本副本/Boss/FATE/任务/危命事件名称、职业自动循环策略与技能名称、战斗阶段与点名/分摊/躲避提示。
- **内置 CJK 字体兼容**：自动绑定中文字体渲染（国际服或国服客户端均可正常显示中文，不出现方框乱码）。
- **默认开启 `WIP` Boss 模块加载**：将 Boss 模块最低成熟度默认设为 `WIP`（含已有旧配置的自动一次性迁移），方便在没有完整时间轴的副本里也能直接显示雷达 AOE 与基础机制提示，辅助手动走位躲避。
  *(注：深层迷宫与探索区域的自动化模块仍保留默认的 `Contributed` 门槛，避免意外接管移动；如需启用可在“全自动副本”设置里自行调整。)*
- **任务搜索器 `BMR ✓` 支持标记**：在常规与高难任务搜索器中直接标出已支持副本及最低模块成熟度颜色（红=`WIP` / 蓝=`Contributed` / 绿=`Verified` / 金=`AI 辅助`）。

## 安装与使用

1. 在 Dalamud 设置 → **实验性功能 > 自定义插件仓库** 添加总库地址：
   ```text
   https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json
   ```
2. 保存后打开 `/xlplugins`，搜索 **BossMod Reborn (CN)** 并安装。
3. 在游戏中输入 `/bmr` 或 `/vbm` 打开设置与雷达界面。

## 开源协议与声明

- 上游原项目：`FFXIV-CombatReborn/BossmodReborn`（BSD-3-Clause License，详见同目录 [LICENSE](LICENSE) 及压缩包内附带的 `LICENSE`）。
- 本分发包仅包含编译后的运行文件与必要许可文件，**完全免费、仅供个人自用与亲友分享，禁止转售或设置付费下载**。
