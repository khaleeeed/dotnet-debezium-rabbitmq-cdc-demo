Start the services:
    docker-compose up -d

Verify CDC is working:
    -- Connect to SQL Server and check CDC status
    USE BookingSystem;
    GO

    -- Check if CDC is enabled on database
    SELECT name, is_cdc_enabled FROM sys.databases WHERE name = 'BookingSystem';

    -- Check CDC enabled tables
    SELECT name, is_tracked_by_cdc FROM sys.tables WHERE is_tracked_by_cdc = 1;

    -- Test CDC by inserting data
    INSERT INTO applicant (first_name, last_name, email) 
    VALUES ('Test', 'User', 'test@example.com');

    INSERT INTO package (name, capacity, status) 
    VALUES ('Test Package', 10, 3);

Check RabbitMQ queues:
    Access RabbitMQ Management: http://localhost:15672 (guest/guest)
    Verify queues are created and receiving messages

