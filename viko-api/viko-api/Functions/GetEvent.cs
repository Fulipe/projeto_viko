using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions;

public class GetEvent
{
    private readonly ILogger<GetEvent> _logger;
    private readonly IEventsService _eventsService;

    public GetEvent(ILogger<GetEvent> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("GetEvent")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher", "Student");
        if (roleCheck != null)
            return roleCheck;

        var guid = req.Query["guid"];

        if (string.IsNullOrEmpty(guid))
        {
            var res = req.CreateResponse(HttpStatusCode.BadRequest);
            await res.WriteStringAsync("Guid is null");
            return res;
        }

        var getEvent = await _eventsService.GetEvent(guid);

        var responseDto = getEvent.Item1;
        var eventFetched = getEvent.Item2;

        if (responseDto.status == false)
        {
            var res = req.CreateResponse(HttpStatusCode.NotFound);
            await res.WriteStringAsync(getEvent.Item1.msg);
            return res;
        }


        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(
            new { 
                status = responseDto.status,
                msg =  responseDto.msg, 
                eventFetched = eventFetched
            });
        return response;

    }
}