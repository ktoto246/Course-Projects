CREATE DATABASE GasRegionDb;
GO
USE GasRegionDb;
GO

CREATE TABLE Welders (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(150) NOT NULL,
    StampNumber NVARCHAR(50) NOT NULL
);

CREATE TABLE Pipelines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(200) NOT NULL
);

CREATE TABLE WeldingMachines (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    InventoryNumber NVARCHAR(50) NOT NULL,
    Model NVARCHAR(100) NOT NULL
);

CREATE TABLE Joints (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JointNumber NVARCHAR(50) NOT NULL,
    WelderId INT NOT NULL FOREIGN KEY REFERENCES Welders(Id),
    PipelineId INT NOT NULL FOREIGN KEY REFERENCES Pipelines(Id),
    MachineId INT NOT NULL FOREIGN KEY REFERENCES WeldingMachines(Id),
    WeldingDate DATE NOT NULL
);

CREATE TABLE Inspections (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    JointId INT NOT NULL FOREIGN KEY REFERENCES Joints(Id),
    InspectionMethod NVARCHAR(50) NOT NULL,
    IsPassed BIT NOT NULL,
    InspectionDate DATE NOT NULL
);

INSERT INTO Welders (FullName, StampNumber) VALUES 
('Иванов Иван Иванович', 'КЛ-01'),
('Петров Петр Петрович', 'КЛ-02'),
('Сидоров Сидор Сидорович', 'КЛ-03');

INSERT INTO Pipelines (Name) VALUES 
('КМ 100 - КМ 125, Участок 1'),
('КМ 125 - КМ 150, Участок 2');

INSERT INTO WeldingMachines (InventoryNumber, Model) VALUES 
('INV-1001', 'Lincoln Electric Vantage 500'),
('INV-1002', 'Kemppi MasterTig 335ACDC');

INSERT INTO Joints (JointNumber, WelderId, PipelineId, MachineId, WeldingDate) VALUES 
('С-001', 1, 1, 1, '2026-06-01'),
('С-002', 2, 1, 2, '2026-06-01'),
('С-003', 3, 2, 1, '2026-06-02');

INSERT INTO Inspections (JointId, InspectionMethod, IsPassed, InspectionDate) VALUES 
(1, 'УЗК', 1, '2026-06-02'),
(2, 'Рентген', 0, '2026-06-02'),
(3, 'ВИК', 1, '2026-06-03');