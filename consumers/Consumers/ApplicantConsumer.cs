using MassTransit;
using Microsoft.Extensions.Logging;

namespace DataStreaming.Consumer;

public class ApplicantConsumer : IConsumer<ApplicantMessage>
{
    private readonly ILogger<ApplicantConsumer> _logger;

    public ApplicantConsumer(ILogger<ApplicantConsumer> logger) => _logger = logger;

    public Task Consume(ConsumeContext<ApplicantMessage> context)
    {
        var msg = context.Message;
        var op = msg.Payload!.Operation;

        var before = msg.Payload.Before;
        var after = msg.Payload.After;
        var current = after ?? before;

        var applicantId = current?.Id;
        var applicantName = string.Join(' ', new[] { current?.FirstName, current?.LastName }.Where(s => !string.IsNullOrWhiteSpace(s)));
        var payloadForLog = after ?? before;

        if (op == ChangeOperation.Update)
        {
            var changes = msg.Payload.GetChangedFields();
            _logger.LogInformation("[Applicant][Update] Id={ApplicantId} Name={ApplicantName} Changes={@Changes}", applicantId, applicantName, changes);

            return Task.CompletedTask;
        }
        _logger.LogInformation("[Applicant] [{op}] Id={ApplicantId} Name={ApplicantName} Payload={@Payload}", op, applicantId, applicantName, payloadForLog);




        return Task.CompletedTask;
    }
}
