IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'MagnitDeliveryDB')
BEGIN
    CREATE DATABASE MagnitDeliveryDB;
END
GO

USE MagnitDeliveryDB;
GO

SET DATEFORMAT ymd;
GO

IF OBJECT_ID('Deliveries', 'U') IS NOT NULL DROP TABLE Deliveries;
IF OBJECT_ID('Products', 'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Suppliers', 'U') IS NOT NULL DROP TABLE Suppliers;
IF OBJECT_ID('Employees', 'U') IS NOT NULL DROP TABLE Employees;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
GO

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Position NVARCHAR(100) NOT NULL
);

CREATE TABLE Suppliers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    CompanyName NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Inn NVARCHAR(12) NOT NULL
);

CREATE TABLE Products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    CategoryId INT NOT NULL,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) 
        REFERENCES Categories(Id) ON DELETE CASCADE
);

CREATE TABLE Deliveries (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DeliveryDate DATETIME NOT NULL,
    Quantity INT NOT NULL,
    ProductId INT NOT NULL,
    SupplierId INT NOT NULL,
    EmployeeId INT NOT NULL,
    CONSTRAINT FK_Deliveries_Products FOREIGN KEY (ProductId) REFERENCES Products(Id),
    CONSTRAINT FK_Deliveries_Suppliers FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
    CONSTRAINT FK_Deliveries_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);
GO

INSERT INTO Categories (Name) VALUES 
(N'Молочная продукция'),
(N'Мясной гастроном'),
(N'Бакалея'),
(N'Овощи и фрукты');

INSERT INTO Employees (FullName, Position) VALUES 
(N'Иванов Сергей Петрович', N'Товаровед'),
(N'Смирнова Анна Алексеевна', N'Старший смены склада'),
(N'Козлов Дмитрий Владимирович', N'Директор магазина');

INSERT INTO Suppliers (CompanyName, Phone, Inn) VALUES 
(N'ООО Молочный Рай', N'+79991112233', N'6449012345'),
(N'АО Мясокомбинат Балашовский', N'+79992223344', N'6449543210'),
(N'ИП Фруктовый Опт', N'+79993334455', N'6449888777');

INSERT INTO Products (Name, CategoryId) VALUES 
(N'Молоко Простоквашино 2.5%', 1),
(N'Кефир Домик в деревне 3.2%', 1),
(N'Колбаса Докторская ГОСТ', 2),
(N'Сосиски Молочные premium', 2),
(N'Макароны Макфа Перья', 3),
(N'Бананы Эквадор', 4);

INSERT INTO Deliveries (DeliveryDate, Quantity, ProductId, SupplierId, EmployeeId) VALUES 
('2026-06-15T08:30:00', 120, 1, 1, 1),
('2026-06-15T09:15:00', 50, 3, 2, 1),
('2026-06-16T14:00:00', 300, 5, 3, 2),
('2026-06-17T11:45:00', 80, 6, 3, 2);
GO