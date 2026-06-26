CREATE DATABASE ЁлкиТорг;
GO

USE ЁлкиТорг;
GO

CREATE TABLE Clients (
    ClientID INT PRIMARY KEY IDENTITY(1,1),
    CompanyName NVARCHAR(150) NOT NULL,
    INN NVARCHAR(12) NOT NULL UNIQUE,
    Phone NVARCHAR(20) NOT NULL,
    City NVARCHAR(100) DEFAULT N'Балашов'
);

CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY(1,1),
    ModelName NVARCHAR(100) NOT NULL,
    Height DECIMAL(3,2) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    StockQuantity INT NOT NULL DEFAULT 0
);

CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY(1,1),
    ClientID INT NOT NULL,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    OrderStatus NVARCHAR(50) NOT NULL DEFAULT N'Новый',
    CONSTRAINT FK_Orders_Clients FOREIGN KEY (ClientID) 
        REFERENCES Clients(ClientID) ON DELETE CASCADE
);

CREATE TABLE OrderDetails (
    OrderDetailID INT PRIMARY KEY IDENTITY(1,1),
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    Quantity INT NOT NULL CHECK (Quantity > 0),
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) 
        REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) 
        REFERENCES Products(ProductID)
);

INSERT INTO Clients (CompanyName, INN, Phone, City) VALUES
(N'ИП Иванов (Ёлки-Палки)', '644012345678', '+7 (905) 123-45-67', N'Балашов'),
(N'ООО СаратовТоргПлюс', '6450987654', '+7 (8452) 99-88-77', N'Саратов'),
(N'База "Хозтовары на Коммунистической"', '644099887766', '+7 (917) 765-43-21', N'Балашов');

INSERT INTO Products (ModelName, Height, Price, StockQuantity) VALUES
(N'Ёлка "Балашовская Сказка"', 1.50, 2500.00, 150),
(N'Ёлка "Балашовская Сказка"', 1.80, 3900.00, 200),
(N'Ёлка "Балашовская Сказка"', 2.20, 5500.00, 80),
(N'Сосна "Премиум Иней"', 1.80, 4800.00, 120),
(N'Сосна "Премиум Иней"', 2.10, 6200.00, 50),
(N'Кремлевская Элит (литая)', 2.00, 12500.00, 30);

INSERT INTO Orders (ClientID, OrderDate, OrderStatus) VALUES
(1, CONVERT(DATETIME, '2026-06-10 14:30:00', 120), N'Отгружен'),
(2, CONVERT(DATETIME, '2026-06-14 10:15:00', 120), N'В обработке'),
(3, GETDATE(), N'Новый');

INSERT INTO OrderDetails (OrderID, ProductID, Quantity) VALUES
(1, 1, 10), 
(1, 2, 5),  
(2, 2, 50), 
(2, 4, 30), 
(2, 6, 5),  
(3, 3, 15), 
(3, 5, 10);