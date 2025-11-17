using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using viko_api.Services;

public class DailyScanner
{
    private readonly ILogger<DailyScanner> _logger;
    private readonly QueueClient _queueClient;
    private readonly IEventsService _eventsService;

    public DailyScanner(ILogger<DailyScanner> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;

        // Queue storage init
        var queueConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                            ?? throw new InvalidOperationException("QueueStorage not set.");

        _queueClient = new QueueClient(
            queueConnectionString,
            "event-status-updates"
        );

        _queueClient.CreateIfNotExists();
    }

    [Function("DailyScanner")]
    //Crontab is set to run everyday at midnight.
    public async Task Run([TimerTrigger("0 0 0 * * *")] TimerInfo timer)
    {
        _logger.LogInformation("DailyScanner started.");

        var events = await _eventsService.GetAllPublicEvents();

        // Only considers events which RegistrationDeadline or EndDate is today
        var today = DateTime.UtcNow.Date;

        foreach (var ev in events.Item2)
        {
            DateTime? scheduleDate = null;

            if (ev.EventStatus == 1)
                scheduleDate = ev.RegistrationDeadline.Date;

            else if (ev.EventStatus == 2)
                scheduleDate = ev.EndDate.Date;

            if (scheduleDate != today)
                continue; // Ignores events that are not from actual date

            Console.WriteLine(ev.Title);

            var payload = JsonSerializer.Serialize(new QueuePayload
            {
                guid = ev.guid,
                CurrentStatus = ev.EventStatus
            });
            
            // Encodes payload, so QueueStorage can storage it 
            var bytes = Encoding.UTF8.GetBytes(payload);
            await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));

            _logger.LogInformation($"Event {ev.guid} added to queue for status update.");
        }

        _logger.LogInformation("DailyScanner finished.");
    }
}

// QueuePayload model
public class QueuePayload
{
    public Guid guid { get; set; }
    public int CurrentStatus { get; set; } = default!;
}
