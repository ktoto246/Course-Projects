IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'Balashov_BoardingHouse')
BEGIN
    CREATE DATABASE Balashov_BoardingHouse;
END
GO

USE Balashov_BoardingHouse;
GO

IF OBJECT_ID('dbo.IssuanceLog', 'U') IS NOT NULL DROP TABLE dbo.IssuanceLog;
IF OBJECT_ID('dbo.Items', 'U') IS NOT NULL DROP TABLE dbo.Items;
IF OBJECT_ID('dbo.Residents', 'U') IS NOT NULL DROP TABLE dbo.Residents;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;
GO

CREATE TABLE dbo.Employees (
    ID INT IDENTITY(1,1) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (ID)
);

CREATE TABLE dbo.Categories (
    ID INT IDENTITY(1,1) NOT NULL,
    CategoryName NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Categories PRIMARY KEY CLUSTERED (ID)
);

CREATE TABLE dbo.Residents (
    ID INT IDENTITY(1,1) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    BirthDate DATE NOT NULL,
    RoomNumber NVARCHAR(10) NOT NULL,
    DisabilityGroup INT NULL,
    CONSTRAINT PK_Residents PRIMARY KEY CLUSTERED (ID)
);

CREATE TABLE dbo.Items (
    ID INT IDENTITY(1,1) NOT NULL,
    ItemName NVARCHAR(150) NOT NULL,
    CategoryID INT NOT NULL,
    TotalQuantity INT NOT NULL DEFAULT 0,
    AvailableQuantity INT NOT NULL DEFAULT 0,
    CONSTRAINT PK_Items PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT FK_Items_Categories FOREIGN KEY (CategoryID) REFERENCES dbo.Categories(ID) ON DELETE CASCADE
);

CREATE TABLE dbo.IssuanceLog (
    ID INT IDENTITY(1,1) NOT NULL,
    ResidentID INT NOT NULL,
    ItemID INT NOT NULL,
    EmployeeID INT NOT NULL,
    IssueDate DATE NOT NULL DEFAULT GETDATE(),
    ReturnDate DATE NULL,
    Quantity INT NOT NULL DEFAULT 1,
    CONSTRAINT PK_IssuanceLog PRIMARY KEY CLUSTERED (ID),
    CONSTRAINT FK_IssuanceLog_Residents FOREIGN KEY (ResidentID) REFERENCES dbo.Residents(ID),
    CONSTRAINT FK_IssuanceLog_Items FOREIGN KEY (ItemID) REFERENCES dbo.Items(ID),
    CONSTRAINT FK_IssuanceLog_Employees FOREIGN KEY (EmployeeID) REFERENCES dbo.Employees(ID)
);
GO

INSERT INTO dbo.Employees (FullName, Position) VALUES
(N'Иванова Мария Петровна', N'Старшая медсестра'),
(N'Петров Сергей Владимирович', N'Заведующий складом'),
(N'Сидорова Анна Николаевна', N'Сестра-хозяйка');

INSERT INTO dbo.Categories (CategoryName) VALUES
(N'Технические средства реабилитации (ТСР)'),
(N'Мягкий инвентарь (Постельное / Одежда)'),
(N'Расходные медицинские материалы'),
(N'Средства гигиены');

INSERT INTO dbo.Residents (FullName, BirthDate, RoomNumber, DisabilityGroup) VALUES
(N'Кузнецов Михаил Иванович', '1945-05-12', '102А', 1),
(N'Смирнова Ольга Борисовна', '1952-10-24', '105', 2),
(N'Васильев Петр Федорович', '1938-02-17', '201Б', 1),
(N'Попова Валентина Сергеевна', '1949-08-30', '102А', NULL);

INSERT INTO dbo.Items (ItemName, CategoryID, TotalQuantity, AvailableQuantity) VALUES
(N'Кресло-коляска с ручным приводом', 1, 10, 8),
(N'Ходунки шагающие для взрослых', 1, 15, 14),
(N'Комплект постельного белья (бязь)', 2, 200, 198),
(N'Матрас ортопедический противопролежневый', 2, 30, 29),
(N'Памперсы для взрослых (размер L)', 4, 500, 450);

INSERT INTO dbo.IssuanceLog (ResidentID, ItemID, EmployeeID, IssueDate, ReturnDate, Quantity) VALUES
(1, 1, 1, '2026-05-10', NULL, 1), 
(2, 2, 2, '2026-05-01', '2026-06-01', 1),
(3, 4, 3, '2026-06-10', NULL, 1),
(1, 5, 1, '2026-06-14', NULL, 50);