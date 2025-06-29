IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'RecipeShareDb')
BEGIN
    CREATE DATABASE [RecipeShareDb];
END;
GO