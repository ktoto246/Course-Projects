CREATE DATABASE CollegeDB;
GO

USE CollegeDB;
GO

CREATE TABLE Teachers (
    TeacherId INT IDENTITY(1,1) PRIMARY KEY,
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50) NULL,
    Phone NVARCHAR(20) NULL,
    Email NVARCHAR(100) NULL,
    Position NVARCHAR(50) NOT NULL
);

CREATE TABLE Groups (
    GroupId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(20) NOT NULL,
    Specialty NVARCHAR(100) NOT NULL,
    SpecialtyCode NVARCHAR(20) NOT NULL,
    Course INT NOT NULL,
    CuratorId INT NULL FOREIGN KEY REFERENCES Teachers(TeacherId)
);

CREATE TABLE Students (
    StudentId INT IDENTITY(1,1) PRIMARY KEY,
    LastName NVARCHAR(50) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    MiddleName NVARCHAR(50) NULL,
    RecordBookNumber NVARCHAR(15) NOT NULL,
    GroupId INT NOT NULL FOREIGN KEY REFERENCES Groups(GroupId),
    Phone NVARCHAR(20) NULL,
    EnrollmentYear INT NOT NULL
);

CREATE TABLE Subjects (
    SubjectId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(150) NOT NULL,
    TotalHours INT NOT NULL
);

CREATE TABLE Schedule (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    GroupId INT NOT NULL FOREIGN KEY REFERENCES Groups(GroupId),
    SubjectId INT NOT NULL FOREIGN KEY REFERENCES Subjects(SubjectId),
    TeacherId INT NOT NULL FOREIGN KEY REFERENCES Teachers(TeacherId),
    ClassDate DATE NOT NULL,
    LessonNumber INT NOT NULL,
    Room NVARCHAR(20) NOT NULL,
    ClassType NVARCHAR(20) NOT NULL
);

CREATE TABLE Attendance (
    RecordId INT IDENTITY(1,1) PRIMARY KEY,
    StudentId INT NOT NULL FOREIGN KEY REFERENCES Students(StudentId),
    ScheduleId INT NOT NULL FOREIGN KEY REFERENCES Schedule(ScheduleId),
    Status NVARCHAR(20) NOT NULL,
    Grade INT NULL,
    ControlType NVARCHAR(20) NOT NULL,
    EntryDate DATE NOT NULL
);

INSERT INTO Teachers (LastName, FirstName, MiddleName, Phone, Email, Position) VALUES
('Иванов', 'Петр', 'Сергеевич', '+79151234567', 'i.ps@technikum.ru', 'преподаватель'),
('Смирнова', 'Ольга', 'Владимировна', '+79152345678', 's.ov@technikum.ru', 'старший преподаватель');

INSERT INTO Groups (Name, Specialty, SpecialtyCode, Course, CuratorId) VALUES
('ИТ-21', 'Информационные системы', '09.02.07', 2, 1),
('ИТ-31', 'Информационные системы', '09.02.07', 3, 2);

INSERT INTO Students (LastName, FirstName, MiddleName, RecordBookNumber, GroupId, Phone, EnrollmentYear) VALUES
('Антонов', 'Алексей', 'Борисович', 'ИТ21001', 1, '+79151112233', 2024),
('Захарова', 'Ольга', 'Игоревна', 'ИТ31001', 2, '+79152223344', 2023);

INSERT INTO Subjects (Name, TotalHours) VALUES
(N'Основы программирования', 120),
(N'Базы данных', 96);

INSERT INTO Schedule (GroupId, SubjectId, TeacherId, ClassDate, LessonNumber, Room, ClassType) VALUES
(1, 1, 1, '2025-09-01', 1, '101', 'лекция'),
(2, 2, 2, '2025-09-02', 1, '201', 'практика');

INSERT INTO Attendance (StudentId, ScheduleId, Status, Grade, ControlType, EntryDate) VALUES
(1, 1, 'присутствовал', 5, 'текущий', '2025-09-01'),
(2, 2, 'присутствовал', 4, 'практика', '2025-09-02');