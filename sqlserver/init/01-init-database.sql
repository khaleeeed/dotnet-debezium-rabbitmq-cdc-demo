-- Create database
CREATE DATABASE BookingSystem;
GO

USE BookingSystem;
GO

-- Enable CDC at database level
EXEC sys.sp_cdc_enable_db;
GO

-- Create package table with new fields
CREATE TABLE package (
    id INT PRIMARY KEY IDENTITY(1,1),
    name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    price DECIMAL(10,2),
    capacity INT,  -- New field
    capacity_used INT DEFAULT 0,  -- New field
    status INT,  -- New field
    availability_status NVARCHAR(50),  -- New field
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE()
);
GO

-- Create applicants table
CREATE TABLE applicant (
    id INT PRIMARY KEY IDENTITY(1,1),
    first_name NVARCHAR(100) NOT NULL,
    last_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(255) NOT NULL,
    phone NVARCHAR(50),
    date_of_birth DATE,
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE()
);
GO

-- Create booking table with additional fields
CREATE TABLE booking (
    id INT PRIMARY KEY IDENTITY(1,1),
    packageId INT,  -- New field
    applicantId INT,  -- New field
    bookingStatus NVARCHAR(50),  -- New field
    booking_date DATETIME2 DEFAULT GETDATE(),
    total_amount DECIMAL(10,2),
    notes NVARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    updated_at DATETIME2 DEFAULT GETDATE(),
    FOREIGN KEY (packageId) REFERENCES package(id),
    FOREIGN KEY (applicantId) REFERENCES applicant(id)
);
GO

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
INSERT INTO package (name, description, price, capacity, capacity_used, status, availability_status) 
VALUES 
('Premium Package', 'Premium travel package', 999.99, 50, 10, 3, 'AVAILABLE'),
('Basic Package', 'Basic travel package', 499.99, 100, 5, 5, 'UNAVAILABLE');

INSERT INTO applicant (first_name, last_name, email, phone) 
VALUES 
('John', 'Doe', 'john.doe@example.com', '+1234567890'),
('Jane', 'Smith', 'jane.smith@example.com', '+0987654321');

INSERT INTO booking (packageId, applicantId, bookingStatus, total_amount) 
VALUES 
(1, 1, 'CONFIRMED', 999.99),
(2, 2, 'PENDING', 499.99);