<p align="center">
  <img src="icon.png" width="96" alt="AlmondChatTranslated">
</p>

<h1 align="center">AlmondChatTranslated · 杏仁秒译</h1>
<p align="center">基于 DeepSeek API 的 FFXIV 极速实时聊天与招募板翻译插件</p>

## 关于

**AlmondChatTranslated** 基于 [kelvin124124/ChatTranslated](https://github.com/kelvin124124/ChatTranslated)（AGPL-3.0）重构与持续开发，移除了原版冗余且易失效的多后端与在线语言检测端口，专为 **DeepSeek API** 深度精简与性能优化。

## 核心特性

- **多接口极速直连 + 晚高峰主备自动兜底**：内置 **DeepSeek、阿里云百炼 (Qwen)、智谱 BigModel (GLM-4-Flash 免费)、火山引擎方舟、硅基流动、腾讯混元、Google Gemini、本地 Ollama (0ms 离线)** 与自定义 OpenAI 兼容接口一键预设（切换自动记住各家 Key）；支持 **主备双线路 + 慢请求竞速秒切 (Hedged Failover)**，主接口卡排队超过设定秒数或报错时自动并发调用备用接口，配合 SSE 流式传输大幅消除晚高峰超时。
- **零网络开销本地语言识别**：基于 Unicode 字符集、简繁日特征词与游戏术语库在本地 0ms 完成语言预检，精准跳过中文对话、纯坐标链接、单字符标记与攻略代号（`MT`、`H1`、`D1`、`o/` 等）。
- **真正隐藏原文 + 保留游戏链接**：开启「仅显示译文（隐藏原文）」时直接拦截原始外语消息并输出 `[译]` 译文，自动保留可点击的物品、地图坐标与招募链接；遇到无需翻译或网络超时自动安全回退显示原文。
- **高可靠并发与熔断保护**：支持请求合并去重、LRU 翻译缓存原子落盘、严格总超时控制、连续故障自动熔断与连接池自愈，主线程安全派发聊天输出。
- **中文发话秒翻（免 `/tr` 直发 + 指令双模式）**：内置中→日/英 FF14 玩家术语表与 0ms 高频社交短语直出，支持**免 `/tr` 在指定频道直接打中文自动翻译发送**（默认仅对小队/团队/说话/喊话/呼喊生效，不干扰中文部队/通讯贝，句首加 `!` 可随时发中文原文）；同时支持 `/tr`、`/trp`、`/trs`、`/tra`、`/trsh`、`/trfc`、`/trr` 指定频道发话，支持直发模式与 `/ok` 预览确认模式。
- **精简暗金 Almond 界面**：三栏式直观导航（`实时翻译` / `DeepSeek` / `语言与频道`），支持一键测试连接与查询余额、聊天记录左键复制译文/右键复制原文、快捷翻译一键发送到聊天框及队伍招募板右键翻译。

## 安装与快速开始

1. 在聊天框输入 `/xlsettings` 打开卫月设置，在 **自定义插件仓库** 中添加：

   `https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json`

2. 在 `/xlplugins` 中搜索 **AlmondChatTranslated** 安装并启用。
3. 输入 `/act` 打开主面板，在 **DeepSeek** 页面填入 `sk-` 开头的 API Key，点击 **测试连接并查余额** 即可开始使用。

## 常用指令

| 指令 | 说明 |
| :--- | :--- |
| `/act`（或 `/pchat`） | 打开实时翻译与主面板 |
| `/act config` | 直接打开 DeepSeek 接口配置页 |
| `/act on` / `/act off` | 开启 / 暂停实时翻译 |
| `/act integration` | 切换是否将译文输出到游戏聊天框 |
| `/tr <中文>` | 按 FF14 术语翻译为外语（支持 `/tr /p 内容` 指定频道、`/tr auto` 开关免指令直译、`/tr mode` 切换直发/预览、`/tr cancel` 取消） |
| `!中文原文` | 开启免 `/tr` 直译后，在句首加 `!` 或 `！` 跳过翻译直接发送原文 |
| `/trp` `/trs` `/tra` `/trsh` `/trfc` `/trr` | 分别翻译并发送到小队 / 说话 / 团队 / 喊话 / 部队 / 回复私聊 |
| `/ok`（或 `/trok`） | 在预览确认模式下确认并发送当前预览译文 |

## 许可协议

遵循原项目 [GNU Affero General Public License v3.0 (AGPL-3.0)](LICENSE.md)。
