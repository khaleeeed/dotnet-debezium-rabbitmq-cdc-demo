**Project Idea**: This repository demonstrates a minimal end-to-end Change Data Capture (CDC) demo using SQL Server + Debezium Server to publish change events into RabbitMQ, and a .NET 8 MassTransit consumer application that listens to those events. It contains:
- Debezium Server configuration (`debezium/conf/application.properties`) configured to read SQL Server CDC and publish to RabbitMQ.
- A `docker-compose.yml` that brings up SQL Server, RabbitMQ and Debezium Server.
- SQL initialization script to create a sample `BookingSystem` database with `booking`, `package`, and `applicant` tables and enable CDC (`sqlserver/init/01-init-database.sql`).
- A small .NET 8 consumer app using MassTransit (`consumers/`) which binds to the Debezium exchange and logs CDC events.

**Architecture (high level)**
- SQL Server (CDC enabled) -> Debezium Server (SQL Server connector) -> RabbitMQ exchange (`cdc.events`) -> MassTransit consumers (three receive endpoints for `applicant`, `booking`, `package`).

**Important notes about this repo**
- Debezium is configured to publish JSON messages to RabbitMQ using the exchange configured in `debezium/conf/application.properties` (property `debezium.sink.rabbitmq.exchange`).
- The MassTransit consumers expect messages on receive endpoints with the same routing keys Debezium uses: `sqlserver-cdc.BookingSystem.dbo.applicant`, `sqlserver-cdc.BookingSystem.dbo.booking`, and `sqlserver-cdc.BookingSystem.dbo.package`.
- The included `rabbitmq/definitions.json` contains example exchanges/queues; double-check the exchange name here vs the Debezium config (`cdc.events`) — they must match.

**Prerequisites**
- Docker & Docker Compose
- .NET 8 SDK (for running the `consumers` app locally)
- PowerShell (Windows) — sample commands below are shown for PowerShell

**Quickstart — run locally**
1. Start the platform services (SQL Server, RabbitMQ and Debezium):

```powershell
docker-compose up -d
```

2. Wait until services are healthy. You can check RabbitMQ management at `http://localhost:15672` (default `guest`/`guest`). SQL Server listens on `1433`.

3. Run the .NET MassTransit consumer (from repository root):

```powershell
# Run from repo root (PowerShell)
cd .\consumers
# set env vars so the consumer connects to the RabbitMQ container on the Docker network
$env:RABBITMQ_HOST = 'rabbitmq'
$env:RABBITMQ_USER = 'guest'
$env:RABBITMQ_PASS = 'guest'
dotnet run --project .\Consumers.csproj
```

4. Generate test data / CDC events by connecting to the SQL Server instance (e.g. with `sqlcmd`, Azure Data Studio, or SSMS) and running inserts/updates against the `BookingSystem` database.

Example SQL (run against `localhost,1433` with `sa` / `YourPassword123!`):

```sql
USE BookingSystem;
GO

-- Insert an applicant
INSERT INTO dbo.applicant (firstName, lastName, email, phone)
VALUES ('Alice', 'Example', 'alice@example.com', '+10000000000');

-- Update a package
UPDATE dbo.package SET price = price + 10 WHERE id = 1;

-- Insert a booking
INSERT INTO dbo.booking (packageId, applicantId, bookingStatus, Amount)
VALUES (1, 1, 1, 999.99);
```

When these statements run Debezium should capture the change and publish events to RabbitMQ. The MassTransit consumer will log the incoming messages.

**How the .NET consumer is wired**
- The consumer app `consumers/Program.cs` uses MassTransit with RabbitMQ. It creates three `ReceiveEndpoint` entries whose names equal the Debezium topics/routing keys (for example `sqlserver-cdc.BookingSystem.dbo.applicant`) and binds them to the exchange `cdc.events` used by Debezium:
    - `sqlserver-cdc.BookingSystem.dbo.applicant` -> handled by `ApplicantConsumer`
    - `sqlserver-cdc.BookingSystem.dbo.booking` -> handled by `BookingConsumer`
    - `sqlserver-cdc.BookingSystem.dbo.package` -> handled by `PackageConsumer`


**Debezium configuration highlights**
- `debezium/conf/application.properties` points Debezium at `sqlserver` on the Docker network and instructs Debezium to publish to RabbitMQ at `rabbitmq:5672`.
- Key settings include:
    - `debezium.source.table.include.list=dbo.booking,dbo.package,dbo.applicant`
    - `debezium.sink.rabbitmq.exchange=cdc.events`
    - `debezium.sink.rabbitmq.routingKey.source=topic` (routing key derived from topic name)

ASCII diagram:

```
[SQL Server] --(CDC)--> [Debezium Server]
       
                      Debezium publishes JSON to RabbitMQ exchange `cdc.events`
                                      |
                                      v
                                [RabbitMQ]
                                      |
     ------------------------------------------------------------
     |              |                          |                 |
     v              v                          v                 v
 [queue: applicant] [queue: booking] [queue: package]  (other queues)
     |              |                          |
     v              v                          v
 [ApplicantConsumer] [BookingConsumer] [PackageConsumer]
```

Quickstart (minimal):

```powershell
# 1) Start services
docker-compose up -d

# 2) Run consumer (from repo root)
cd .\consumers
$env:RABBITMQ_HOST = 'rabbitmq'
$env:RABBITMQ_USER = 'guest'
$env:RABBITMQ_PASS = 'guest'
dotnet run --project .\Consumers.csproj
```

Test by applying SQL changes to the `BookingSystem` DB (see `sqlserver/init/01-init-database.sql`). Debezium will pick up CDC events and publish to RabbitMQ; the consumer logs incoming messages.

Key files:
- `docker-compose.yml` — starts SQL Server, RabbitMQ, Debezium
- `debezium/conf/application.properties` — Debezium -> RabbitMQ config
- `sqlserver/init/01-init-database.sql` — create DB, enable CDC, sample data
- `consumers/` — .NET MassTransit consumer app


