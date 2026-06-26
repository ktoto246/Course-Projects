IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'prikhoperskoe_po')
BEGIN
    CREATE DATABASE prikhoperskoe_po;
END
GO

USE prikhoperskoe_po;
GO

CREATE TABLE districts (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL,
    dispatcher_phone NVARCHAR(20) NOT NULL
);
GO

CREATE TABLE equipment_types (
    id INT IDENTITY(1,1) PRIMARY KEY,
    name NVARCHAR(100) NOT NULL
);
GO

CREATE TABLE brigades (
    id INT IDENTITY(1,1) PRIMARY KEY,
    district_id INT NOT NULL,
    call_sign NVARCHAR(50) NOT NULL,
    leader_name NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_brigades_districts FOREIGN KEY (district_id) 
        REFERENCES districts(id) ON DELETE CASCADE ON UPDATE CASCADE
);
GO

CREATE TABLE incidents (
    id INT IDENTITY(1,1) PRIMARY KEY,
    district_id INT NOT NULL,
    equipment_type_id INT NOT NULL,
    description NVARCHAR(MAX) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    status NVARCHAR(50) DEFAULT N'В обработке',
    CONSTRAINT FK_incidents_districts FOREIGN KEY (district_id) 
        REFERENCES districts(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_incidents_equipment FOREIGN KEY (equipment_type_id) 
        REFERENCES equipment_types(id) ON DELETE CASCADE ON UPDATE CASCADE
);
GO

CREATE TABLE work_logs (
    id INT IDENTITY(1,1) PRIMARY KEY,
    incident_id INT NOT NULL,
    brigade_id INT NOT NULL,
    departure_time DATETIME NOT NULL,
    fix_time DATETIME NULL,
    CONSTRAINT FK_worklogs_incidents FOREIGN KEY (incident_id) 
        REFERENCES incidents(id) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT FK_worklogs_brigades FOREIGN KEY (brigade_id) 
        REFERENCES brigades(id)
);
GO

CREATE TABLE workers (
    id INT IDENTITY(1,1) PRIMARY KEY,
    brigade_id INT NOT NULL,
    full_name NVARCHAR(100) NOT NULL,
    position NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_workers_brigades FOREIGN KEY (brigade_id) 
        REFERENCES brigades(id) ON DELETE CASCADE ON UPDATE CASCADE
);
GO

INSERT INTO districts (name, dispatcher_phone) VALUES
(N'Балашовский РЭС', N'+7-845-45-X-XX-XX'),
(N'Аркадакский РЭС', N'+7-845-42-X-XX-XX'),
(N'Романовский РЭС', N'+7-845-44-X-XX-XX');
GO

INSERT INTO equipment_types (name) VALUES
(N'Воздушная линия (ВЛ) 10 кВ'),
(N'Трансформаторная подстанция (ТП) 10/0.4 кВ'),
(N'Масляный выключатель 35 кВ'),
(N'Опора ЛЭП 0.4 кВ');
GO

INSERT INTO brigades (district_id, call_sign, leader_name) VALUES
(1, N'ОВБ-1 Балашов', N'Иванов И.И.'),
(1, N'Мастер-3 Балашов', N'Петров П.П.'),
(2, N'ОВБ-Аркадак', N'Сидоров С.С.'),
(3, N'ОВБ-Романовка', N'Федоров Ф.Ф.');
GO

INSERT INTO incidents (district_id, equipment_type_id, description, created_at, status) VALUES
(1, 1, N'Обрыв провода из-за падения дерева, фаза А на земле', '20260615 08:30:00', N'Устранено'),
(2, 2, N'Течь масла из бака трансформатора, перегрев', '20260615 11:15:00', N'Бригада выехала'),
(3, 4, N'Наезд грузовика на опору, угроза падения линии', '20260615 12:40:00', N'В обработке');
GO

INSERT INTO work_logs (incident_id, brigade_id, departure_time, fix_time) VALUES
(1, 1, '20260615 08:45:00', '20260615 10:30:00'), 
(2, 3, '20260615 11:30:00', NULL);                  
GO

INSERT INTO workers (brigade_id, full_name, position) VALUES
(1, N'Смирнов А.В.', N'Электромонтер 4 разряда'),
(1, N'Козлов Д.С.', N'Водитель-электромонтер'),
(2, N'Волков Е.А.', N'Электромонтер 3 разряда'),
(3, N'Морозов И.И.', N'Электромонтер 5 разряда'),
(3, N'Зайцев В.П.', N'Машинист автовышки'),
(4, N'Новиков К.М.', N'Электромонтер 4 разряда');
GO