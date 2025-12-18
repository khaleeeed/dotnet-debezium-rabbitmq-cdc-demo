using MassTransit;
using CdcDashboard.Models;
using CdcDashboard.Services;
using CdcDashboard.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CdcDashboard.Consumers;

public class PackageConsumer : IConsumer<PackageMessage>
{
    private readonly ILogger<PackageConsumer> _logger;
    private readonly EventStore _eventStore;
    private readonly IHubContext<CdcHub> _hubContext;

    public PackageConsumer(
        ILogger<PackageConsumer> logger, 
        EventStore eventStore,
        IHubContext<CdcHub> hubContext)
    {
        _logger = logger;
        _eventStore = eventStore;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<PackageMessage> context)
    {
        var msg = context.Message;
        var op = msg.Payload!.Operation;

        var before = msg.Payload.Before;
        var after = msg.Payload.After;
        var current = after ?? before;

        var packageId = current?.Id ?? 0;
        var packageName = current?.Name ?? "Unknown";

        var evt = new PackageEvent
        {
            Id = packageId,
            Type = op.ToString(),
            Before = before,
            After = after,
            Name = packageName,
            Timestamp = DateTime.UtcNow
        };

        if (op == ChangeOperation.Update)
        {
            evt.Changes = msg.Payload.GetChangedFields();
            _logger.LogInformation("[Package][Update] Id={PackageId} Name={PackageName} Changes={@Changes}", 
                packageId, packageName, evt.Changes);
        }
        else
        {
            _logger.LogInformation("[Package] [{Op}] Id={PackageId} Name={PackageName} Payload={@Payload}", 
                op, packageId, packageName, after ?? before);
        }

        _eventStore.AddPackageEvent(evt);
        await _hubContext.Clients.All.SendAsync("PackageEvent", evt);
    }
}
