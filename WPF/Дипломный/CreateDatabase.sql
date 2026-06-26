DROP DATABASE IF EXISTS EquipmentAccountingDb;
GO

CREATE DATABASE EquipmentAccountingDb;
GO

USE EquipmentAccountingDb;
GO

CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Login NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(50) NOT NULL,
    Role NVARCHAR(20) NOT NULL
);

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Statuses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Positions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Departments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    PositionId INT NULL,
    DepartmentId INT NULL,
    CONSTRAINT FK_Employees_Positions FOREIGN KEY (PositionId) REFERENCES Positions(Id) ON DELETE SET NULL,
    CONSTRAINT FK_Employees_Departments FOREIGN KEY (DepartmentId) REFERENCES Departments(Id) ON DELETE SET NULL
);

CREATE TABLE Cabinets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Number NVARCHAR(50) NOT NULL UNIQUE,
    ResponsibleEmployeeId INT NULL,
    CONSTRAINT FK_Cabinets_Employees FOREIGN KEY (ResponsibleEmployeeId) REFERENCES Employees(Id) ON DELETE NO ACTION
);

CREATE TABLE Equipments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventoryNumber NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(200) NOT NULL,
    SerialNumber NVARCHAR(100) NULL,
    PurchaseDate DATETIME2(7) NULL,
    Price DECIMAL(18,2) NULL,
    CabinetId INT NULL,
    Notes NVARCHAR(500) NULL,
    WarrantyExpireDate DATETIME2(7) NULL,
    ServiceLifeYears INT NULL,
    Supplier NVARCHAR(200) NULL,
    CategoryId INT NOT NULL,
    StatusId INT NOT NULL,
    EmployeeId INT NULL,
    CONSTRAINT FK_Equipments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Equipments_Statuses FOREIGN KEY (StatusId) REFERENCES Statuses(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Equipments_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE SET NULL,
    CONSTRAINT FK_Equipments_Cabinets FOREIGN KEY (CabinetId) REFERENCES Cabinets(Id) ON DELETE SET NULL
);

CREATE TABLE MovementHistories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentId INT NOT NULL,
    FromEmployeeId INT NULL,
    ToEmployeeId INT NULL,
    TransferDate DATETIME2(7) NOT NULL,
    Reason NVARCHAR(250) NULL,
    CONSTRAINT FK_MovementHistories_Equipments FOREIGN KEY (EquipmentId) REFERENCES Equipments(Id) ON DELETE CASCADE,
    CONSTRAINT FK_MovementHistories_Employees_From FOREIGN KEY (FromEmployeeId) REFERENCES Employees(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_MovementHistories_Employees_To FOREIGN KEY (ToEmployeeId) REFERENCES Employees(Id) ON DELETE NO ACTION
);

CREATE TABLE RepairHistories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentId INT NOT NULL,
    DateIn DATETIME2(7) NOT NULL,
    DateOut DATETIME2(7) NULL,
    IssueDescription NVARCHAR(500) NOT NULL,
    Cost DECIMAL(18,2) NULL,
    Contractor NVARCHAR(200) NULL,
    IsWarrantyRepair BIT NOT NULL DEFAULT 0,
    CONSTRAINT FK_RepairHistories_Equipments FOREIGN KEY (EquipmentId) REFERENCES Equipments(Id) ON DELETE CASCADE
);
GO

INSERT INTO Users (Login, Password, Role) VALUES 
('admin', 'admin', 'Admin'), 
('oper', 'oper', 'Operator'), 
('user', '1234', 'Viewer');

INSERT INTO Categories (Name) VALUES 
('Компьютеры и ноутбуки'), 
('Принтеры и МФУ'), 
('Мониторы'), 
('Сетевое оборудование'), 
('Сетевые фильтры и ИБП'), 
('Проекторы'), 
('Периферия'), 
('Серверное оборудование');

INSERT INTO Statuses (Name) VALUES 
('В эксплуатации'), 
('На складе'), 
('В ремонте'), 
('Списано');

INSERT INTO Positions (Name) VALUES 
('Системный администратор'), 
('Бухгалтер'), 
('Менеджер по кадрам'), 
('Директор'), 
('Главный бухгалтер'), 
('Техник'), 
('Программист'), 
('Секретарь');

INSERT INTO Departments (Name) VALUES 
('IT-отдел'), 
('Бухгалтерия'), 
('Отдел кадров'), 
('Руководство');

INSERT INTO Employees (FullName, PositionId, DepartmentId) VALUES 
('Иванов Иван Иванович', 1, 1),
('Петров Петр Петрович', 2, 2),
('Сидорова Анна Сергеевна', 3, 3),
('Смирнов Алексей Викторович', 4, 4),
('Кузнецова Елена Дмитриевна', 5, 2),
('Попов Дмитрий Олегович', 6, 1),
('Васильев Игорь Павлович', 7, 1),
('Морозова Мария Ивановна', 8, 4);

INSERT INTO Cabinets (Number, ResponsibleEmployeeId) VALUES 
('Каб. 201', 1), 
('Каб. 305', 4), 
('Каб. 102', 2), 
('Каб. 301', 3), 
('Серверная', 1), 
('Переговорная', 8), 
('Склад', NULL);

INSERT INTO Equipments (InventoryNumber, Name, SerialNumber, PurchaseDate, Price, CabinetId, CategoryId, StatusId, EmployeeId, Supplier, ServiceLifeYears, WarrantyExpireDate) VALUES 
('INV-001', 'Ноутбук Lenovo ThinkPad L14', 'LR00X123', '2023-05-10', 85000.00, 1, 1, 1, 1, 'ООО ТехноМир', 5, '2025-05-10'),
('INV-002', 'Ноутбук Dell Latitude 5420', 'DL9923XX', '2024-06-15', 92000.00, 2, 1, 1, 4, 'Ситилинк', 5, '2027-06-15'),
('INV-003', 'ПК Kraftway Credo', 'KR445566', '2021-02-20', 45000.00, 3, 1, 1, 2, 'М-Видео', 7, '2024-02-20'),
('INV-004', 'МФУ Kyocera M2040dn', 'KY998877', '2024-08-22', 35000.00, 3, 2, 1, 5, 'ООО ПринтСервис', 4, '2026-08-22'),
('INV-005', 'Принтер Epson L1110', 'EP554433', '2022-12-01', 18000.00, 7, 2, 3, NULL, 'ДНС', 3, '2023-12-01'),
('INV-006', 'Монитор Samsung Odyssey 27"', 'SM776655', '2025-01-10', 25000.00, 1, 3, 1, 7, 'ООО ТехноМир', 5, '2028-01-10'),
('INV-007', 'Коммутатор Cisco Catalyst 2960', 'CS112233', '2023-10-05', 120000.00, 5, 4, 1, 1, 'Cisco Systems', 10, '2026-10-05'),
('INV-008', 'Проектор BenQ MH560', 'BQ990011', '2024-03-20', 42000.00, 6, 6, 1, 8, 'Ситилинк', 5, '2026-03-20'),
('INV-009', 'Сервер HP ProLiant DL380', 'HP556677', '2024-11-11', 350000.00, 5, 8, 1, 1, 'ООО СерверГрупп', 7, '2027-11-11'),
('INV-010', 'ПК DNS Office Pro', 'DN112211', '2020-05-15', 30000.00, 7, 1, 4, NULL, 'ДНС', 5, '2021-05-15'),
('INV-011', 'ИБП APC Smart-UPS 1500', 'APC88811', '2025-03-12', 48000.00, 5, 5, 1, 1, 'Ситилинк', 5, '2028-03-12'),
('INV-012', 'Сетевой фильтр Pilot Pro', 'PL773322', '2025-04-10', 3500.00, 2, 5, 1, 4, 'ДНС', 3, '2026-04-10');

INSERT INTO RepairHistories (EquipmentId, DateIn, DateOut, IssueDescription, Cost, Contractor, IsWarrantyRepair) VALUES 
(4, '2024-10-10', '2024-10-14', 'Замена ролика захвата бумаги', 3500.00, 'ООО РемонтПлюс', 0),
(1, '2025-03-05', '2025-03-10', 'Замена кулера и термопасты', 2500.00, 'Внутренний сервис IT', 0),
(3, '2025-08-15', '2025-08-20', 'Ремонт цепей питания материнской платы', 4800.00, 'ООО ТехноСервис', 0),
(7, '2025-11-01', '2025-11-05', 'Замена сгоревшего сетевого порта', 12000.00, 'Cisco Трейд', 0),
(2, '2026-01-15', '2026-01-18', 'Ремонт петель крышки матрицы ноутбука', 6500.00, 'ООО РемонтПлюс', 0),
(9, '2026-03-10', '2026-03-15', 'Замена вышедшего из строя блока питания', 15000.00, 'ООО СерверГрупп', 0),
(5, '2026-05-01', NULL, 'Критическая ошибка печатающей головки', NULL, 'ООО ПринтСервис', 1);

INSERT INTO MovementHistories (EquipmentId, FromEmployeeId, ToEmployeeId, TransferDate, Reason) VALUES 
(1, 2, 1, '2023-05-10', 'Первичная выдача системному администратору'),
(2, 3, 4, '2024-06-15', 'Оснащение рабочего места директора'),
(3, 1, 2, '2025-01-20', 'Передача компьютера в бухгалтерию Петрову'),
(4, 4, 5, '2024-08-22', 'Выдача МФУ главному бухгалтеру');