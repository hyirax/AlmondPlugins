<p align="center">
  <img src="../../../../icons/AlmondHousing.png" width="96" alt="AlmondHousing">
</p>

<h1 align="center">AlmondHousing · Housing Atelier</h1>
<p align="center">Layouts, Möbel und Materialverwaltung für FFXIV-Housing</p>

[简体中文](../../README.md) · [繁體中文](../zh_TW/README.md) · [文言文](../lzh/README.md) · [English](../en/README.md) · [日本語](../ja/README.md) · [한국어](../ko/README.md) · **Deutsch** · [Français](../fr/README.md) · [Русский](../ru/README.md)

## Über AlmondHousing

**AlmondHousing** ist ein von **AlmondCookie** unabhängig entwickeltes und gepflegtes FFXIV-Housing-Werkzeug. Möbelsuche, Layoutverwaltung, Materialprüfung und Datenaustausch sind in einer übersichtlichen Oberfläche vereint.

## Kostenlose Nutzung

**Das Plugin selbst ist kostenlos. Es darf weder weiterverkauft noch hinter einer Bezahlschranke angeboten werden.** Wenn Sie dafür bezahlt haben, verlangen Sie eine Rückerstattung und melden Sie den Verkäufer.

Housing-Layouts können kreative Arbeiten anderer enthalten. Teilen oder handeln Sie nur Daten, für die Sie eine Berechtigung besitzen, und beachten Sie die Bedingungen des ursprünglichen Designers.

## Funktionen

- **Möbel und Bauteile**: Innenmöbel, Gartenmöbel und Bauteile in Echtzeit suchen, kategorisieren und filtern.
- **Layoutverwaltung**: Haus- und Gartenlayouts lesen, zuordnen, anwenden und exportieren; der Export benötigt keine Einrichtungsrechte.
- **Materialprüfung**: Inventar und Chocobo-Satteltaschen prüfen, benötigte, vorhandene und fehlende Mengen vergleichen sowie Teamcraft- oder CSV-Listen exportieren.
- **Layoutkompatibilität**: `.json`-Dateien von MakePlace und ReMakePlace lesen und geteilte Daten als `.almond` speichern.
- **Geschützter Austausch**: Standardverschlüsselung oder ein optionales eigenes Passwort. Bei geschützten Importen erscheint automatisch die Passworteingabe.
- **Präzisionssteuerung**: Koordinaten ziehen, Einrasten, Gruppenbewegung, Rückgängig/Wiederholen und ein 3D-Transformationswerkzeug.

## Installation

1. Starten Sie das Spiel mit [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/).
2. Geben Sie `/xlsettings` im Chat ein, um die Dalamud-Einstellungen zu öffnen.
3. Fügen Sie unter **Experimental** bei **Custom Plugin Repositories** diese URL hinzu:

   `https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json`

4. Aktivieren Sie das Repository und speichern Sie die Einstellungen.
5. Suchen Sie in `/xlplugins` nach **AlmondHousing** und installieren Sie es.

## Schnellstart

1. Öffnen Sie das Plugin mit `/almond`.
2. Betreten Sie ein lesbares Haus oder einen Garten und wählen Sie unter **Layout & Export** die Option **Speichern unter**.
3. Prüfen Sie unter **Materialprüfung** die fehlenden Gegenstände und exportieren Sie bei Bedarf eine Einkaufsliste.
4. Stellen Sie vor dem Anwenden die benötigten Möbel im aktuellen Haus auf, aktivieren Sie den Rotationsmodus und wählen Sie **Aus Datei anwenden**.

Das Plugin ordnet nur Möbel zu und bewegt sie, wenn sie bereits im aktuellen Haus stehen. Lassen Sie den Rotationsmodus aktiv, bewegen Sie keine Möbel von Hand und verlassen Sie das Haus während der Anwendung nicht.

## Layouts und Passwörter

- Ohne Passwort verwendet `.almond` die Standardverschlüsselung.
- Ein eigenes Passwort muss mindestens 6 Zeichen lang sein und bleibt nur für die aktuelle Plugin-Sitzung gespeichert.
- Beim Import einer passwortgeschützten `.almond`-Datei öffnet sich die Passworteingabe. Normale Dateien lösen keine Abfrage aus.

## Danksagung

Drakansoul · NotNite · jawslouis
