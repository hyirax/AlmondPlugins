<p align="center">
  <img src="../../icons/AlmondHousing.png" width="96" alt="AlmondHousing">
</p>

<h1 align="center">AlmondHousing · Housing Atelier</h1>
<p align="center">Layouts, furniture, and material tools for FFXIV housing</p>

[简体中文](../../README.md) · [繁體中文](../zh_TW/README.md) · [文言文](../lzh/README.md) · **English** · [日本語](../ja/README.md) · [한국어](../ko/README.md) · [Deutsch](../de/README.md) · [Français](../fr/README.md) · [Русский](../ru/README.md)

## About

**AlmondHousing** is an FFXIV housing tool independently designed and maintained by **AlmondCookie**. It brings furniture browsing, layout management, material auditing, and layout sharing into one focused interface.

## Free Use

**The plugin itself is free. Do not resell it or place its download behind a paywall.** If you paid to obtain this plugin, request a refund and report the seller.

Housing layouts may contain someone else's creative work. Only share or trade data you are authorized to use, and respect the original designer's terms.

## Features

- **Furniture and fixtures**: Search, categorize, and filter indoor furniture, outdoor furnishings, and fixtures in real time.
- **Layout management**: Read, match, apply, and export house or yard layouts; exporting does not require furnishing permissions.
- **Material audit**: Scan inventory and Chocobo Saddlebags, compare required, owned, and missing quantities, and export Teamcraft or CSV lists.
- **Layout compatibility**: Read MakePlace and ReMakePlace `.json` files and save shared data as `.almond` files.
- **Protected sharing**: Use standard encryption or an optional custom password. Password-protected imports prompt for the password automatically.
- **Precision controls**: Coordinate dragging, snapping, grouped movement, undo/redo, and a 3D transform gizmo.

## Installation

1. Start the game with [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/).
2. Enter `/xlsettings` in chat to open Dalamud settings.
3. Open **Experimental** and add this URL under **Custom Plugin Repositories**:

   `https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json`

4. Enable the repository and save the settings.
5. Search for **AlmondHousing** in `/xlplugins` and install it.

## Quick Start

1. Enter `/almond` to open the plugin.
2. Enter any readable house or yard, open **Layout & Export**, and choose **Save As**.
3. Open **Material Audit** to review missing items and export a shopping list when needed.
4. Before applying a layout, place the required furniture in the current house, enable Rotate Layout mode, and choose **Apply from File**.

The plugin only matches and moves furniture already placed in the current house. Keep Rotate Layout mode active, do not move furniture manually, and do not leave the house while applying.

## Layouts and Passwords

- Leaving the password empty uses standard `.almond` encryption.
- A custom password must contain at least 6 characters and is kept only for the current plugin session.
- Importing a password-protected `.almond` file opens a password prompt. Ordinary files do not ask for one.

## Credits

Drakansoul · NotNite · jawslouis
