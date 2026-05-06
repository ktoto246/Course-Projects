CREATE DATABASE ПрокатАвтомобилей;
GO

USE ПрокатАвтомобилей;
GO

CREATE TABLE Автомобили (
    ID_Авто INT IDENTITY(1,1) PRIMARY KEY,
    Гос_Номер VARCHAR(20) UNIQUE NOT NULL,
    Марка_Модель VARCHAR(100) NOT NULL,
    Класс VARCHAR(50) NOT NULL,
    Цена_Сутки DECIMAL(10, 2) NOT NULL,
    Год_Выпуска INT NOT NULL,
    Текущий_Пробег INT DEFAULT 0,
    Статус VARCHAR(50) DEFAULT 'Свободен'
);
GO

CREATE TABLE Клиенты (
    ID_Клиента INT IDENTITY(1,1) PRIMARY KEY,
    ФИО VARCHAR(150) NOT NULL,
    Паспорт VARCHAR(50) UNIQUE NOT NULL,
    ВУ VARCHAR(50) UNIQUE NOT NULL,
    Телефон VARCHAR(20) NOT NULL,
    Черный_Список BIT DEFAULT 0 
);
GO

CREATE TABLE Аренда (
    ID_Аренды INT IDENTITY(1,1) PRIMARY KEY,
    ID_Авто INT REFERENCES Автомобили(ID_Авто) ON DELETE CASCADE,
    ID_Клиента INT REFERENCES Клиенты(ID_Клиента) ON DELETE CASCADE,
    Дата_Выдачи DATE NOT NULL,
    Дата_Возврата_План DATE NOT NULL,
    Дата_Возврата_Факт DATE,
    Сумма_Итого DECIMAL(10, 2),
    Залог DECIMAL(10, 2) NOT NULL,
    Статус VARCHAR(50) DEFAULT 'Активна'
);
GO

CREATE TABLE ДТП (
    ID_ДТП INT IDENTITY(1,1) PRIMARY KEY,
    ID_Аренды INT REFERENCES Аренда(ID_Аренды) ON DELETE CASCADE,
    Дата_ДТП DATE NOT NULL,
    Описание TEXT NOT NULL,
    Сумма_Ущерба DECIMAL(10, 2) DEFAULT 0,
    Вина_Клиента BIT NOT NULL
);
GO

INSERT INTO Автомобили (Гос_Номер, Марка_Модель, Класс, Цена_Сутки, Год_Выпуска, Текущий_Пробег) VALUES
('А123АА77', 'Kia Rio', 'Эконом', 2500.00, 2021, 45000),
('В456ВВ77', 'Toyota Camry', 'Комфорт', 5000.00, 2023, 12000),
('С789СС77', 'BMW 520i', 'Бизнес', 12000.00, 2024, 5000);
GO

INSERT INTO Клиенты (ФИО, Паспорт, ВУ, Телефон) VALUES
('Иванов Иван Иванович', '4512 123456', '77АА 123456', '+7(999)123-45-67'),
('Петров Петр Петрович', '4515 654321', '50ВВ 654321', '+7(900)765-43-21'),
('Сидоров Сидор Алексеевич', '4520 111222', '99СС 111222', '+7(926)111-22-33');
GO

INSERT INTO Аренда (ID_Авто, ID_Клиента, Дата_Выдачи, Дата_Возврата_План, Залог, Статус) VALUES
(2, 1, '2026-04-25', '2026-04-28', 10000.00, 'Активна'),
(3, 2, '2026-04-27', '2026-04-29', 30000.00, 'Активна'); 
GO

INSERT INTO Аренда (ID_Авто, ID_Клиента, Дата_Выдачи, Дата_Возврата_План, Дата_Возврата_Факт, Сумма_Итого, Залог, Статус) VALUES
(1, 3, '2026-04-20', '2026-04-22', '2026-04-22', 5000.00, 5000.00, 'Завершена');
GO

INSERT INTO ДТП (ID_Аренды, Дата_ДТП, Описание, Сумма_Ущерба, Вина_Клиента) VALUES
(1, '2026-04-26', 'Въехал в столб при парковке задним ходом. Разбит бампер и фара.', 45000.00, 1);