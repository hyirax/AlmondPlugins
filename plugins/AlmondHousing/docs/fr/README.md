<p align="center">
  <img src="../../../../icons/AlmondHousing.png" width="96" alt="AlmondHousing">
</p>

<h1 align="center">AlmondHousing · Housing Atelier</h1>
<p align="center">Agencements, mobilier et matériaux pour le housing de FFXIV</p>

[简体中文](../../README.md) · [繁體中文](../zh_TW/README.md) · [文言文](../lzh/README.md) · [English](../en/README.md) · [日本語](../ja/README.md) · [한국어](../ko/README.md) · [Deutsch](../de/README.md) · **Français** · [Русский](../ru/README.md)

## À propos

**AlmondHousing** est un outil de housing FFXIV conçu et maintenu indépendamment par **AlmondCookie**. Il réunit la recherche de mobilier, la gestion des agencements, l'inventaire des matériaux et le partage de données dans une interface claire.

## Utilisation gratuite

**Le plugin lui-même est gratuit. Il est interdit de le revendre ou de placer son téléchargement derrière un accès payant.** Si vous avez payé pour l'obtenir, demandez un remboursement et signalez le vendeur.

Les agencements peuvent contenir le travail créatif d'autres personnes. Ne partagez ou n'échangez que les données pour lesquelles vous avez une autorisation et respectez les conditions du créateur d'origine.

## Fonctions

- **Mobilier et éléments** : recherchez, classez et filtrez en temps réel le mobilier intérieur, extérieur et les éléments de logement.
- **Gestion des agencements** : lisez, associez, appliquez et exportez les agencements de maison ou de jardin ; l'export ne demande pas de droits d'aménagement.
- **Inventaire des matériaux** : analysez l'inventaire et les sacoches de chocobo, comparez les quantités requises, possédées et manquantes, puis exportez une liste Teamcraft ou CSV.
- **Compatibilité** : lisez les fichiers `.json` de MakePlace et ReMakePlace, puis enregistrez les données partagées au format `.almond`.
- **Partage protégé** : utilisez le chiffrement standard ou un mot de passe personnalisé facultatif. Les imports protégés affichent automatiquement la saisie du mot de passe.
- **Réglages précis** : déplacement par coordonnées, alignement, mouvement groupé, annulation/rétablissement et gizmo 3D.

## Installation

1. Lancez le jeu avec [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/).
2. Saisissez `/xlsettings` dans le chat pour ouvrir les paramètres de Dalamud.
3. Dans **Experimental**, ajoutez cette URL à **Custom Plugin Repositories** :

   `https://raw.githubusercontent.com/hyirax/AlmondPlugins/main/pluginmaster.json`

4. Activez le dépôt et enregistrez les paramètres.
5. Recherchez **AlmondHousing** dans `/xlplugins` et installez-le.

## Démarrage rapide

1. Saisissez `/almond` pour ouvrir le plugin.
2. Entrez dans une maison ou un jardin lisible, ouvrez **Agencement et export**, puis choisissez **Enregistrer sous**.
3. Ouvrez **Inventaire des matériaux** pour vérifier les objets manquants et exporter une liste d'achats si nécessaire.
4. Avant d'appliquer un agencement, placez le mobilier requis dans la maison actuelle, activez le mode rotation, puis choisissez **Appliquer depuis un fichier**.

Le plugin associe et déplace uniquement le mobilier déjà placé dans la maison actuelle. Gardez le mode rotation actif, ne déplacez rien manuellement et ne quittez pas la maison pendant l'application.

## Agencements et mots de passe

- Sans mot de passe, `.almond` utilise le chiffrement standard.
- Un mot de passe personnalisé doit contenir au moins 6 caractères et n'est conservé que pendant la session actuelle du plugin.
- L'import d'un fichier `.almond` protégé ouvre une demande de mot de passe. Les fichiers ordinaires n'en affichent pas.

## Remerciements

Drakansoul · NotNite · jawslouis
