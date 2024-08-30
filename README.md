# MyMvcApp
.Net MVC Application build over on VS Code and using Ms SQL Server

# Database Setup
CREATE DATABASE demo;

CREATE TABLE Products(
	Id INT PRIMARY KEY IDENTITY(1,1),
	Name NVARCHAR(100) NOT NULL,
	Price DECIMAL(18,2) NOT NULL,
	Stock INT NOT NULL
);

CREATE TABLE Purchases (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ProductId INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    PurchaseDate DATETIME NOT NULL,
    FOREIGN KEY (ProductId) REFERENCES Products(Id)
);

# Packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package ClosedXML

dotnet ef migrations add AddPurchaseModel
dotnet ef database update
