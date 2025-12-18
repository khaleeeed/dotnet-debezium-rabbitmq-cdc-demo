using CdcDashboard.Consumers;
using CdcDashboard.Hubs;
using CdcDashboard.Services;
using MassTransit;
using System.Net.Mime;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddSingleton<EventStore>();

// Configure MassTransit with RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ApplicantConsumer>();
    x.AddConsumer<BookingConsumer>();
    x.AddConsumer<PackageConsumer>();
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(builder.Configuration.GetValue<string>("RabbitMQ:Host") ?? "localhost", "/", h =>
        {
            h.Username(builder.Configuration.GetValue<string>("RabbitMQ:Username") ?? "guest");
            h.Password(builder.Configuration.GetValue<string>("RabbitMQ:Password") ?? "guest");
        });


        // Applicant
        cfg.ReceiveEndpoint("cdc-applicants-dashboard", e =>
        {
            e.PrefetchCount = 1;
            e.ConfigureConsumeTopology = false;
            e.DefaultContentType = new ContentType("application/json");
            e.UseRawJsonDeserializer();
            e.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
            e.ConfigureConsumer<ApplicantConsumer>(ctx);
            e.Bind("sqlserver-cdc.BookingSystem.dbo.applicant", s => { s.ExchangeType = "fanout"; });
        });

        // Booking
        cfg.ReceiveEndpoint("cdc-bookings-dashboard", e =>
        {
            e.PrefetchCount = 1;
            e.ConfigureConsumeTopology = false;
            e.DefaultContentType = new ContentType("application/json");
            e.UseRawJsonDeserializer();
            e.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
            e.ConfigureConsumer<BookingConsumer>(ctx);
            e.Bind("sqlserver-cdc.BookingSystem.dbo.booking", s => { s.ExchangeType = "fanout"; });
        });

        // Package
        cfg.ReceiveEndpoint("cdc-packages-dashboard", e =>
        {
            e.PrefetchCount = 1;
            e.ConfigureConsumeTopology = false;
            e.DefaultContentType = new ContentType("application/json");
            e.UseRawJsonDeserializer();
            e.UseMessageRetry(retryConfig => retryConfig.Interval(2, 100));
            e.ConfigureConsumer<PackageConsumer>(ctx);
            e.Bind("sqlserver-cdc.BookingSystem.dbo.package", s => { s.ExchangeType = "fanout"; });
        });

        // Ensure case-insensitive JSON mapping for payloads
        cfg.ConfigureJsonSerializerOptions(opts =>
        {
            opts.PropertyNameCaseInsensitive = true;
            return opts;
        });
    });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapHub<CdcHub>("/cdcHub");

app.Run();
