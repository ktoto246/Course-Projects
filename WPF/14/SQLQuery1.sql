IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'BaltexEquipmentDB')
CREATE DATABASE BaltexEquipmentDB;
GO
USE BaltexEquipmentDB;
GO

DROP TABLE IF EXISTS MaintenanceRecords;
DROP TABLE IF EXISTS Equipment;
DROP TABLE IF EXISTS Employees;
DROP TABLE IF EXISTS EquipmentStatuses;
DROP TABLE IF EXISTS EquipmentTypes;
DROP TABLE IF EXISTS Manufacturers;
DROP TABLE IF EXISTS ProductionUnits;
GO

CREATE TABLE ProductionUnits (
    UnitID INT PRIMARY KEY IDENTITY(1,1),
    UnitName NVARCHAR(100) NOT NULL
);

CREATE TABLE Manufacturers (
    ManufacturerID INT PRIMARY KEY IDENTITY(1,1),
    ManufacturerName NVARCHAR(100) NOT NULL,
    Country NVARCHAR(50)
);

CREATE TABLE EquipmentTypes (
    TypeID INT PRIMARY KEY IDENTITY(1,1),
    TypeName NVARCHAR(100) NOT NULL
);

CREATE TABLE EquipmentStatuses (
    StatusID INT PRIMARY KEY IDENTITY(1,1),
    StatusName NVARCHAR(50) NOT NULL
);

CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(150) NOT NULL,
    Position NVARCHAR(100)
);

CREATE TABLE Equipment (
    EquipmentID INT PRIMARY KEY IDENTITY(1,1),
    InventoryNumber NVARCHAR(20) NOT NULL UNIQUE,
    EquipmentName NVARCHAR(150) NOT NULL,
    TypeID INT FOREIGN KEY REFERENCES EquipmentTypes(TypeID),
    ManufacturerID INT FOREIGN KEY REFERENCES Manufacturers(ManufacturerID),
    Model NVARCHAR(100),
    UnitID INT FOREIGN KEY REFERENCES ProductionUnits(UnitID),
    StatusID INT FOREIGN KEY REFERENCES EquipmentStatuses(StatusID),
    InstallDate DATE,
    LastMaintenanceDate DATE,
    NextMaintenanceDate DATE,
    Notes NVARCHAR(500),
    WriteOffDate DATE NULL,
    WriteOffRepairCost DECIMAL(10,2) NULL,
    WriteOffPartsCost DECIMAL(10,2) NULL
);

CREATE TABLE MaintenanceRecords (
    RecordID INT PRIMARY KEY IDENTITY(1,1),
    EquipmentID INT FOREIGN KEY REFERENCES Equipment(EquipmentID),
    EmployeeID INT FOREIGN KEY REFERENCES Employees(EmployeeID),
    MaintenanceDate DATE NOT NULL,
    MaintenanceType NVARCHAR(50) NOT NULL, 
    Description NVARCHAR(500),
    Cost DECIMAL(10,2)
);
GO

INSERT INTO ProductionUnits (UnitName) VALUES
(N'Ткацкое производство'),
(N'Отделочное производство'),
(N'Склад готовой продукции'),
(N'Механическая служба');
GO

INSERT INTO Manufacturers (ManufacturerName, Country) VALUES
(N'CJI', N'Россия'),
(N'Зуккер-Мюллер', N'Германия'),
(N'Н-175-У', N'Чехия'),
(N'СТБ2-180-ШЛ', N'Россия'),
(N'Киото', N'Япония'),
(N'Шолл', N'Швейцария'),
(N'Тисс', N'Германия'),
(N'Меккано-Тессиле', N'Италия'),
(N'Рамиш-Кляйневеферс', N'Германия');
GO

INSERT INTO EquipmentTypes (TypeName) VALUES
(N'Сновальная машина'),
(N'Шлихтовальная машина'),
(N'Гидравлический ткацкий станок'),
(N'Бесчелночный ткацкий станок'),
(N'Красильно-отделочная линия'),
(N'Оборудование для нанесения покрытий'),
(N'Сушильно-термообрабатывающая установка'),
(N'Вспомогательное насосно-компрессорное оборудование');
GO

INSERT INTO EquipmentStatuses (StatusName) VALUES
(N'В работе'),
(N'На плановом ТО'),
(N'В ремонте'),
(N'Резерв'),
(N'Списано');
GO

INSERT INTO Employees (FullName, Position) VALUES
(N'Иванов С.П.', N'Слесарь'),
(N'Петров А.В.', N'Механик'),
(N'Сидоров К.Л.', N'Наладчик'),
(N'Егоров Д.М.', N'Диагност'),
(N'Цехова О.Н.', N'Старший механик');
GO

INSERT INTO Equipment
(InventoryNumber, EquipmentName, TypeID, ManufacturerID, Model, UnitID, StatusID, InstallDate, LastMaintenanceDate, NextMaintenanceDate, Notes, WriteOffDate, WriteOffRepairCost, WriteOffPartsCost) VALUES
(N'BTX-WV-001', N'Сновальная машина', 1, 1, N'CJI-180-Х', 1, 1, '2018-04-12', '2025-09-18', '2025-12-18', N'Используется для подготовки основы под ткачество', NULL, NULL, NULL),
(N'BTX-WV-002', N'Сновальная машина', 1, 1, N'CJI-180-Х', 1, 1, '2019-03-15', '2025-08-22', '2025-11-22', N'Работает в сменном режиме', NULL, NULL, NULL),
(N'BTX-SL-001', N'Шлихтовальная машина', 2, 2, N'ZM-Sizing-1200', 1, 2, '2017-10-05', '2025-10-01', '2025-10-20', N'Плановая проверка секции нанесения шлихты', NULL, NULL, NULL),
(N'BTX-WE-001', N'Ткацкий станок гидравлический', 3, 3, N'N-175-U', 1, 1, '2016-06-20', '2025-09-10', '2025-12-10', N'Основной станок для выпуска суровых тканей', NULL, NULL, NULL),
(N'BTX-WE-002', N'Ткацкий станок бесчелночный', 4, 4, N'STB2-180-SHL', 1, 1, '2015-11-14', '2025-10-03', '2026-01-03', N'Используется для серийного выпуска полотна', NULL, NULL, NULL),
(N'BTX-FI-001', N'Красильно-отделочная линия', 5, 5, N'Kyoto-DyeLine', 2, 1, '2020-02-11', '2025-09-26', '2025-12-26', N'Для гладкокрашеных и отделанных тканей', NULL, NULL, NULL),
(N'BTX-FI-002', N'Линия нанесения покрытий', 6, 6, N'Scholl-CoatPro', 2, 1, '2019-08-30', '2025-10-05', '2026-01-05', N'Нанесение мембранных и полиуретановых покрытий', NULL, NULL, NULL),
(N'BTX-FI-003', N'Сушильно-термообрабатывающая установка', 7, 7, N'Tiss-ThermoDry', 2, 1, '2018-12-18', '2025-09-14', '2025-12-14', N'Термофиксация после отделки', NULL, NULL, NULL),
(N'BTX-MC-001', N'Насосно-компрессорный агрегат', 8, 8, N'MT-Comp-75', 4, 3, '2014-05-22', '2025-10-12', '2025-10-27', N'Требуется проверка пневмосистемы', NULL, NULL, NULL),
(N'BTX-MC-002', N'Вспомогательный компрессор', 8, 9, N'RK-40', 4, 4, '2016-09-09', '2025-06-19', '2025-12-19', N'Резервный агрегат', NULL, NULL, NULL),
(N'BTX-OLD-001', N'Старый компрессор', 8, 8, N'OldComp-90', 4, 5, '2010-01-01', '2024-10-01', NULL, N'Списан из-за морального износа', '2025-05-20', 45000.00, 15000.00);
GO

INSERT INTO MaintenanceRecords
(EquipmentID, EmployeeID, MaintenanceDate, MaintenanceType, Description, Cost) VALUES
(1, 1, '2025-09-18', N'Плановое ТО', N'Проверка направляющих, смазка узлов, калибровка натяжения основы', 18500.00),
(3, 2, '2025-10-01', N'Плановое ТО', N'Осмотр валов, замена изношенных элементов секции нанесения шлихты', 24000.00),
(4, 3, '2025-09-10', N'Плановое ТО', N'Проверка механизмов прокладки утка и систем автоматической остановки', 27000.00),
(6, 4, '2025-09-26', N'Диагностика', N'Проверка форсунок, температурных датчиков и расхода красителя', 14500.00),
(9, 5, '2025-10-12', N'Внеплановый ремонт', N'Замена узла компрессора и проверка пневмолинии', 39800.00);
GO