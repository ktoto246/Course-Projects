CREATE DATABASE BalashovMillDB;
GO

USE BalashovMillDB;
GO

CREATE TABLE Mills (
    MillID INT IDENTITY(1,1) PRIMARY KEY,
    MillName NVARCHAR(100) NOT NULL,
    YearBuilt INT NOT NULL
);

CREATE TABLE Products (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(150) NOT NULL,
    PackagingWeightKG DECIMAL(5,2) NOT NULL,
    PricePerTon DECIMAL(10,2) NOT NULL
);

CREATE TABLE Clients (
    ClientID INT IDENTITY(1,1) PRIMARY KEY,
    ClientName NVARCHAR(200) NOT NULL,
    INN NVARCHAR(12) NOT NULL,
    Phone NVARCHAR(20) NULL,
    LegalAddress NVARCHAR(250) NULL
);

CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    OrderDate DATETIME NOT NULL DEFAULT GETDATE(),
    ClientID INT NOT NULL,
    MillID INT NOT NULL,
    CONSTRAINT FK_Orders_Clients FOREIGN KEY (ClientID) REFERENCES Clients(ClientID) ON DELETE CASCADE,
    CONSTRAINT FK_Orders_Mills FOREIGN KEY (MillID) REFERENCES Mills(MillID)
);

CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT NOT NULL,
    ProductID INT NOT NULL,
    QuantityTons DECIMAL(8,3) NOT NULL, -- Количество в тоннах
    CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY (OrderID) REFERENCES Orders(OrderID) ON DELETE CASCADE,
    CONSTRAINT FK_OrderDetails_Products FOREIGN KEY (ProductID) REFERENCES Products(ProductID)
);
GO

INSERT INTO Mills (MillName, YearBuilt) VALUES 
(N'Мельница №5', 1896),
(N'Мельница №6', 1903);

INSERT INTO Products (ProductName, PackagingWeightKG, PricePerTon) VALUES 
(N'Мука пшеничная высший сорт', 50.00, 24500.00),
(N'Мука пшеничная первый сорт', 50.00, 21000.00),
(N'Мука ржаная обдирная', 45.00, 18500.00),
(N'Отруби пшеничные', 25.00, 8500.00);

INSERT INTO Clients (ClientName, INN, Phone, LegalAddress) VALUES 
(N'ООО Балашовский Хлебокомбинат', '6440012345', '+78454511111', N'г. Балашов, ул. Ленина, д. 10'),
(N'АО Саратов-Хлеб', '6450098765', '+78452222222', N'г. Саратов, пр. Строителей, д. 5'),
(N'ИП Безбородов А.В. (Пекарня)', '644005555555', '+79053334455', N'г. Балашов, ул. Купцов, д. 24');

INSERT INTO Orders (OrderDate, ClientID, MillID) VALUES 
('2026-06-10 09:00:00', 1, 1);

INSERT INTO OrderDetails (OrderID, ProductID, QuantityTons) VALUES 
(1, 1, 15.500),
(1, 4, 3.000);

INSERT INTO Orders (OrderDate, ClientID, MillID) VALUES 
('2026-06-12 14:30:00', 2, 2);

INSERT INTO OrderDetails (OrderID, ProductID, QuantityTons) VALUES 
(2, 3, 20.000);
GO