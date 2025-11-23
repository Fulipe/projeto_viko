using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions.Teacher;

public class EditEvent
{
    private readonly ILogger<EditEvent> _logger;
    private readonly IEventsService _eventsService;


    public EditEvent(ILogger<EditEvent> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("EditEvent")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var request = await req.ReadFromJsonAsync<UpdateEventRequest>();

        if (request == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        var updateEvent = await _eventsService.EditEvent(request.guid, request.eventUpdate);

        if (updateEvent.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await badResponse.WriteStringAsync(updateEvent.msg);
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { status = updateEvent.status, msg = updateEvent.msg });
        return res;

    }
}

//Class to receive request
public class UpdateEventRequest
{
    public string guid { get; set; }
    public EventEditDto eventUpdate { get; set; }
}