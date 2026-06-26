IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ElanCheeseDB')
BEGIN
    CREATE DATABASE ElanCheeseDB;
END
GO

USE ElanCheeseDB;
GO

IF OBJECT_ID('dbo.QualityInspections', 'U') IS NOT NULL DROP TABLE dbo.QualityInspections;
IF OBJECT_ID('dbo.CheeseBatches', 'U') IS NOT NULL DROP TABLE dbo.CheeseBatches;
IF OBJECT_ID('dbo.Employees', 'U') IS NOT NULL DROP TABLE dbo.Employees;
IF OBJECT_ID('dbo.StorageChambers', 'U') IS NOT NULL DROP TABLE dbo.StorageChambers;
IF OBJECT_ID('dbo.CheeseVarieties', 'U') IS NOT NULL DROP TABLE dbo.CheeseVarieties;
GO

CREATE TABLE CheeseVarieties (
    Id INT IDENTITY(1,1) NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    MaturationDays INT NOT NULL,
    CONSTRAINT PK_CheeseVarieties PRIMARY KEY CLUSTERED (Id)
);

CREATE TABLE StorageChambers (
    Id INT IDENTITY(1,1) NOT NULL,
    ChamberNumber INT NOT NULL,
    Temperature DECIMAL(4,1) NOT NULL,
    Humidity DECIMAL(4,1) NOT NULL,
    CONSTRAINT PK_StorageChambers PRIMARY KEY CLUSTERED (Id)
);

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) NOT NULL,
    FullName NVARCHAR(150) NOT NULL,
    Position NVARCHAR(100) NOT NULL,
    CONSTRAINT PK_Employees PRIMARY KEY CLUSTERED (Id)
);

CREATE TABLE CheeseBatches (
    Id INT IDENTITY(1,1) NOT NULL,
    VarietyId INT NOT NULL,
    ChamberId INT NOT NULL,
    EmployeeId INT NOT NULL,
    ProductionDate DATE NOT NULL,
    WeightKg DECIMAL(10,2) NOT NULL,
    CONSTRAINT PK_CheeseBatches PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_CheeseBatches_Varieties FOREIGN KEY (VarietyId) REFERENCES CheeseVarieties(Id),
    CONSTRAINT FK_CheeseBatches_Chambers FOREIGN KEY (ChamberId) REFERENCES StorageChambers(Id),
    CONSTRAINT FK_CheeseBatches_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id)
);

CREATE TABLE QualityInspections (
    Id INT IDENTITY(1,1) NOT NULL,
    BatchId INT NOT NULL,
    InspectionDate DATE NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    Notes NVARCHAR(500) NULL,
    CONSTRAINT PK_QualityInspections PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_QualityInspections_Batches FOREIGN KEY (BatchId) REFERENCES CheeseBatches(Id) ON DELETE CASCADE
);
GO

INSERT INTO CheeseVarieties (Name, MaturationDays) VALUES 
(N'Российский', 60),
(N'Голландский', 45),
(N'Маасдам', 90),
(N'Костромской', 45);

INSERT INTO StorageChambers (ChamberNumber, Temperature, Humidity) VALUES 
(101, 12.0, 85.0),
(102, 14.5, 90.0),
(103, 11.0, 82.5);

INSERT INTO Employees (FullName, Position) VALUES 
(N'Иванов Петр Сергеевич', N'Старший технолог'),
(N'Смирнова Анна Владимировна', N'Мастер сыродельного цеха'),
(N'Кузнецов Алексей Николаевич', N'Технолог линии созревания');

INSERT INTO CheeseBatches (VarietyId, ChamberId, EmployeeId, ProductionDate, WeightKg) VALUES 
(1, 1, 2, '2026-04-15', 450.00),
(2, 2, 2, '2026-05-01', 300.50),
(3, 3, 1, '2026-03-10', 600.00),
(4, 1, 3, '2026-05-20', 250.00);

INSERT INTO QualityInspections (BatchId, InspectionDate, Status, Notes) VALUES 
(1, '2026-04-30', N'Зреет', N'Процесс идет нормально, плесени нет.'),
(1, '2026-05-30', N'Зреет', N'Органолептика соответствует норме.'),
(2, '2026-05-15', N'Зреет', N'Влажность в камере была повышена, скорректировали.'),
(3, '2026-06-10', N'Готов к продаже', N'Срок созревания вышел. Качество отличное, отправлен на склад готовой продукции.');