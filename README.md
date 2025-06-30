# RecipeShare App

RecipeShare is a full-stack application that allows users to create, browse, and manage recipes. It is built with an Angular frontend and a .NET 8 Web API backend, using SQL Server for persistence.

## 🏗 Architecture Overview

```text
┌──────────────────────────┐
│       Angular Frontend   │
│ (RecipeShareClientV2)    │
│                          │
│ • Login                  │
│ • Recipe list/form       │
│ • Auth via JWT           │
└──────────┬───────────────┘
           │ HTTP (API + Static files)
┌──────────▼───────────────┐
│        ASP.NET Core API  │
│    (RecipeShare.API)     │
│                          │
│ • JWT Authentication     │
│ • Controllers + Services │
│ • Entity Framework Core  │
└──────────┬───────────────┘
           │ SQL
┌──────────▼───────────────┐
│        SQL Server DB     │
│  (recipes, users, etc.)  │
└──────────────────────────┘


## ? Features

- ? ASP.NET Core 6 Web API with EF Core
- ? Full CRUD for recipes
- ? Filtering by dietary tags
- ? Client-side Angular UI
- ? Server-side and client-side validation
- ? Unit tests with xUnit & Moq

## ?? Benchmark Results

![Performance Test Results for GET /api/recipes](assets/benchmark.png)

**500 GET calls to `/api/recipes` in Release mode**