<p align="center">
  <img src="../../icons/AlmondHousing.png" width="96" alt="AlmondHousing">
</p>

<h1 align="center">AlmondHousing · 杏仁造境</h1>
<p align="center">FFXIV 房屋裝修的佈局、家具與物料工具</p>

[简体中文](../../README.md) · **繁體中文** · [文言文](../lzh/README.md) · [English](../en/README.md) · [日本語](../ja/README.md) · [한국어](../ko/README.md) · [Deutsch](../de/README.md) · [Français](../fr/README.md) · [Русский](../ru/README.md)

## 關於

**AlmondHousing** 是由 **AlmondCookie** 獨立設計與維護的 FFXIV 房屋裝修工具。它將家具瀏覽、佈局管理、物料盤點與圖紙分享集中在一個清晰的介面中。

## 免費使用

**外掛本體免費，禁止轉售或設定付費下載。** 如果你透過付費管道取得本外掛，請申請退款並檢舉販售者。

裝修佈局可能包含他人的創作成果。只分享或交易你有權處理的資料，並遵守原設計者的授權要求。

## 功能

- **家具與部件**：即時搜尋、分類與篩選室內家具、庭具和房屋部件。
- **佈局管理**：讀取、配對、套用及匯出房屋或庭院佈局；匯出不需要裝潢權限。
- **物料盤點**：掃描背包與陸行鳥鞍囊，比較所需、已有及缺少數量，並匯出 Teamcraft 或 CSV 清單。
- **圖紙相容**：讀取 MakePlace、ReMakePlace 的 `.json` 檔案，並使用 `.almond` 儲存分享資料。
- **安全分享**：支援標準加密與可選自訂密碼；匯入受密碼保護的檔案時會自動提示輸入密碼。
- **精細調節**：提供座標拖曳、吸附、群組移動、復原/重做及 3D 操作軸。

## 安裝

1. 使用 [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/) 啟動遊戲。
2. 在聊天欄輸入 `/xlsettings`，開啟 Dalamud 設定。
3. 進入 **實驗性功能**，在 **自訂外掛倉庫** 中加入：

   `https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json`

4. 啟用該倉庫並儲存設定。
5. 在 `/xlplugins` 中搜尋 **AlmondHousing** 並安裝。

## 快速開始

1. 輸入 `/almond` 開啟外掛。
2. 進入任意可讀取的房屋或庭院，在 **佈局與匯出** 中選擇 **另存新檔**。
3. 開啟 **物料盤點**，檢查缺件並按需要匯出採購清單。
4. 套用佈局前，先把需要的家具放進目前房屋，開啟裝潢旋轉模式，再選擇 **從檔案套用**。

外掛只會配對並移動目前房屋中已放置的家具。套用期間請保持旋轉模式，不要手動移動家具，也不要離開房屋。

## 圖紙與密碼

- 密碼留空時，`.almond` 使用標準加密。
- 自訂密碼至少需要 6 個字元，且只在目前外掛工作階段中保留。
- 匯入帶密碼的 `.almond` 檔案時，外掛會彈出密碼輸入視窗；一般檔案不會詢問密碼。

## 致謝

Drakansoul · NotNite · jawslouis
