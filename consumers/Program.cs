using DataStreaming.Consumer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(cfg => cfg.AddConsole());

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ApplicantConsumer>();
            x.AddConsumer<BookingConsumer>();
            x.AddConsumer<PackageConsumer>();

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
                var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
                var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "guest";

                cfg.Host(rabbitHost, "/", h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                });

                // Applicant
                cfg.ReceiveEndpoint("sqlserver-cdc.BookingSystem.dbo.applicant", e =>
                {
                    e.PrefetchCount = 1;
                    e.ConfigureConsumeTopology = false;
                    e.DefaultContentType = new ContentType("application/json");
                    e.UseRawJsonDeserializer();
                    e.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
                    e.ConfigureConsumer<ApplicantConsumer>(ctx);
                    e.Bind("cdc.events", s => { s.RoutingKey = "sqlserver-cdc.BookingSystem.dbo.applicant"; s.ExchangeType = "topic"; });
                });

                // Booking
                cfg.ReceiveEndpoint("sqlserver-cdc.BookingSystem.dbo.booking", e =>
                {
                    e.PrefetchCount = 1;
                    e.ConfigureConsumeTopology = false;
                    e.DefaultContentType = new ContentType("application/json");
                    e.UseRawJsonDeserializer();
                    e.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
                    e.ConfigureConsumer<BookingConsumer>(ctx);
                    e.Bind("cdc.events", s => { s.RoutingKey = "sqlserver-cdc.BookingSystem.dbo.booking"; s.ExchangeType = "topic"; });
                });

                // Package
                cfg.ReceiveEndpoint("sqlserver-cdc.BookingSystem.dbo.package", e =>
                {
                    e.PrefetchCount = 1;
                    e.ConfigureConsumeTopology = false;
                    e.DefaultContentType = new ContentType("application/json");
                    e.UseRawJsonDeserializer();
                    e.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
                    e.ConfigureConsumer<PackageConsumer>(ctx);
                    e.Bind("cdc.events", s => { s.RoutingKey = "sqlserver-cdc.BookingSystem.dbo.package"; s.ExchangeType = "topic"; });
                });

                // Ensure case-insensitive JSON mapping for payloads
                cfg.ConfigureJsonSerializerOptions(opts =>
                {
                    opts.PropertyNameCaseInsensitive = true;
                    return opts;
                });
            });
        });
    })
    .Build();

await host.RunAsync();
