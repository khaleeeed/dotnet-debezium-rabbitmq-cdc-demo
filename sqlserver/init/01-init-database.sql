-- Create database
CREATE DATABASE BookingSystem;
GO

USE BookingSystem;
GO

-- Enable CDC at database level
EXEC sys.sp_cdc_enable_db;
GO

CREATE TABLE package (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(255) NOT NULL,
    price money,
    capacity INT, 
    capacityUsed INT DEFAULT 0, 
    status INT,
    createdAt DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE applicant (
    id INT PRIMARY KEY IDENTITY(1,1),
    firstName NVARCHAR(100) NOT NULL,
    lastName NVARCHAR(100) NOT NULL,
    email NVARCHAR(255) NOT NULL,
    phone NVARCHAR(50),
    BirthDate DATETIME2,
    CreatedAt DATETIME2 DEFAULT GETDATE()
);
GO

CREATE TABLE booking (
    id INT PRIMARY KEY IDENTITY(1,1),
    packageId INT, 
    applicantId INT,
    bookingStatus int, 
    bookingDate DATETIME2,
    Amount money,
    createdAt DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (packageId) REFERENCES package(id),
    FOREIGN KEY (applicantId) REFERENCES applicant(id)
);
GO

ALTER TABLE Applicant
ALTER COLUMN BirthDate datetime2(7);


-- Enable CDC for required tables
EXEC sys.sp_cdc_enable_table  
@source_schema = N'dbo',  
@source_name   = N'booking',  
@role_name     = NULL,  
@supports_net_changes = 1;
GO

EXEC sys.sp_cdc_enable_table  
@source_schema = N'dbo',  
@source_name   = N'package',  
@role_name     = NULL,  
@supports_net_changes = 1;
GO

EXEC sys.sp_cdc_enable_table  
@source_schema = N'dbo',  
@source_name   = N'applicant',  
@role_name     = NULL,  
@supports_net_changes = 1;
GO

-- Insert sample data
INSERT INTO package (name, price, capacity, capacityUsed, status) 
VALUES 
('Premium Package', 999.99, 50, 10, 3),
('Basic Package', 499.99, 100, 5, 5);

INSERT INTO applicant (firstName, Lastname, email, phone) 
VALUES 
('John', 'Doe', 'john.doe@example.com', '+1234567890'),
('Jane', 'Smith', 'jane.smith@example.com', '+0987654321');

INSERT INTO booking (packageId, applicantId, bookingStatus, Amount) 
VALUES 
(1, 1, 2, 999.99),
(2, 2, 6, 499.99);