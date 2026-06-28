use master;
go

if exists(select * from sys.databases where name = 'BMTC_DB') 
begin
    drop database BMTC_DB;
end
go

create database BMTC_DB;
go

use BMTC_DB;
go

create table Employees (
    Id int primary key identity(1,1),
    FullName nvarchar(150) not null,
    Role nvarchar(50) not null
);

create table Clients (
    Id int primary key identity(1,1),
    FullName nvarchar(150) not null,
    Phone nvarchar(15) not null,
    DriverLicense nvarchar(20) not null
);

create table Vehicles (
    Id int primary key identity(1,1),
    ClientId int not null,
    VIN nvarchar(17) not null unique,
    RegNumber nvarchar(10) not null,
    Brand nvarchar(50) not null,
    Model nvarchar(50) not null,
    Category nvarchar(10) not null,
    constraint FK_Vehicles_Clients foreign key (ClientId) 
    references Clients(Id) on delete cascade
);

create table Services(
    Id int primary key identity(1,1),
    Name nvarchar(100) not null,
    Price decimal(10,2) not null
);

create table Inspections(
    Id int primary key identity(1,1),
    VehicleId int not null,
    EmployeeId int not null,
    ServiceId int not null,
    InspectionDate datetime not null default getdate(),
    IsPassed bit not null,
    Comments nvarchar(Max) null,
    constraint FK_Inspections_Vehicles foreign key (VehicleId)
    references Vehicles(Id) on delete cascade,
    constraint FK_Inspections_Employees foreign key (EmployeeId)
    references Employees(Id),
    constraint FK_Inspections_Services foreign key (ServiceId)
    references Services(Id)
);
go

INSERT INTO Employees (FullName, Role) VALUES
(N'Иванов Петр Сергеевич', N'Диагност'),
(N'Админов Алексей Николаевич', N'Администратор');

INSERT INTO Clients (FullName, Phone, DriverLicense) VALUES
(N'Петров Сидор Васильевич', '+79270001122', '64AA123456'),
(N'ООО Балашов-Автотранс', '+78454541111', 'Юр. Лицо');

INSERT INTO Vehicles (ClientId, VIN, RegNumber, Brand, Model, Category) VALUES
(1, 'XTA21070012345678', 'А123АА64', 'LADA', '2107', 'M1'),
(2, 'XTY72311098765432', 'В777ВВ64', 'PAZ', '3205', 'M3');

INSERT INTO Services (Name, Price) VALUES
(N'Технический осмотр легкового автомобиля (M1)', 1022.00),
(N'Технический осмотр автобуса массой до 5 тонн (M2)', 1740.00),
(N'Технический осмотр автобуса массой более 5 тонн (M3)', 2110.00);

INSERT INTO Inspections (VehicleId, EmployeeId, ServiceId, InspectionDate, IsPassed, Comments) VALUES
(1, 1, 1, '20260625 10:30:00', 1, N'Тормозная система и выхлоп в норме.'),
(2, 1, 3, '20260626 14:15:00', 0, N'Неисправна световая сигнализация, повторный осмотр.');