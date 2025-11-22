using MassTransit;
using Microsoft.Extensions.Logging;

namespace DataStreaming.Consumer;

public class PackageConsumer : IConsumer<PackageMessage>
{
    private readonly ILogger<PackageConsumer> _logger;

    public PackageConsumer(ILogger<PackageConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<PackageMessage> context)
    {
        var msg = context.Message;
        var op = msg.Payload!.Operation;

        var before = msg.Payload.Before;
        var after = msg.Payload.After;
        var current = after ?? before;

        var packageId = current?.Id;
        var packageName = current?.Name;
        var payloadForLog = after ?? before;

        if (op == ChangeOperation.Update)
        {

            var changes = msg.Payload.GetChangedFields();

            _logger.LogInformation("[Package][Update] Id={PackageId} Name={PackageName} Changes={@Changes}", packageId, packageName, changes);

            return Task.CompletedTask;
        }

        _logger.LogInformation("[Package] [{Op}] Id={PackageId} Name={PackageName} Payload={@Payload}", op, packageId, packageName, payloadForLog);

        return Task.CompletedTask;
    }
}
