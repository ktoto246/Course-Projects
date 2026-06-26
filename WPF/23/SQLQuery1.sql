CREATE DATABASE KFH_Gorbov_NI;
GO

USE KFH_Gorbov_NI;
GO

CREATE TABLE Employees (
    EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50),
    Position NVARCHAR(50) NOT NULL,
    HireDate DATE NOT NULL,
    Salary DECIMAL(10,2) NOT NULL CHECK (Salary > 0),
    Phone NVARCHAR(20),
    CONSTRAINT CHK_Employees_HireDate CHECK (HireDate <= CAST(GETDATE() AS DATE)),
    CONSTRAINT CHK_Employees_Phone CHECK (LEN(Phone) >= 10)
);

CREATE TABLE Counterparties (
    CounterpartyID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL CHECK (Type IN ('Поставщик', 'Покупатель', 'Прочее')),
    INN NVARCHAR(12),
    Phone NVARCHAR(20)
);

CREATE TABLE Crops (
    CropID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(50) NOT NULL CHECK (Type IN ('зерновые', 'овощи', 'фрукты', 'технические')),
    MaturationDays INT NOT NULL CHECK (MaturationDays > 0),
    SeedCostPerHa DECIMAL(10,2) CHECK (SeedCostPerHa >= 0),
    ExpectedYieldTHa DECIMAL(8,2) CHECK (ExpectedYieldTHa >= 0)
);

CREATE TABLE Plots (
    PlotID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    ResponsibleEmployeeID INT NOT NULL,
    AreaHa DECIMAL(10,2) NOT NULL CHECK (AreaHa > 0 AND AreaHa <= 1000),
    CropID INT,
    PlantingDate DATE,
    PlannedHarvestDate DATE,
    FOREIGN KEY (ResponsibleEmployeeID) REFERENCES Employees(EmployeeID),
    FOREIGN KEY (CropID) REFERENCES Crops(CropID),
    CONSTRAINT CHK_Plots_Dates CHECK (PlannedHarvestDate > PlantingDate OR PlannedHarvestDate IS NULL)
);

CREATE TABLE FinancialTransactions (
    TransactionID INT IDENTITY(1,1) PRIMARY KEY,
    TransactionDate DATE NOT NULL,
    Type NVARCHAR(10) NOT NULL CHECK (Type IN ('доход', 'расход')),
    Amount DECIMAL(15,2) NOT NULL CHECK (Amount > 0),
    Category NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500),
    EmployeeID INT NOT NULL, 
    PlotID INT,             
    CounterpartyID INT,     
    FOREIGN KEY (EmployeeID) REFERENCES Employees(EmployeeID),
    FOREIGN KEY (PlotID) REFERENCES Plots(PlotID),
    FOREIGN KEY (CounterpartyID) REFERENCES Counterparties(CounterpartyID),
    CONSTRAINT CHK_Transactions_Date CHECK (TransactionDate <= CAST(GETDATE() AS DATE))
);
GO

INSERT INTO Employees (LastName, FirstName, MiddleName, Position, HireDate, Salary, Phone) VALUES
('Иванов', 'Петр', 'Сергеевич', 'Главный агроном', '2020-03-15', 55000.00, '+7-915-123-45-67'),
('Сидорова', 'Ольга', 'Владимировна', 'Бухгалтер', '2019-11-20', 48000.00, '+7-916-234-56-78'),
('Петров', 'Алексей', 'Игоревич', 'Тракторист', '2021-05-10', 42000.00, '+7-917-345-67-89'),
('Козлова', 'Мария', 'Дмитриевна', 'Зоотехник', '2020-08-25', 46000.00, '+7-918-456-78-90'),
('Смирнов', 'Дмитрий', 'Александрович', 'Механик', '2018-12-01', 52000.00, '+7-919-567-89-01'),
('Федорова', 'Елена', 'Викторовна', 'Кладовщик', '2022-02-14', 38000.00, '+7-920-678-90-12'),
('Николаев', 'Сергей', 'Петрович', 'Водитель', '2021-07-30', 41000.00, '+7-921-789-01-23'),
('Орлова', 'Анна', 'Сергеевна', 'Экономист', '2020-09-05', 49000.00, '+7-922-890-12-34');

INSERT INTO Counterparties (Name, Type, INN, Phone) VALUES
('ООО АгроСнабТорг', 'Поставщик', '6450123456', '+7-845-222-33-44'),
('АО Росагролизинг', 'Поставщик', '7704217112', '+7-495-739-55-55'),
('ИП Самойлов В.А. (Опт)', 'Покупатель', '645398765432', '+7-903-321-11-22'),
('Саратовский Хлебокомбинат', 'Покупатель', '6455001122', '+7-845-250-60-70'),
('Местная Энергосеть', 'Поставщик', '6452112233', '+7-845-299-88-77');

INSERT INTO Crops (Name, Type, MaturationDays, SeedCostPerHa, ExpectedYieldTHa) VALUES
('Пшеница озимая', 'зерновые', 280, 1500.00, 4.5),
('Ячмень яровой', 'зерновые', 90, 1200.00, 3.8),
('Картофель', 'овощи', 110, 8000.00, 25.0),
('Морковь', 'овощи', 85, 5000.00, 40.0),
('Яблоки Голден', 'фрукты', 365, 25000.00, 15.0),
('Груша Конференц', 'фрукты', 400, 28000.00, 12.0),
('Подсолнечник', 'технические', 120, 3000.00, 2.5),
('Рапс', 'технические', 100, 2500.00, 2.2),
('Овес', 'зерновые', 95, 1100.00, 3.5),
('Капуста белокочанная', 'овощи', 130, 6000.00, 35.0);

INSERT INTO Plots (Name, ResponsibleEmployeeID, AreaHa, CropID, PlantingDate, PlannedHarvestDate) VALUES
('Поле Северное', 1, 50.0, 1, '2024-09-20', '2025-07-15'),
('Поле Южное', 1, 35.0, 2, '2024-04-10', '2024-07-10'),
('Огород Центральный', 4, 5.0, 3, '2024-05-01', '2024-08-20'),
('Сад Яблоневый', 4, 8.0, 5, '2020-03-15', '2024-09-20'),
('Поле Западное', 3, 25.0, 7, '2024-05-15', '2024-09-10'),
('Огород Восточный', 4, 3.0, 4, '2024-04-25', '2024-07-20'),
('Поле Рапсовое', 1, 20.0, 8, '2024-04-05', '2024-07-15'),
('Сад Грушевый', 4, 6.0, 6, '2020-03-20', '2024-10-01'),
('Поле Овсяное', 3, 15.0, 9, '2024-04-15', '2024-07-25'),
('Огород Капустный', 4, 2.5, 10, '2024-05-10', '2024-09-15');

INSERT INTO FinancialTransactions (TransactionDate, Type, Amount, Category, Description, EmployeeID, PlotID, CounterpartyID) VALUES
('2024-01-15', 'расход', 150000.00, 'Закупка семян', 'Покупка семян пшеницы и ячменя', 2, 1, 1),
('2024-01-20', 'расход', 75000.00, 'ГСМ', 'Заправка техники', 5, NULL, 1),
('2024-02-10', 'расход', 50000.00, 'Удобрения', 'Минеральные удобрения для полей', 1, 1, 1),
('2024-03-05', 'доход', 350000.00, 'Продажа зерна', 'Продажа пшеницы прошлого урожая', 2, NULL, 4),
('2024-03-15', 'расход', 120000.00, 'Зарплата', 'Зарплата за февраль', 2, NULL, NULL),
('2024-04-01', 'расход', 25000.00, 'Ремонт техники', 'Ремонт трактора', 5, NULL, 1),
('2024-04-15', 'расход', 80000.00, 'Закупка саженцев', 'Покупка саженцев яблонь', 4, 4, 1),
('2024-05-10', 'доход', 180000.00, 'Продажа овощей', 'Продажа картофеля и моркови', 2, NULL, 3),
('2024-05-20', 'расход', 45000.00, 'Средства защиты', 'Пестициды и гербициды', 1, 2, 1),
('2024-06-05', 'расход', 95000.00, 'Зарплата', 'Зарплата за май', 2, NULL, NULL),
('2024-06-15', 'доход', 120000.00, 'Продажа ягод', 'Продажа клубники', 2, NULL, 3),
('2024-07-01', 'расход', 30000.00, 'Коммунальные услуги', 'Электричество и вода', 2, NULL, 5),
('2024-07-10', 'расход', 60000.00, 'Топливо', 'Дизельное топливо для уборочной', 5, NULL, 1),
('2024-08-15', 'доход', 280000.00, 'Продажа зерна', 'Продажа ячменя нового урожая', 2, 2, 4),
('2024-09-01', 'расход', 110000.00, 'Зарплата', 'Зарплата за август', 2, NULL, NULL);
GO