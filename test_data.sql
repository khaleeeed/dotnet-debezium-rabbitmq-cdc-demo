USE BookingSystem;
GO

-- =============================================
-- TEST DATA GENERATION SCRIPT
-- =============================================

PRINT 'Starting Test Data Generation...';

-- 1. Applicants (6 Created, 3 Updated, 3 Deleted = 12 Events)
PRINT 'Generating Applicants...';

-- Create 6 Applicants
INSERT INTO applicant (firstName, lastName, email, phone, BirthDate) VALUES ('Alice', 'Wonder', 'alice@test.com', '555-0101', '1990-01-01');
INSERT INTO applicant (firstName, lastName, email, phone, BirthDate) VALUES ('Bob', 'Builder', 'bob@test.com', '555-0102', '1985-05-15');
INSERT INTO applicant (firstName, lastName, email, phone, BirthDate) VALUES ('Charlie', 'Chaplin', 'charlie@test.com', '555-0103', '1889-04-16');
INSERT INTO applicant (firstName, lastName, email, phone, BirthDate) VALUES ('David', 'Copperfield', 'david@test.com', '555-0104', '1956-09-16');
INSERT INTO applicant (firstName, lastName, email, phone, BirthDate) VALUES ('Eve', 'Polastri', 'eve@test.com', '555-0105', '1980-11-20');
INSERT INTO applicant (firstName, lastName, email, phone, BirthDate) VALUES ('Frank', 'Castle', 'frank@test.com', '555-0106', '1975-02-15');
WAITFOR DELAY '00:00:01';

-- Update 3 Applicants
UPDATE applicant SET email = 'alice.new@test.com' WHERE email = 'alice@test.com';
UPDATE applicant SET phone = '555-9999' WHERE email = 'bob@test.com';
UPDATE applicant SET firstName = 'Charles' WHERE email = 'charlie@test.com';
WAITFOR DELAY '00:00:01';

-- Delete 3 Applicants
DELETE FROM applicant WHERE email = 'david@test.com';
DELETE FROM applicant WHERE email = 'eve@test.com';
DELETE FROM applicant WHERE email = 'frank@test.com';
WAITFOR DELAY '00:00:01';


-- 2. Packages (3 Created, Updates, 1 Deleted)
PRINT 'Generating Packages...';

-- Create 3 Packages
INSERT INTO package (name, price, capacity, capacityUsed, status) VALUES ('Gold Package', 100.00, 50, 0, 1);
INSERT INTO package (name, price, capacity, capacityUsed, status) VALUES ('Silver Package', 50.00, 100, 0, 1);
INSERT INTO package (name, price, capacity, capacityUsed, status) VALUES ('Bronze Package', 25.00, 200, 0, 1);
WAITFOR DELAY '00:00:01';

-- Update Packages (Capacity and Price)
UPDATE package SET capacity = 60 WHERE name = 'Gold Package';
UPDATE package SET price = 55.00 WHERE name = 'Silver Package';
WAITFOR DELAY '00:00:01';

-- Delete Bronze Package
DELETE FROM package WHERE name = 'Bronze Package';
WAITFOR DELAY '00:00:01';


-- 3. Bookings (11 Bookings with Status Flows)
PRINT 'Generating Bookings...';

-- Helper variables for IDs
DECLARE @GoldPkgId INT = (SELECT TOP 1 id FROM package WHERE name = 'Gold Package');
DECLARE @SilverPkgId INT = (SELECT TOP 1 id FROM package WHERE name = 'Silver Package');
DECLARE @AliceId INT = (SELECT TOP 1 id FROM applicant WHERE email = 'alice.new@test.com');
DECLARE @BobId INT = (SELECT TOP 1 id FROM applicant WHERE email = 'bob@test.com');
DECLARE @CharlieId INT = (SELECT TOP 1 id FROM applicant WHERE firstName = 'Charles');

-- Create 11 Pending Bookings
-- We will track IDs to update them later
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@GoldPkgId, @AliceId, 1, 100.00, GETDATE()); -- B1
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@GoldPkgId, @BobId, 1, 100.00, GETDATE());   -- B2
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@SilverPkgId, @CharlieId, 1, 55.00, GETDATE()); -- B3
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@SilverPkgId, @AliceId, 1, 55.00, GETDATE());   -- B4
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@SilverPkgId, @BobId, 1, 55.00, GETDATE());     -- B5
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@GoldPkgId, @CharlieId, 1, 100.00, GETDATE());  -- B6
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@GoldPkgId, @AliceId, 1, 100.00, GETDATE());    -- B7
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@SilverPkgId, @BobId, 1, 55.00, GETDATE());     -- B8
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@SilverPkgId, @CharlieId, 1, 55.00, GETDATE()); -- B9
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@GoldPkgId, @BobId, 1, 100.00, GETDATE());      -- B10
INSERT INTO booking (packageId, applicantId, bookingStatus, Amount, bookingDate) VALUES (@GoldPkgId, @AliceId, 1, 100.00, GETDATE());    -- B11
WAITFOR DELAY '00:00:02';


-- CONFIRM Bookings (Update Status to 2, Increase Capacity)
-- B1, B2, B3, B4, B5, B6, B7, B10, B11 become Confirmed
UPDATE booking SET bookingStatus = 2 WHERE bookingStatus = 1;

-- Update Package Capacity (Logic: 11 bookings confirmed)
-- Gold Package: 6 bookings (B1, B2, B6, B7, B10, B11)
UPDATE package SET capacityUsed = capacityUsed + 6 WHERE id = @GoldPkgId;
-- Silver Package: 5 bookings (B3, B4, B5, B8, B9) -> Wait, B8, B9 are also confirmed above? Yes "WHERE bookingStatus = 1" captures all.
UPDATE package SET capacityUsed = capacityUsed + 5 WHERE id = @SilverPkgId;
WAITFOR DELAY '00:00:02';


-- CANCEL Some Bookings (Update Status 2 -> 3, Decrease Capacity)
-- Cancel B4, B5, B6 (Silver, Silver, Gold)
-- We need to identify them specifically. Let's use TOP or specific criteria.
-- For script simplicity, we'll pick recent bookings for these users.

-- Cancel B4 (Alice, Silver)
UPDATE TOP (1) booking SET bookingStatus = 3 WHERE applicantId = @AliceId AND packageId = @SilverPkgId AND bookingStatus = 2;
UPDATE package SET capacityUsed = capacityUsed - 1 WHERE id = @SilverPkgId;

-- Cancel B5 (Bob, Silver)
UPDATE TOP (1) booking SET bookingStatus = 3 WHERE applicantId = @BobId AND packageId = @SilverPkgId AND bookingStatus = 2;
UPDATE package SET capacityUsed = capacityUsed - 1 WHERE id = @SilverPkgId;

-- Cancel B6 (Charlie, Gold)
UPDATE TOP (1) booking SET bookingStatus = 3 WHERE applicantId = @CharlieId AND packageId = @GoldPkgId AND bookingStatus = 2;
UPDATE package SET capacityUsed = capacityUsed - 1 WHERE id = @GoldPkgId;

WAITFOR DELAY '00:00:01';

PRINT 'Test Data Generation Complete.';
GO
