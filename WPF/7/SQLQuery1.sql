USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'AssetDB')
    DROP DATABASE AssetDB;
GO

CREATE DATABASE AssetDB;
GO

USE AssetDB;
GO

CREATE TABLE Отделы (
    IDОтдела   INT IDENTITY(1,1) PRIMARY KEY,
    Название   NVARCHAR(150) NOT NULL,
    Телефон    NVARCHAR(20)  NULL
);
GO

CREATE TABLE РабочиеМеста (
    IDМеста            INT IDENTITY(1,1) PRIMARY KEY,
    ИнвентарныйНомер   NVARCHAR(50)  NOT NULL UNIQUE,
    Расположение       NVARCHAR(200) NULL,
    IDОтдела           INT NOT NULL,
    CONSTRAINT FK_РМ_Отделы
        FOREIGN KEY (IDОтдела) REFERENCES Отделы(IDОтдела)
);
GO

CREATE TABLE АппаратноеОбеспечение (
    IDОборудования   INT IDENTITY(1,1) PRIMARY KEY,
    Наименование     NVARCHAR(200) NOT NULL,
    Производитель    NVARCHAR(150) NULL,
    Модель           NVARCHAR(150) NULL,
    СерийныйНомер   NVARCHAR(100) NULL,
    ДатаПриобретения DATE          NULL,
    Стоимость        DECIMAL(12,2) NULL,
    IDМеста          INT           NULL,
    CONSTRAINT FK_АО_РМ
        FOREIGN KEY (IDМеста) REFERENCES РабочиеМеста(IDМеста)
);
GO

CREATE TABLE ПрограммноеОбеспечение (
    IDПО             INT IDENTITY(1,1) PRIMARY KEY,
    Наименование     NVARCHAR(200) NOT NULL,
    Версия           NVARCHAR(50)  NULL,
    Разработчик      NVARCHAR(150) NULL,
    ТипЛицензии      NVARCHAR(100) NULL,
    ЛицензионныйКлюч NVARCHAR(200) NULL
);
GO

CREATE TABLE УстановкиПО (
    IDУстановки    INT IDENTITY(1,1) PRIMARY KEY,
    IDПО           INT  NOT NULL,
    IDМеста        INT  NOT NULL,
    ДатаУстановки  DATE NULL,
    CONSTRAINT FK_Уст_ПО
        FOREIGN KEY (IDПО) REFERENCES ПрограммноеОбеспечение(IDПО),
    CONSTRAINT FK_Уст_РМ
        FOREIGN KEY (IDМеста) REFERENCES РабочиеМеста(IDМеста)
);
GO


INSERT INTO Отделы (Название, Телефон) VALUES
(N'Отдел ИТ',     N'1001'),
(N'Бухгалтерия',  N'1002'),
(N'Отдел продаж', N'1003');

INSERT INTO РабочиеМеста (ИнвентарныйНомер, Расположение, IDОтдела) VALUES
(N'РМ-001', N'Каб. 101', 1),
(N'РМ-002', N'Каб. 101', 1),
(N'РМ-003', N'Каб. 205', 2),
(N'РМ-004', N'Каб. 310', 3);

INSERT INTO АппаратноеОбеспечение 
    (Наименование, Производитель, Модель, СерийныйНомер, ДатаПриобретения, Стоимость, IDМеста) VALUES
(N'Системный блок', N'HP',     N'ProDesk 400',  N'SN-001', '2022-01-10', 45000, 1),
(N'Монитор 24"',    N'Dell',   N'P2422H',       N'SN-002', '2022-01-10', 18000, 1),
(N'Системный блок', N'HP',     N'ProDesk 400',  N'SN-003', '2022-03-01', 45000, 2),
(N'Ноутбук',        N'Lenovo', N'ThinkPad E15', N'SN-004', '2022-06-20', 65000, 3),
(N'Системный блок', N'Dell',   N'OptiPlex 7090',N'SN-005', '2023-04-05', 52000, 4);

INSERT INTO ПрограммноеОбеспечение 
    (Наименование, Версия, Разработчик, ТипЛицензии, ЛицензионныйКлюч) VALUES
(N'Windows 10 Pro',        N'22H2',  N'Microsoft',  N'OEM',          N'XXXXX-XXXXX-XXXXX-11111'),
(N'Windows 11 Pro',        N'23H2',  N'Microsoft',  N'OEM',          N'XXXXX-XXXXX-XXXXX-22222'),
(N'Microsoft Office 2021', N'2021',  N'Microsoft',  N'Коммерческая', N'XXXXX-XXXXX-XXXXX-33333'),
(N'Microsoft Office 365',  N'365',   N'Microsoft',  N'Подписка',     N'XXXXX-XXXXX-XXXXX-44444'),
(N'Kaspersky',             N'12.3',  N'Касперский', N'Подписка',     N'KAV-2024-001'),
(N'1С:Предприятие',        N'8.3.25',N'1С',         N'Коммерческая', N'1C-ENT-001'),
(N'Google Chrome',         N'120.0', N'Google',     N'Свободная',    NULL),
(N'Visual Studio 2022',    N'17.9',  N'Microsoft',  N'Коммерческая', N'VS-PRO-2022-001');

INSERT INTO УстановкиПО (IDПО, IDМеста, ДатаУстановки) VALUES
(2, 1, '2023-11-01'),
(4, 1, '2023-11-02'),
(5, 1, '2023-11-02'),
(7, 1, '2024-01-10'),
(8, 1, '2024-01-15'),
(2, 2, '2023-11-05'),
(3, 2, '2023-11-05'),
(5, 2, '2023-11-05'),
(1, 3, '2022-06-25'),
(3, 3, '2022-06-25'),
(5, 3, '2022-06-25'),
(6, 3, '2024-03-10'),
(1, 4, '2023-04-10'),
(4, 4, '2023-04-10'),
(5, 4, '2023-04-10'),
(7, 4, '2023-04-10');

PRINT N'База данных AssetDB создана и заполнена.';
GO