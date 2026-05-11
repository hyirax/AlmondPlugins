# 🏡 AlmondHousing

*🌐 Languages: [简体中文](../../README.md) | [繁體中文](../zh_TW/README.md) | [English](../en/README.md) | [日本語](../ja/README.md) | [한국어](../ko/README.md) | **[Deutsch](README.md)** | [Français](../fr/README.md)*

---

### 🛑 Strikte Warnung vor Weiterverkauf & Respekt vor Originalautoren 🛑

**Dieses Plugin ist 100 % KOSTENLOS und OPEN-SOURCE! Der Verkauf in jeglicher Form ist strengstens untersagt!**

Der ursprüngliche Zweck dieses Tools ist es, Spielern zu helfen, **ihre eigenen Einrichtungen zu sichern** und **kostenlose Baupläne (Blueprints) regionsübergreifend zu teilen**. Wir tolerieren es absolut nicht, wenn dieses Tool genutzt wird, um originelle Designs anderer Spieler oder Designer für kommerzielle Zwecke, Aufträge oder Weiterverkauf zu stehlen!
Wenn Sie dieses Plugin über einen kostenpflichtigen Kanal erhalten haben oder jemanden sehen, der damit erstellte Baupläne verkauft, fordern Sie bitte **sofort eine Rückerstattung an und melden Sie den Verkäufer**.

**Wissen ist frei. Respektiert Originalautoren. Sagt NEIN zum Weiterverkauf!**

---

### ✨ Einführung

**AlmondHousing** ist ein erweitertes FFXIV-Housing-Plugin, das auf [DisPlace](https://github.com/Drakansoul/DisPlace) basiert, tiefgreifend neu strukturiert und von **AlmondCookie** unabhängig gepflegt wird. Es wurde entwickelt, um komplexe Wohnungseinrichtungen, Materialverwaltung und Datenaustausch geekiger, puristischer und einfacher zu machen.

**Hauptmerkmale:**
* 🌍 **Nahtlose Mehrsprachigkeit**: Native Unterstützung für Deutsch, Englisch, Chinesisch (Ver/Tra), Japanisch, Koreanisch und Französisch, um allen Spielumgebungen gerecht zu werden.
* 📦 **Intelligente Materialprüfung**: Scannt automatisch Ihr Inventar und Ihre Chocobo-Satteltasche, vergleicht sie in Echtzeit mit dem aktuellen Layout und berechnet genau, welche Möbel Ihnen „fehlen“.
* 📊 **Erweiterter Listen-Export**: Ein-Klick-Export für **Teamcraft-Einkaufslisten** (bereits vorhandene Möbel werden herausgefiltert) oder CSV-Tabellen mit detaillierten Bestandsdaten.
* 🏗️ **Layout-Kompatibilität & Schutz**: Vollständige Abwärtskompatibilität zum Einlesen von `.json`-Bauplänen aus MakePlace und ReMakePlace; unterstützt zudem das Speichern im exklusiven `.almond`-Format mit ChaCha20-Poly1305-Verschlüsselung, um Manipulationen und Diebstahl zu verhindern.

---

### 🚀 Installation

1. **Voraussetzung**: Sie müssen den [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/) verwenden, um das Spiel zu starten.
2. Geben Sie `/xlsettings` in den Spiel-Chat ein, um die Dalamud-Einstellungen zu öffnen.
3. Wechseln Sie zum Reiter **"Experimental" (Experimentell)**.
4. Fügen Sie unter **"Custom Plugin Repositories" (Benutzerdefinierte Plugin-Repositorys)** den folgenden Link hinzu:
   `https://raw.githubusercontent.com/hyirax/AlmondHousing/master/pluginmaster.json`
5. Klicken Sie auf das **+**-Symbol, stellen Sie sicher, dass **"Enabled" (Aktiviert)** angehakt ist, und klicken Sie dann auf "Save and Close" (Speichern und schließen).
6. Suchen Sie im `/xlplugins`-Installationsprogramm nach **`AlmondHousing`** und installieren Sie es.

---

### 🎮 Kurzanleitung

Geben Sie **`/almond`** im Chat ein, um das Bedienfeld zu öffnen.

1. **Layout speichern**: Betreten Sie ein Haus, in dem Sie Rechte haben, aktivieren Sie den integrierten Modus „Drehen“ (Rotate Furniture) und klicken Sie auf `Speichern unter (Save As)`, um die aktuellen Hausdaten zu sichern.
2. **Materialvorbereitung**: Klicken Sie auf den Reiter `Materialprüfung (Material Audit)`, um fehlende Möbel zu überprüfen, und exportieren Sie die Liste für einen präzisen Einkauf.
3. **Layout anwenden**: Legen Sie die benötigten Möbel in Ihr Inventar oder platzieren Sie sie auf dem Boden. Klicken Sie dann auf `Datei auswählen & anwenden (Select File & Apply)`. Das Plugin übernimmt automatisch die Platzierung.

*(Hinweis: Bewegen Sie während der automatischen Platzierung NICHT Ihre Maus und lassen Sie Ihren Charakter nicht den Bewegungspfad der Möbel blockieren, um Spielabstürze zu vermeiden.)*

---

### 💖 Danksagungen

Dieses Projekt wurde auf Basis der folgenden hervorragenden Open-Source-Projekte neu strukturiert und optimiert. Unser Dank gilt den Entwicklern:

* **Drakansoul**: Für die Erstellung und Pflege von DisPlace.
* **NotNite**: Für die erneute Open-Source-Freigabe des MakePlace-Plugins.
* **jawslouis**: Für die Erstellung des bemerkenswerten MakePlacePlugin.
* Besonderer Dank geht an **HousingPos**, **BDTH** und **HouseMate** für die technische Inspiration.

---
*Wenn Ihnen dieses Plugin geholfen hat, geben Sie diesem Projekt bitte einen ⭐️ Star auf GitHub!*