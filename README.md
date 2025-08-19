# 📦 GestionStock



![.NET](https://img.shields.io/badge/.NET-6-blue)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-red)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-purple)

---

## 🔹 1. Introduction

**GestionStock** est une application web développée en **ASP.NET Core MVC** pour centraliser et automatiser la gestion des stocks.  

**Objectifs :**
- ⚡ Gestion efficace des stocks avec alertes sur seuils critiques.  
- 📈 Suivi des entrées et sorties de produits.  
- 🏭 Optimisation des entrepôts et de leur capacité.

---

## 🔹 2. Problématique

Le processus actuel est manuel :  
- ❌ Erreurs fréquentes dans les mouvements et quantités.  
- ❌ Ruptures de stock non anticipées.  
- ❌ Difficulté à gérer la capacité des entrepôts.  

💡 Besoin : une solution centralisée, automatisée et fiable.

---

## 🔹 3. Solution Proposée

**Technologies :** ASP.NET Core MVC + Entity Framework  
**Fonctionnalités :**  
- 🛒 **Gestion des produits** : Ajouter, modifier, supprimer, suivi des quantités et seuils.  
- 🔄 **Mouvements de stock** : Entrées et sorties avec dates et quantités.  
- 🏢 **Gestion des entrepôts** : Suivi de la capacité et informations sur les entrepôts.  
- ⚠️ **Alertes** : Notifications pour stock faible.  
- 👤 **Gestion des utilisateurs** : Comptes admin et opérateurs.

---

## 🔹 4. Critique des systèmes existants

- ⚠️ Système manuel avec tableurs  
- ⚠️ Risque élevé d’erreurs humaines  
- ⚠️ Absence d’automatisation pour les alertes  
- ⚠️ Données non centralisées  

---

## 🔹 5. Besoins Fonctionnels

| ID   | Besoin Fonctionnel              | Description |
|------|---------------------------------|------------|
| BF1  | 👤 Gestion des utilisateurs      | Créer, modifier ou supprimer des utilisateurs. |
| BF2  | 🛒 Gestion des produits          | Ajouter, modifier ou supprimer des produits avec nom, prix, seuil et sous-catégorie. |
| BF3  | 📦 Gestion des stocks            | Gestion des quantités, association aux entrepôts, alertes sur stock faible. |
| BF4  | 🔄 Mouvements de stock           | Suivi des entrées et sorties avec date, type et produits concernés. |
| BF5  | 🏢 Gestion des entrepôts         | Ajouter et gérer les entrepôts avec capacité et adresse. |
| BF6  | ⚠️ Alertes sur seuil de stock     | Notifications lorsque le stock est inférieur au seuil minimal. |
| BF7  | 📊 Rapports et exportation       | Exportation des données en PDF ou Excel. |
| BF8  | 🎨 Interface intuitive           | Interface simple et facile à utiliser. |

---



## 🔹6. Technologies utilisées

- **Backend :** ASP.NET Core MVC, Entity Framework Core  
- **Base de données :** SQL Server ou MySQL  
- **Frontend :** HTML, CSS, JavaScript, Bootstrap  

---

✨ **GestionStock** simplifie la gestion des stocks, réduit les erreurs et offre un suivi en temps réel pour optimiser la gestion des produits et entrepôts.
