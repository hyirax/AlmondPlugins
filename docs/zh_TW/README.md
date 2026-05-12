# 🏡 AlmondHousing 杏仁房屋工具

*🌐 Languages: [简体中文](../../README.md) | **[繁體中文](README.md)** | [English](../en/README.md) | [日本語](../ja/README.md) | [한국어](../ko/README.md) | [Deutsch](../de/README.md) | [Français](../fr/README.md)*

---

### 🛑 嚴正聲明：抵制倒賣，尊重原創 🛑

**本外掛 100% 完全免費且開源！嚴禁任何形式的售賣！**

本工具的初衷是為了方便玩家**備份自己的裝修**、**跨區共享免費圖紙**。絕不容忍任何人使用本工具偷竊其他玩家、設計師的原創裝修資料，並用於接單、倒賣或任何商業牟利行為！
如果您是在任何付費渠道獲取到的本外掛，或發現有人利用本外掛盜賣圖紙，請立刻**申請退款並檢舉**！

**知識自由，尊重原創！抵制倒賣，從我做起！**

---

### ✨ 簡介

**AlmondHousing** 是基於 [DisPlace](https://github.com/Drakansoul/DisPlace) 深度重構的 FF14 房屋裝修增強外掛，由 **AlmondCookie** 獨立維護。致力於讓複雜的房屋裝修、材料盤點與資料分享變得更加極客與純粹。

**核心特性：**
* 🌍 **多語言無縫切換**：原生支援 中文（簡/繁）、英語、日語、韓語、德語、法語，滿足國際服/陸服全環境需求。
* 📦 **智能材料盤點**：全自動掃描玩家背包與陸行鳥鞍包，即時對比目前圖紙，精準計算家具「缺口」。
* 📊 **高級清單匯出**：一鍵匯出剔除了已有家具的 **Teamcraft 採購單**，或帶有詳細餘缺資料的 CSV 表格。
* 🏗️ **圖紙相容與保護**：完美向下相容讀取 MakePlace 和 ReMakePlace 的 `.json` 圖紙；同時採用 ChaCha20-Poly1305 演算法支援儲存為專屬 `.almond` 加密格式，防篡改、防盜用。

---

### 🚀 如何安裝

1. **前提**：使用 [衛月框架 (Dalamud / FFXIV Quick Launcher)](https://goatcorp.github.io/) 啟動遊戲。
2. 在遊戲聊天框輸入 `/xlsettings` 打開衛月設定。
3. 切換至 **「實驗性功能 (Experimental)」** 標籤頁。
4. 在 **「自訂外掛倉庫」** 中新增以下連結：
   `https://raw.githubusercontent.com/hyirax/AlmondHousing/master/pluginmaster.json`
5. 點擊 **+** 號並勾選 **「啟用 (Enabled)」**，然後點擊「儲存並關閉」。
6. 在 `/xlplugins` 外掛大廳搜尋 **`AlmondHousing`** 即可安裝。

---

### 🎮 快速指南

在遊戲聊天框輸入 **`/almond`** 喚出控制台。

1. **儲存佈局**：進入您擁有權限的房屋，開啟內建的「旋轉佈局」模式後，點擊 `另存新檔 (Save As)` 即可備份目前房屋資料。
2. **材料準備**：點擊 `材料盤點 (Material Audit)` 標籤頁，查看所缺家具，並匯出清單進行精準採購。
3. **套用佈局**：將圖紙所需的家具放在背包或放置在地板上，點擊 `選擇檔案並套用 (Select File & Apply)`，外掛將自動完成佈置。

*(注意：自動佈置期間請勿移動滑鼠或讓人物擋住家具移動路線，以免引發遊戲崩潰。)*

---

### 💖 鳴謝

本項目在以下優秀開源項目的基礎上重構與優化，特此致敬：

* **Drakansoul**：感謝創建並維護了 DisPlace。
* **NotNite**：感謝強行將 MakePlace 外掛重新開源。
* **jawslouis**：感謝製作了非常出色的 MakePlacePlugin 外掛。
* 感謝 **HousingPos**, **BDTH** 以及 **HouseMate** 等前輩項目提供的技術靈感。

---
*如果您覺得本外掛有幫助，請為這個項目點一個 ⭐️ Star！*