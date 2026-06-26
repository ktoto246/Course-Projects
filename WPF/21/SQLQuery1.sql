CREATE DATABASE BalashovGrainQualityDB;
GO

USE BalashovGrainQualityDB;
GO

CREATE TABLE Suppliers (
    SupplierID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(250) NULL
);

CREATE TABLE LaboratoryStaff (
    StaffID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    Position NVARCHAR(100) NOT NULL
);

CREATE TABLE GrainBatches (
    BatchID INT IDENTITY(1,1) PRIMARY KEY,
    SupplierID INT NOT NULL,
    DeliveryDate DATETIME NOT NULL DEFAULT GETDATE(),
    VehicleNumber NVARCHAR(20) NOT NULL,
    WeightTons DECIMAL(7,2) NOT NULL,
    CONSTRAINT FK_GrainBatches_Suppliers FOREIGN KEY (SupplierID) 
        REFERENCES Suppliers(SupplierID) ON DELETE CASCADE
);

CREATE TABLE QualityIndicators (
    IndicatorID INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Unit NVARCHAR(20) NOT NULL,
    MinNormalValue DECIMAL(5,2) NOT NULL,
    MaxNormalValue DECIMAL(5,2) NOT NULL
);

CREATE TABLE AnalysisLog (
    AnalysisID INT IDENTITY(1,1) PRIMARY KEY,
    BatchID INT NOT NULL,
    StaffID INT NOT NULL,
    IndicatorID INT NOT NULL,
    AnalysisValue DECIMAL(5,2) NOT NULL,
    AnalysisDate DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_AnalysisLog_GrainBatches FOREIGN KEY (BatchID) 
        REFERENCES GrainBatches(BatchID) ON DELETE CASCADE,
    CONSTRAINT FK_AnalysisLog_LaboratoryStaff FOREIGN KEY (StaffID) 
        REFERENCES LaboratoryStaff(StaffID),
    CONSTRAINT FK_AnalysisLog_QualityIndicators FOREIGN KEY (IndicatorID) 
        REFERENCES QualityIndicators(IndicatorID)
);
GO

INSERT INTO Suppliers (Name, Phone, Address) VALUES 
('ООО Степные Просторы', '+79171234567', 'Саратовская обл., Балашовский р-н, с. Репное'),
('КФХ Иванов И.И.', '+79279876543', 'Саратовская обл., Романовский р-н'),
('ЗАО АгроХолдинг-Волга', '+78452555666', 'г. Саратов, ул. Чернышевского, 10');

INSERT INTO LaboratoryStaff (FullName, Position) VALUES 
('Петрова Анна Сергеевна', 'Старший лаборант'),
('Сидорова Елена Викторовна', 'Техник-лаборант');

INSERT INTO QualityIndicators (Name, Unit, MinNormalValue, MaxNormalValue) VALUES 
('Влажность', '%', 10.00, 14.50),
('Клейковина', '%', 18.00, 32.00),
('Сорная примесь', '%', 0.00, 2.00);

INSERT INTO GrainBatches (SupplierID, DeliveryDate, VehicleNumber, WeightTons) VALUES 
(1, '2026-06-15T08:30:00', 'А123АА64', 24.50),
(2, '2026-06-15T09:15:00', 'Х777ХХ164', 18.20),
(3, '2026-06-16T11:00:00', 'О555ОО64', 32.10);

INSERT INTO AnalysisLog (BatchID, StaffID, IndicatorID, AnalysisValue, AnalysisDate) VALUES 
(1, 1, 1, 13.20, '2026-06-15T08:45:00'),
(1, 1, 2, 24.00, '2026-06-15T08:50:00'),
(1, 1, 3, 1.10, '2026-06-15T08:55:00'),
(2, 2, 1, 15.80, '2026-06-15T09:30:00'),
(2, 2, 2, 21.50, '2026-06-15T09:35:00'),
(2, 2, 3, 2.50, '2026-06-15T09:40:00');
GO