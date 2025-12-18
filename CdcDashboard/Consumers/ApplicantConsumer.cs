using MassTransit;
using CdcDashboard.Models;
using CdcDashboard.Services;
using CdcDashboard.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CdcDashboard.Consumers;

public class ApplicantConsumer : IConsumer<ApplicantMessage>
{
    private readonly ILogger<ApplicantConsumer> _logger;
    private readonly EventStore _eventStore;
    private readonly IHubContext<CdcHub> _hubContext;

    public ApplicantConsumer(
        ILogger<ApplicantConsumer> logger, 
        EventStore eventStore,
        IHubContext<CdcHub> hubContext)
    {
        _logger = logger;
        _eventStore = eventStore;
        _hubContext = hubContext;
    }

    public async Task Consume(ConsumeContext<ApplicantMessage> context)
    {
        var msg = context.Message;
        var op = msg.Payload!.Operation;

        var before = msg.Payload.Before;
        var after = msg.Payload.After;
        var current = after ?? before;

        var applicantId = current?.Id ?? 0;
        var applicantName = current?.FullName ?? string.Empty;

        var evt = new ApplicantEvent
        {
            Id = applicantId,
            Type = op.ToString(),
            Before = before,
            After = after,
            Name = applicantName,
            Timestamp = DateTime.UtcNow
        };

        if (op == ChangeOperation.Update)
        {
            evt.Changes = msg.Payload.GetChangedFields();
            _logger.LogInformation("[Applicant][Update] Id={ApplicantId} Name={ApplicantName} Changes={@Changes}", 
                applicantId, applicantName, evt.Changes);
        }
        else
        {
            _logger.LogInformation("[Applicant] [{Op}] Id={ApplicantId} Name={ApplicantName} Payload={@Payload}", 
                op, applicantId, applicantName, after ?? before);
        }

        _eventStore.AddApplicantEvent(evt);
        await _hubContext.Clients.All.SendAsync("ApplicantEvent", evt);
    }
}
