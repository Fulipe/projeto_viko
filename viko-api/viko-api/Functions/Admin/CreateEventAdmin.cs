using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions.Admin;

public class CreateEventAdmin
{
    private readonly ILogger<CreateEventAdmin> _logger;
    private readonly IEventsService _eventsService;


    public CreateEventAdmin(ILogger<CreateEventAdmin> logger, IEventsService eventsService)
    {
        _eventsService = eventsService;
        _logger = logger;
    }

    [Function("CreateEventAdmin")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin");
        if (roleCheck != null)
            return roleCheck;

        //Receive a request with corresponding with EventCreationDto

        var request = await req.ReadFromJsonAsync<AdminEventCreationDto>();

        if (request == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }


        //Send EventCreationDto and ID to CreateEvent() function in IEventService

        var eventCreated = await _eventsService.AdminCreateEvent(request);

        if (eventCreated.status == false)
        {
            var res = req.CreateResponse(HttpStatusCode.Conflict);
            await res.WriteStringAsync(eventCreated.msg);
            return res;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { eventCreated });
        return response;

        //Return full EventDto
    }
}
