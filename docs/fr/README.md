# 🏡 AlmondHousing

*🌐 Languages: [简体中文](../../README.md) | [繁體中文](../zh_TW/README.md) | [English](../en/README.md) | [日本語](../ja/README.md) | [한국어](../ko/README.md) | [Deutsch](../de/README.md) | **[Français](README.md)***

---

### 🛑 Avertissement strict contre la revente & Respect des créateurs 🛑

**Ce plugin est 100% GRATUIT et OPEN-SOURCE ! La vente de ce logiciel sous quelque forme que ce soit est strictement interdite !**

L'objectif initial de cet outil est d'aider les joueurs à **sauvegarder leurs propres décorations** et à **partager gratuitement des plans (blueprints) entre les régions**. Nous ne tolérons absolument pas l'utilisation de cet outil pour voler les conceptions originales d'autres joueurs ou designers à des fins commerciales, de commandes ou de revente !
Si vous avez obtenu ce plugin via un canal payant, ou si vous trouvez quelqu'un revendant des plans créés avec celui-ci, veuillez **demander un remboursement immédiatement et signaler le vendeur**.

**Le savoir est gratuit. Respectez les créateurs originaux. Dites NON à la revente !**

---

### ✨ Introduction

**AlmondHousing** est un plugin d'amélioration du logement FFXIV, profondément reconstruit et optimisé à partir de [DisPlace](https://github.com/Drakansoul/DisPlace), et maintenu indépendamment par **AlmondCookie**. Il est dédié à rendre la décoration de logement complexe, la gestion des matériaux et le partage de données plus geek, pur et simple.

**Caractéristiques principales :**
* 🌍 **Support multilingue transparent** : Support natif du français, de l'anglais, du chinois (Simp/Trad), du japonais, du coréen et de l'allemand pour répondre à tous les environnements de jeu.
* 📦 **Inventaire intelligent des matériaux** : Analyse automatiquement votre inventaire et votre sacoche de chocobo, les compare en temps réel avec l'agencement actuel et calcule exactement les meubles qui vous "manquent".
* 📊 **Exportation avancée de listes** : Exportation en un clic pour les **listes d'achat Teamcraft** (les meubles déjà possédés sont filtrés) ou des tableaux CSV avec des données détaillées sur les surplus et les manques.
* 🏗️ **Compatibilité et protection des agencements** : Rétrocompatibilité parfaite pour lire les plans `.json` de MakePlace et ReMakePlace ; supporte également l'enregistrement au format crypté exclusif `.almond` utilisant l'algorithme ChaCha20-Poly1305 pour empêcher toute falsification et vol.

---

### 🚀 Installation

1. **Prérequis** : Vous devez utiliser le [Dalamud / FFXIV Quick Launcher](https://goatcorp.github.io/) pour lancer le jeu.
2. Tapez `/xlsettings` dans le chat du jeu pour ouvrir les paramètres de Dalamud.
3. Accédez à l'onglet **"Experimental" (Expérimental)**.
4. Ajoutez le lien suivant dans la section **"Custom Plugin Repositories" (Dépôts de plugins personnalisés)** :
   `https://raw.githubusercontent.com/hyirax/AlmondHousing/master/pluginmaster.json`
5. Cliquez sur l'icône **+**, assurez-vous que **"Enabled" (Activé)** est coché, puis cliquez sur "Save and Close" (Sauvegarder et fermer).
6. Recherchez **`AlmondHousing`** dans l'installateur `/xlplugins` et installez-le.

---

### 🎮 Guide de démarrage rapide

Tapez **`/almond`** dans le chat pour ouvrir le panneau de contrôle.

1. **Sauvegarder un agencement** : Entrez dans une maison où vous avez les permissions, activez le mode intégré "Rotation" (Rotate Furniture), et cliquez sur `Sauvegarder sous (Save As)` pour sauvegarder les données actuelles de la maison.
2. **Préparer les matériaux** : Cliquez sur l'onglet `Inventaire (Material Audit)` pour voir quels meubles manquent et exportez la liste pour des achats précis.
3. **Appliquer un agencement** : Placez les meubles requis dans votre inventaire ou sur le sol, puis cliquez sur `Sélectionner un fichier et appliquer (Select File & Apply)`. Le plugin positionnera tout automatiquement.

*(Remarque : NE BOUGEZ PAS votre souris et ne laissez pas votre personnage bloquer le chemin de déplacement des meubles pendant le placement automatique afin d'éviter les crashs du jeu.)*

---

### 💖 Remerciements

Ce projet a été reconstruit et optimisé sur la base des excellents projets open-source suivants. Merci aux développeurs :

* **Drakansoul** : Pour la création et la maintenance de DisPlace.
* **NotNite** : Pour avoir forcé la réouverture du code source du plugin MakePlace.
* **jawslouis** : Pour avoir créé le remarquable MakePlacePlugin.
* Remerciements particuliers à **HousingPos**, **BDTH** et **HouseMate** pour l'inspiration technique apportée par leurs travaux.

---
*Si vous trouvez ce plugin utile, n'hésitez pas à donner une ⭐️ Star à ce projet sur GitHub !*