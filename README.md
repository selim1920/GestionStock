# GestionStock

**Auteur :** Selim Merzoug  
**Année scolaire :** 2024-2025  

## 1. Introduction

GestionStock est une application web développée en **ASP.NET Core MVC** visant à centraliser et automatiser la gestion des stocks au sein d’une entreprise.  
Elle permet de gérer les produits, les entrepôts, les mouvements de stock, et les utilisateurs, avec des alertes en cas de seuil critique.

**Objectifs :**
- Gestion efficace des stocks avec alertes sur seuils critiques.
- Suivi des entrées et sorties de produits.
- Optimisation des entrepôts et de leur capacité.

## 2. Problématique

Le processus actuel est manuel, ce qui entraîne :
- Erreurs fréquentes dans les mouvements et les quantités de stock.
- Ruptures de stock non anticipées.
- Difficulté à gérer la capacité des entrepôts.

L’entreprise a besoin d’une solution centralisée, automatisée et fiable.

## 3. Solution Proposée

L’application est développée avec **ASP.NET Core MVC** et utilise **Entity Framework** pour la gestion des données.  
Elle offre les fonctionnalités principales suivantes :

- **Gestion des produits** : Ajouter, modifier, supprimer des produits ; suivi des quantités et des seuils d'alerte.  
- **Mouvements de stock** : Suivi des entrées et sorties avec dates et quantités.  
- **Gestion des entrepôts** : Suivi des informations sur les entrepôts et leur capacité.  
- **Alertes** : Notifications lorsque le stock est sous le seuil défini.  
- **Gestion des utilisateurs** : Création et gestion des comptes administrateurs et opérateurs.

## 4. Critique des systèmes existants

- Système manuel avec tableurs.  
- Risque élevé d’erreurs humaines.  
- Pas d’automatisation pour les alertes.  
- Difficulté à suivre les mouvements et à anticiper les ruptures.  
- Données non centralisées.

## 5. Besoins Fonctionnels

| ID   | Besoin Fonctionnel              | Description |
|------|---------------------------------|------------|
| BF1  | Gestion des utilisateurs        | Créer, modifier ou supprimer des utilisateurs. |
| BF2  | Gestion des produits            | Ajouter, modifier ou supprimer des produits avec nom, prix, seuil et sous-catégorie. |
| BF3  | Gestion des stocks              | Gestion des quantités, association aux entrepôts, alertes sur stock faible. |
| BF4  | Mouvements de stock             | Suivi des entrées et sorties avec date, type et produits concernés. |
| BF5  | Gestion des entrepôts           | Ajouter et gérer les entrepôts avec capacité et adresse. |
| BF6  | Alertes sur seuil de stock      | Notifications lorsque le stock est inférieur au seuil minimal. |
| BF7  | Rapports et exportation         | Exportation des données en PDF ou Excel. |
| BF8  | Interface utilisateur intuitive | Interface simple et facile à utiliser pour gérer stocks et utilisateurs. |

## 6. Technologies utilisées

- **Backend** : ASP.NET Core MVC, Entity Framework Core  
- **Base de données** : SQL Server ou MySQL  
- **Frontend** : HTML, CSS, JavaScript, Bootstrap  

---

**GestionStock** permet de moderniser la gestion des stocks, de réduire les erreurs manuelles et de fournir une meilleure visibilité et contrôle sur les produits et entrepôts.
