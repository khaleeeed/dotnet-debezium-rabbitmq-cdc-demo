using MassTransit;
using CdcDashboard.Models;
using CdcDashboard.Services;
using CdcDashboard.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CdcDashboard.Consumers;

public class BookingConsumer : IConsumer<BookingMessage>
{
    private readonly ILogger<BookingConsumer> _logger;
    private readonly EventStore _eventStore;
    private readonly IHubContext<CdcHub> _hubContext;

    public BookingConsumer(
        ILogger<BookingConsumer> logger, 
        EventStore eventStore,
        IHubContext<CdcHub> hubContext)
    {
        _logger = logger;
        _eventStore = eventStore;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<BookingMessage> context)
    {
        var msg = context.Message;
        var op = msg.Payload!.Operation;

        var before = msg.Payload.Before;
        var after = msg.Payload.After;
        var current = after ?? before;

        var bookingId = current?.Id ?? 0;
        var bookingStatus = current?.StatusName ?? "Unknown";

        var evt = new BookingEvent
        {
            Id = bookingId,
            Type = op.ToString(),
            Before = before,
            After = after,
            Name = $"Booking #{bookingId}",
            Timestamp = DateTime.UtcNow
        };

        if (op == ChangeOperation.Update)
        {
            evt.Changes = msg.Payload.GetChangedFields();
            _logger.LogInformation("[Booking][Update] Id={BookingId} Status={BookingStatus} Changes={@Changes}", 
                bookingId, bookingStatus, evt.Changes);
        }
        else
        {
            _logger.LogInformation("[Booking] [{Op}] Id={BookingId} Status={BookingStatus} Payload={@Payload}", 
                op, bookingId, bookingStatus, after ?? before);
        }

        _eventStore.AddBookingEvent(evt);
        await _hubContext.Clients.All.SendAsync("BookingEvent", evt);
    }
}
