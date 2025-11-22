using MassTransit;
using Microsoft.Extensions.Logging;

namespace DataStreaming.Consumer;

public class BookingConsumer : IConsumer<BookingMessage>
{
    private readonly ILogger<BookingConsumer> _logger;

    public BookingConsumer(ILogger<BookingConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<BookingMessage> context)
    {
        var msg = context.Message;
        var op = msg.Payload!.Operation;

        var before = msg.Payload.Before;
        var after = msg.Payload.After;
        var current = after ?? before;

        var bookingId = current?.Id;
        var bookingStatus = current?.BookingStatus;
        var payloadForLog = after ?? before;

        if (op == ChangeOperation.Update)
        {
            var changes = msg.Payload.GetChangedFields();

            _logger.LogInformation("[Booking][Update] Id={BookingId} Status={BookingStatus} Changes={@Changes}", bookingId, bookingStatus, changes);

            return Task.CompletedTask;
        }

        _logger.LogInformation("[Booking] [{Op}] Id={BookingId} Status={BookingStatus} Payload={@Payload}", op, bookingId, bookingStatus, payloadForLog);

        return Task.CompletedTask;
    }
}
