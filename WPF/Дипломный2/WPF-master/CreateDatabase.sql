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
GO

CREATE TABLE Categories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE Statuses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Employees (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Position NVARCHAR(100) NULL,
    Department NVARCHAR(100) NULL
);
GO

CREATE TABLE Equipments (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventoryNumber NVARCHAR(50) NOT NULL UNIQUE,
    Name NVARCHAR(200) NOT NULL,
    SerialNumber NVARCHAR(100) NULL,
    PurchaseDate DATETIME2(7) NULL,
    Price DECIMAL(18,2) NULL,
    Location NVARCHAR(100) NULL,
    Notes NVARCHAR(500) NULL,
    WarrantyExpireDate DATETIME2(7) NULL,
    ServiceLifeYears INT NULL,
    Supplier NVARCHAR(200) NULL,
    CategoryId INT NOT NULL,
    StatusId INT NOT NULL,
    EmployeeId INT NULL,
    CONSTRAINT FK_Equipments_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Equipments_Statuses FOREIGN KEY (StatusId) REFERENCES Statuses(Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Equipments_Employees FOREIGN KEY (EmployeeId) REFERENCES Employees(Id) ON DELETE SET NULL
);
GO

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
GO

CREATE TABLE RepairHistories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    EquipmentId INT NOT NULL,
    DateIn DATETIME2(7) NOT NULL,
    DateOut DATETIME2(7) NULL,
    IssueDescription NVARCHAR(500) NOT NULL,
    Cost DECIMAL(18,2) NULL,
    Contractor NVARCHAR(200) NULL,
    CONSTRAINT FK_RepairHistories_Equipments FOREIGN KEY (EquipmentId) REFERENCES Equipments(Id) ON DELETE CASCADE
);
GO

CREATE INDEX IX_MovementHistories_EquipmentId ON MovementHistories(EquipmentId);
CREATE INDEX IX_MovementHistories_FromEmployeeId ON MovementHistories(FromEmployeeId);
CREATE INDEX IX_MovementHistories_ToEmployeeId ON MovementHistories(ToEmployeeId);
CREATE INDEX IX_RepairHistories_EquipmentId ON RepairHistories(EquipmentId);
CREATE INDEX IX_Equipments_StatusId ON Equipments(StatusId);
CREATE INDEX IX_Equipments_EmployeeId ON Equipments(EmployeeId);
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
('Офисная мебель'), 
('Проекторы'), 
('Периферия'),
('Серверное оборудование');

INSERT INTO Statuses (Name) VALUES 
('В эксплуатации'), ('На складе'), ('В ремонте'), ('Списано');

INSERT INTO Employees (FullName, Position, Department) VALUES 
('Иванов Иван Иванович', 'Системный администратор', 'IT-отдел'),
('Петров Петр Петрович', 'Бухгалтер', 'Бухгалтерия'),
('Сидорова Анна Сергеевна', 'Менеджер по кадрам', 'Отдел кадров'),
('Смирнов Алексей Викторович', 'Директор', 'Руководство'),
('Кузнецова Елена Дмитриевна', 'Главный бухгалтер', 'Бухгалтерия'),
('Попов Дмитрий Олегович', 'Техник', 'IT-отдел'),
('Васильев Игорь Павлович', 'Программист', 'IT-отдел'),
('Морозова Мария Ивановна', 'Секретарь', 'Руководство');

INSERT INTO Equipments (InventoryNumber, Name, SerialNumber, PurchaseDate, Price, Location, CategoryId, StatusId, EmployeeId, Supplier, ServiceLifeYears, WarrantyExpireDate) VALUES 
('INV-001', 'Ноутбук Lenovo ThinkPad L14', 'LR00X123', '2023-05-10', 85000.00, 'Каб. 201', 1, 1, 1, 'ООО ТехноМир', 5, '2025-05-10'),
('INV-002', 'Ноутбук Dell Latitude 5420', 'DL9923XX', '2024-06-15', 92000.00, 'Каб. 305', 1, 1, 4, 'Ситилинк', 5, '2027-06-15'),
('INV-003', 'ПК Kraftway Credo', 'KR445566', '2021-02-20', 45000.00, 'Каб. 102', 1, 1, 2, 'М-Видео', 7, '2024-02-20'),
('INV-004', 'МФУ Kyocera M2040dn', 'KY998877', '2024-08-22', 35000.00, 'Каб. 102', 2, 1, 5, 'ООО ПринтСервис', 4, '2026-08-22'),
('INV-005', 'Принтер Epson L1110', 'EP554433', '2022-12-01', 18000.00, 'Склад', 2, 3, NULL, 'ДНС', 3, '2023-12-01'),
('INV-006', 'Монитор Samsung Odyssey 27"', 'SM776655', '2025-01-10', 25000.00, 'Каб. 201', 3, 1, 7, 'ООО ТехноМир', 5, '2028-01-10'),
('INV-007', 'Коммутатор Cisco Catalyst 2960', 'CS112233', '2023-10-05', 120000.00, 'Серверная', 4, 1, 1, 'Cisco Systems', 10, '2026-10-05'),
('INV-008', 'Проектор BenQ MH560', 'BQ990011', '2024-03-20', 42000.00, 'Переговорная', 6, 1, 8, 'Ситилинк', 5, '2026-03-20'),
('INV-009', 'Сервер HP ProLiant DL380', 'HP556677', '2024-11-11', 350000.00, 'Серверная', 8, 1, 1, 'ООО СерверГрупп', 7, '2027-11-11'),
('INV-010', 'ПК DNS Office Pro', 'DN112211', '2020-05-15', 30000.00, 'Склад', 1, 4, NULL, 'ДНС', 5, '2021-05-15'),
('INV-011', 'МФУ Canon i-SENSYS', 'CN445566', '2025-05-20', 41000.00, 'Каб. 301', 2, 1, 3, 'М-Видео', 4, '2026-05-20'),
('INV-012', 'Монитор Dell P2419H', 'DL778899', '2024-02-10', 19000.00, 'Каб. 102', 3, 2, NULL, 'Ситилинк', 5, '2027-02-10');

INSERT INTO MovementHistories (EquipmentId, FromEmployeeId, ToEmployeeId, TransferDate, Reason) VALUES 
(1, NULL, 1, '2023-05-11', 'Выдача при трудоустройстве'),
(2, NULL, 4, '2024-06-16', 'Оснащение рабочего места директора'),
(4, 2, 5, '2025-01-10', 'Перенос в кабинет главбуха'),
(3, 2, NULL, '2025-02-15', 'Возврат на склад для ТО'),
(3, NULL, 2, '2025-02-20', 'Возврат пользователю после ТО'),
(6, NULL, 7, '2025-01-12', 'Выдача нового оборудования программисту'),
(12, 3, NULL, '2026-01-15', 'Увольнение сотрудника, возврат на склад');

INSERT INTO RepairHistories (EquipmentId, DateIn, DateOut, IssueDescription, Cost, Contractor) VALUES 
(4, '2024-10-10', '2024-10-14', 'Замена ролика захвата бумаги', 3500.00, 'ООО РемонтПлюс'),
(1, '2025-03-05', '2025-03-10', 'Замена кулера и термопасты', 2500.00, 'Внутренний сервис IT'),
(3, '2025-08-12', '2025-08-15', 'Установка нового SSD диска', 4800.00, 'Внутренний сервис IT'),
(8, '2025-11-20', '2025-11-25', 'Замена лампы проектора', 12000.00, 'ООО СервисПроект'),
(2, '2026-01-15', '2026-01-18', 'Ремонт петель крышки матрицы', 6500.00, 'СЦ DellMaster'),
(7, '2026-03-05', '2026-03-15', 'Ремонт блока питания', 15000.00, 'ООО СетьРемонт'),
(5, '2026-05-01', NULL, 'Критическая ошибка печатающей головки', NULL, 'ООО ПринтСервис');