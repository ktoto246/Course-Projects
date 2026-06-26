CREATE DATABASE Учет_СВТ_Оптима;
GO
USE Учет_СВТ_Оптима;
GO

CREATE TABLE Типы_СВТ (
    IDТипа INT PRIMARY KEY IDENTITY(1,1),
    Название NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Сотрудники (
    IDСотрудника INT PRIMARY KEY IDENTITY(1,1),
    ФИО NVARCHAR(150) NOT NULL,
    Отдел NVARCHAR(100)
);

CREATE TABLE Техника (
    IDТехники INT PRIMARY KEY IDENTITY(1,1),
    Инвентарный_номер NVARCHAR(50) UNIQUE NOT NULL,
    Модель NVARCHAR(150) NOT NULL,
    ID_Типа INT NOT NULL,
    ID_Сотрудника INT,
    Статус NVARCHAR(50) DEFAULT N'В работе',
    CONSTRAINT FK_Техника_Типы FOREIGN KEY (ID_Типа) REFERENCES Типы_СВТ(IDТипа),
    CONSTRAINT FK_Техника_Сотрудники FOREIGN KEY (ID_Сотрудника) REFERENCES Сотрудники(IDСотрудника) ON DELETE SET NULL
);

CREATE TABLE Комплектующие (
    IDКомплектующего INT PRIMARY KEY IDENTITY(1,1),
    Название NVARCHAR(100) NOT NULL, 
    Модель NVARCHAR(150),
    Серийный_номер NVARCHAR(100) UNIQUE,
    ID_Техники INT,
    CONSTRAINT FK_Комплектующие_Техника FOREIGN KEY (ID_Техники) REFERENCES Техника(IDТехники) ON DELETE SET NULL
);

INSERT INTO Типы_СВТ (Название) VALUES (N'Системный блок'), (N'Ноутбук'), (N'Монитор');
INSERT INTO Сотрудники (ФИО, Отдел) VALUES (N'Иванов И.И.', N'Бухгалтерия'), (N'Петров П.П.', N'IT');
INSERT INTO Техника (Инвентарный_номер, Модель, ID_Типа, ID_Сотрудника) VALUES (N'СВТ-001', N'HP EliteDesk', 1, 1), (N'СВТ-002', N'MacBook Pro 14', 2, 2);
INSERT INTO Комплектующие (Название, Модель, Серийный_номер, ID_Техники) VALUES (N'Процессор', N'Intel i7-12700', N'SN-990011', 1), (N'RAM', N'Crucial 16GB', N'SN-880022', 1), (N'SSD', N'Samsung 1TB', N'SN-770033', 1);