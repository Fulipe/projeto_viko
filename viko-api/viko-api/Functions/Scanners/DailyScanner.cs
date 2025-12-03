using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using viko_api.Models;
using viko_api.Services;

public class DailyScanner
{
    private readonly ILogger<DailyScanner> _logger;
    private readonly QueueClient _queueClient;
    private readonly IEventsService _eventsService;
    private readonly VikoDbContext _dbContext;

    public DailyScanner(ILogger<DailyScanner> logger, IEventsService eventsService, VikoDbContext dbContext)
    {
        _logger = logger;
        _eventsService = eventsService;
        _dbContext = dbContext;

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
            DateTime? scheduleDateExact = null;
            DateTime? scheduleDate = null;

            if (ev.EventStatus == 1)
            {
                scheduleDateExact = ev.RegistrationDeadline;
                scheduleDate = ev.RegistrationDeadline.Date;
            }

            else if (ev.EventStatus == 2)
            {
                scheduleDateExact = ev.EndDate;
                scheduleDate = ev.EndDate.Date;
            }

            else if (ev.EventStatus == 3) 
            {
                continue;
            }

            if (scheduleDate != today)
                continue; // Ignores events that are not from actual date


            // Calculates delay until the exact hour
            var eventTimeUtc = scheduleDateExact.Value.ToUniversalTime();
            var nowUtc = DateTime.UtcNow;

            var delay = eventTimeUtc - nowUtc;

            // If schedule has passed do the update of the status
            if (delay <= TimeSpan.Zero)
            {
                // Only schedules if theres not an already pending queue
                if (!ev.HasPendingStatusChange)
                {
                    var eventNoQueue =  await _dbContext.Events.Where(e => e.EventGuid == ev.guid).FirstOrDefaultAsync();

                    if (eventNoQueue != null)
                    {
                        eventNoQueue.EventStatusId = eventNoQueue.EventStatusId switch {
                            1 => 2,
                            2 => 3,
                            _ => eventNoQueue.EventStatusId
                        };
                    }

                    eventNoQueue.HasPendingStatusChange = false;

                    await _dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Executed immediate status update for {ev.guid}.");
                }

                continue; 
            }

            // Only schedules if theres not an already pending queue
            if (!ev.HasPendingStatusChange)
            { 

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

                var eventToQueue = await _dbContext.Events.Where(e => e.EventGuid == ev.guid).FirstOrDefaultAsync();
                if (eventToQueue == null)
                    return;

                eventToQueue.HasPendingStatusChange = true;
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Event {ev.guid} added to queue for status update.");
            }
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
