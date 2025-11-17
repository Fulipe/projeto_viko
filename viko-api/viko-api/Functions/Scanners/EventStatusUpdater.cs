using System;
using System.Text.Json;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using viko_api.Services;

namespace viko_api.Functions.Scanners;

public class EventStatusUpdater
{
    private readonly ILogger<EventStatusUpdater> _logger;
    private readonly IEventsService _eventsService;

    public EventStatusUpdater(ILogger<EventStatusUpdater> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;

    }

    [Function("EventStatusUpdater")]
    public async Task Run([QueueTrigger("event-status-updates", Connection = "AzureWebJobsStorage")] string message)
    {

        _logger.LogInformation("----Queue started.----");


        var payload = JsonSerializer.Deserialize<QueuePayload>(message);

        if (payload == null)
        {
            _logger.LogWarning("Invalid payload.");
            return;
        }

        string eventGuid = payload.guid.ToString();

        var ev = await _eventsService.GetEvent(eventGuid);

        var response = ev.Item1;
        var eventFetched = ev.Item2;

        if (response.status == false)
        {
            _logger.LogWarning(response.msg);
            return;
        }

        // Changes eventStatus depending on inbound eventStatus
        int newStatus = eventFetched.EventStatus switch
        {
            1 => 2, //"Open" => "Closed",
            2 => 3, //"Closed" => "Finished",
            _ => eventFetched.EventStatus // no change
        };

        if (newStatus == eventFetched.EventStatus)
        {
            _logger.LogInformation($"Event {eventGuid} ignored (status: {eventFetched.EventStatus}).");
            return;
        }

        // Updates event of eventGuid for its new Status
        await _eventsService.UpdateEventStatus(eventGuid, newStatus);

        _logger.LogInformation(
            $"Event {eventGuid} status changed FROM {eventFetched.EventStatus} TO {newStatus}."
        );
    }
}
