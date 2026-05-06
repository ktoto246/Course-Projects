SET DATEFORMAT ymd;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'Регистратура_БД')
    DROP DATABASE Регистратура_БД;
GO

CREATE DATABASE Регистратура_БД;
GO

USE Регистратура_БД;
GO

CREATE TABLE Специальности (
    ID INT PRIMARY KEY IDENTITY(1,1),
    Название NVARCHAR(100) NOT NULL
);

CREATE TABLE Врачи (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ФИО NVARCHAR(255) NOT NULL,
    ID_Специальности INT,
    Кабинет INT,
    FOREIGN KEY (ID_Специальности) REFERENCES Специальности(ID)
);

CREATE TABLE Пациенты (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ФИО NVARCHAR(255) NOT NULL,
    Дата_Рождения DATE NOT NULL,
    Телефон NVARCHAR(20),
    Адрес NVARCHAR(MAX)
);

CREATE TABLE Приемы (
    ID INT PRIMARY KEY IDENTITY(1,1),
    ID_Пациента INT NOT NULL,
    ID_Врача INT NOT NULL,
    Дата_Приема DATETIME NOT NULL,
    Статус NVARCHAR(50) DEFAULT 'Запланировано',
    FOREIGN KEY (ID_Пациента) REFERENCES Пациенты(ID),
    FOREIGN KEY (ID_Врача) REFERENCES Врачи(ID)
);

INSERT INTO Специальности (Название) VALUES 
('Терапевт'), ('Хирург'), ('Окулист'), ('Невролог');

INSERT INTO Врачи (ФИО, ID_Специальности, Кабинет) VALUES 
('Иванов Иван Иванович', 1, 101),
('Петров Петр Петрович', 2, 202),
('Сидорова Анна Сергеевна', 3, 305);

INSERT INTO Пациенты (ФИО, Дата_Рождения, Телефон, Адрес) VALUES 
('Кузнецов Алексей', '1990-05-15', '+79001112233', 'ул. Пушкина, д. Колотушкина'),
('Смирнова Елена', '1985-10-20', '+79998887766', 'пр. Ленина, д. 10');


INSERT INTO Приемы (ID_Пациента, ID_Врача, Дата_Приема, Статус) VALUES 
(1, 1, '2026-05-10 09:00:00', 'Запланировано'),
(2, 3, '2026-05-10 11:30:00', 'Запланировано'),
(1, 2, '2026-04-20 14:00:00', 'Завершено');