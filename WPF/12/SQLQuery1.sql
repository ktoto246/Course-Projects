-- 1. Таблица сотрудников
CREATE TABLE Сотрудники (
    ID_Сотрудника INT PRIMARY KEY IDENTITY(1,1),
    ФИО NVARCHAR(100) NOT NULL,
    Должность NVARCHAR(50) NOT NULL
);

-- 2. Таблица клиентов
CREATE TABLE Клиенты (
    ID_Клиента INT PRIMARY KEY IDENTITY(1,1),
    ФИО NVARCHAR(100) NOT NULL,
    Телефон NVARCHAR(20) NOT NULL,
    Паспорт NVARCHAR(100)
);

-- 3. Таблица категорий
CREATE TABLE Категории (
    ID_Категории INT PRIMARY KEY IDENTITY(1,1),
    Название NVARCHAR(50) NOT NULL,
    Цена_за_час DECIMAL(10, 2) NOT NULL
);

-- 4. Таблица инвентаря
CREATE TABLE Плавсредства (
    ID_Плавсредства INT PRIMARY KEY IDENTITY(1,1),
    ID_Категории INT NOT NULL,
    Модель NVARCHAR(100) NOT NULL,
    Серийный_номер NVARCHAR(50) UNIQUE NOT NULL,
    Состояние NVARCHAR(50) DEFAULT 'Исправно',
    Доступно BIT DEFAULT 1,
    FOREIGN KEY (ID_Категории) REFERENCES Категории(ID_Категории)
);

-- 5. Журнал проката
CREATE TABLE Аренда (
    ID_Аренды INT PRIMARY KEY IDENTITY(1,1),
    ID_Клиента INT NOT NULL,
    ID_Плавсредства INT NOT NULL,
    ID_Сотрудника INT NOT NULL,
    Время_начала DATETIME NOT NULL,
    Время_конца DATETIME NULL,
    Сумма_Залога DECIMAL(10, 2) DEFAULT 0,
    Итого_к_оплате DECIMAL(10, 2) NULL,
    FOREIGN KEY (ID_Клиента) REFERENCES Клиенты(ID_Клиента),
    FOREIGN KEY (ID_Плавсредства) REFERENCES Плавсредства(ID_Плавсредства),
    FOREIGN KEY (ID_Сотрудника) REFERENCES Сотрудники(ID_Сотрудника)
);

-- 6. Журнал поломок
CREATE TABLE Поломки (
    ID_Поломки INT PRIMARY KEY IDENTITY(1,1),
    ID_Аренды INT NOT NULL,
    Описание NVARCHAR(255) NOT NULL,
    Сумма_Штрафа DECIMAL(10, 2) NOT NULL,
    Оплачено BIT DEFAULT 0,
    FOREIGN KEY (ID_Аренды) REFERENCES Аренда(ID_Аренды)
);

-- ЗАЛИВКА ДАННЫХ

INSERT INTO Сотрудники (ФИО, Должность) VALUES
('Иванов Иван Иванович', 'Старший администратор'),
('Петров Петр Петрович', 'Инструктор на причале');

INSERT INTO Клиенты (ФИО, Телефон, Паспорт) VALUES
('Смирнов Алексей Викторович', '+79991234567', '1234 567890'),
('Кузнецова Анна Игоревна', '+79997654321', '0987 654321'),
('Сидоров Олег Олегович', '+78005553535', '1111 222333');

INSERT INTO Категории (Название, Цена_за_час) VALUES
('Гидроцикл', 3000.00),
('Катер', 5000.00),
('SUP-борд', 600.00),
('Весельная лодка', 800.00);

INSERT INTO Плавсредства (ID_Категории, Модель, Серийный_номер, Состояние, Доступно) VALUES
(1, 'Yamaha WaveRunner', 'YAM-001', 'Исправно', 1),
(1, 'Sea-Doo Spark', 'SD-002', 'В ремонте', 0),
(2, 'Bayliner VR5', 'BAY-100', 'Исправно', 1),
(3, 'Gladiator Pro 10.6', 'GL-101', 'Исправно', 1),
(4, 'Пелла-Фиорд', 'PEL-201', 'Исправно', 1);

-- Используем безопасный формат YYYYMMDD hh:mm:ss
INSERT INTO Аренда (ID_Клиента, ID_Плавсредства, ID_Сотрудника, Время_начала, Время_конца, Сумма_Залога, Итого_к_оплате) VALUES
(1, 1, 1, '20260426 10:00:00', '20260426 12:00:00', 5000.00, 6000.00),
(2, 4, 2, '20260427 09:00:00', NULL, 1000.00, NULL);

INSERT INTO Поломки (ID_Аренды, Описание, Сумма_Штрафа, Оплачено) VALUES
(1, 'Глубокая царапина на правом борту', 2500.00, 1);