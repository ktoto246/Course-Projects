CREATE DATABASE RetailDB;
GO

USE RetailDB;
GO

CREATE TABLE Categories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);

CREATE TABLE Products (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CategoryId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    SKU NVARCHAR(50) UNIQUE NOT NULL,
    Price DECIMAL(18, 2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);

CREATE TABLE Customers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    Phone NVARCHAR(20) UNIQUE,
    DiscountLevel INT DEFAULT 0
);

CREATE TABLE Employees (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(255) NOT NULL,
    Position NVARCHAR(100)
);

CREATE TABLE Orders (
    Id INT PRIMARY KEY IDENTITY(1,1),
    CustomerId INT NULL,
    EmployeeId INT NOT NULL,
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(18, 2) NOT NULL DEFAULT 0,
    CONSTRAINT FK_Orders_Customers FOREIGN KEY (CustomerId) REFERENCES Customers(Id),
    CONSTRAINT FK_Orders_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);

CREATE TABLE OrderItems (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(18, 2) NOT NULL,
    CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO

INSERT INTO Categories (Name) VALUES ('Электроника'), ('Продукты'), ('Одежда');

INSERT INTO Products (CategoryId, Name, SKU, Price, StockQuantity) VALUES 
(1, 'Смартфон Z-1', 'PH-001', 50000.00, 10),
(1, 'Наушники Noise-X', 'AU-002', 15000.00, 25),
(2, 'Хлеб Бородинский', 'FD-001', 50.00, 100),
(3, 'Футболка White Box', 'CL-001', 1200.00, 50);

INSERT INTO Employees (FullName, Position) VALUES 
('Иванов Иван', 'Старший кассир'),
('Сидорова Анна', 'Менеджер зала');

INSERT INTO Customers (FullName, Phone, DiscountLevel) VALUES 
('Петр Петров', '+79001112233', 5),
('Василий Батарейкин', '+79998887766', 10);

INSERT INTO Orders (CustomerId, EmployeeId, TotalAmount) VALUES (1, 1, 65000.00);

INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice) VALUES 
(1, 1, 1, 50000.00),
(1, 2, 1, 15000.00);
GO