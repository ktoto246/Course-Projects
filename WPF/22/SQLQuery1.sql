CREATE DATABASE Алфавит;
GO

USE Алфавит;
GO

CREATE TABLE Clients (
    client_id INT IDENTITY(1,1) PRIMARY KEY,
    company_name VARCHAR(150),
    contact_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20) NOT NULL
);

CREATE TABLE Employees (
    employee_id INT IDENTITY(1,1) PRIMARY KEY,
    full_name VARCHAR(100) NOT NULL,
    position VARCHAR(50) NOT NULL
);

CREATE TABLE Services (
    service_id INT IDENTITY(1,1) PRIMARY KEY,
    service_name VARCHAR(150) NOT NULL,
    base_price DECIMAL(10, 2) NOT NULL
);

CREATE TABLE Orders (
    order_id INT IDENTITY(1,1) PRIMARY KEY,
    client_id INT REFERENCES Clients(client_id),
    employee_id INT REFERENCES Employees(employee_id),
    order_date DATE NOT NULL,
    status VARCHAR(50) NOT NULL
);

CREATE TABLE Order_Details (
    detail_id INT IDENTITY(1,1) PRIMARY KEY,
    order_id INT REFERENCES Orders(order_id) ON DELETE CASCADE,
    service_id INT REFERENCES Services(service_id),
    quantity INT NOT NULL CHECK (quantity > 0)
);

INSERT INTO Clients (company_name, contact_name, phone) VALUES
('ИП Петров (Магазин У Дяди Вани)', 'Петр Иванов', '+79991112233'),
('Кафе Хопер', 'Елена Сидорова', '+79992223344'),
(NULL, 'Алексей Кузнецов', '+79993334455');

INSERT INTO Employees (full_name, position) VALUES
('Иванов Сергей Владимирович', 'Менеджер по продажам'),
('Смирнова Ольга Игоревна', 'Дизайнер'),
('Козлов Дмитрий Николаевич', 'Монтажник рекламных конструкций');

INSERT INTO Services (service_name, base_price) VALUES
('Печать баннера (за кв.м.)', 450.00),
('Разработка дизайн-макета визитки', 1500.00),
('Печать визиток (100 шт.)', 600.00),
('Монтаж вывески (час работы)', 1200.00);

INSERT INTO Orders (client_id, employee_id, order_date, status) VALUES
(1, 1, '2026-06-10', 'Завершен'),
(2, 1, '2026-06-15', 'В работе'),
(3, 1, '2026-06-16', 'Новый');

INSERT INTO Order_Details (order_id, service_id, quantity) VALUES
(1, 2, 1),
(1, 3, 5),
(2, 1, 10),
(2, 4, 3),
(3, 3, 2);