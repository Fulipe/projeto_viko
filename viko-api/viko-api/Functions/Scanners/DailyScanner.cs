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

            // calcula delay até à hora exata
            var eventTimeUtc = scheduleDate.Value.ToUniversalTime();
            var nowUtc = DateTime.UtcNow;

            var delay = eventTimeUtc - nowUtc;

            // A hora já passou => faz o update imediato do status
            if (delay <= TimeSpan.Zero)
            {

                var eventGuid = ev.guid.ToString();

                var fetched = await _eventsService.GetEvent(eventGuid);

                if (!fetched.Item1.status)
                {
                    _logger.LogWarning($"Event {eventGuid} not found for immediate update.");
                    continue;
                }

                var eventData = fetched.Item2;

                int newStatus = eventData.EventStatus switch
                {
                    1 => 2, // Open > Closed
                    2 => 3, // Closed > Finished
                    _ => eventData.EventStatus
                };

                if (newStatus != eventData.EventStatus)
                {
                    await _eventsService.UpdateEventStatus(eventGuid, newStatus);

                    _logger.LogInformation(
                        $"Event {eventGuid} updated IMMEDIATELY from {eventData.EventStatus} to {newStatus}"
                    );
                }
                else
                {
                    _logger.LogInformation(
                        $"Event {eventGuid} ignored (no further status transition)."
                    );
                }

                continue; // já tratou, não precisa meter na queue
            }

            Console.WriteLine(ev.Title);

            var payload = JsonSerializer.Serialize(new QueuePayload
            {
                guid = ev.guid,
                CurrentStatus = ev.EventStatus
            });
            
            // Encodes payload, so QueueStorage can storage it 
            var bytes = Encoding.UTF8.GetBytes(payload);
            await _queueClient.SendMessageAsync(
                Convert.ToBase64String(bytes),
                visibilityTimeout: delay
            );

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
