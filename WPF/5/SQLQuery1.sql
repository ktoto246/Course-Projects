CREATE DATABASE InfosecDB;
GO

USE InfosecDB;
GO

CREATE TABLE Сотрудники (
    ID_Сотрудника INT IDENTITY(1,1) PRIMARY KEY,
    ФИО NVARCHAR(150) NOT NULL,
    Должность NVARCHAR(100) NOT NULL,
    Отдел NVARCHAR(100) NOT NULL
);

CREATE TABLE Клиенты (
    ID_Клиента INT IDENTITY(1,1) PRIMARY KEY,
    Название_Компании NVARCHAR(150) NOT NULL,
    Контактное_Лицо NVARCHAR(100) NOT NULL,
    Телефон NVARCHAR(20) NOT NULL
);

CREATE TABLE Продукты (
    ID_Продукта INT IDENTITY(1,1) PRIMARY KEY,
    Название_Продукта NVARCHAR(150) NOT NULL,
    Категория NVARCHAR(50) NOT NULL,
    Базовая_Цена DECIMAL(18,2) NOT NULL
);

CREATE TABLE Сделки (
    ID_Сделки INT IDENTITY(1,1) PRIMARY KEY,
    ID_Клиента INT FOREIGN KEY REFERENCES Клиенты(ID_Клиента),
    ID_Сотрудника INT FOREIGN KEY REFERENCES Сотрудники(ID_Сотрудника),
    ID_Продукта INT FOREIGN KEY REFERENCES Продукты(ID_Продукта),
    Количество INT NOT NULL CHECK (Количество > 0),
    Итоговая_Сумма DECIMAL(18,2) NOT NULL,
    Дата_Сделки DATE NOT NULL DEFAULT GETDATE()
);
GO

INSERT INTO Сотрудники (ФИО, Должность, Отдел)
VALUES 
('Иванов Иван Иванович', 'Программист С++', 'Отдел разработки'),
('Смирнов Алексей Петрович', 'Инженер ИБ', 'Отдел аппаратных решений'),
('Соколова Анна Игоревна', 'Менеджер по продажам', 'Отдел реализации');

INSERT INTO Клиенты (Название_Компании, Контактное_Лицо, Телефон)
VALUES 
('ПАО "МегаБанк"', 'Петров В.В.', '+7-999-111-22-33'),
('ООО "РосТехЗащита"', 'Сидоров А.А.', '+7-900-222-33-44'),
('ГосДепартамент', 'Григорьев И.С.', '+7-495-333-44-55');

INSERT INTO Продукты (Название_Продукта, Категория, Базовая_Цена)
VALUES 
('CyberShield Pro (Межсетевой экран)', 'ПО', 150000.00),
('CryptoGate-X (Аппаратный шифратор)', 'Аппаратное обеспечение', 450000.00),
('ПАК "Стена-Абсолют"', 'ПАК', 1200000.00);

INSERT INTO Сделки (ID_Клиента, ID_Сотрудника, ID_Продукта, Количество, Итоговая_Сумма, Дата_Сделки)
VALUES 
(1, 3, 3, 2, 2400000.00, '2024-04-10'),
(2, 3, 1, 5, 750000.00, '2024-04-25'),
(3, 3, 2, 10, 4500000.00, '2024-04-28');