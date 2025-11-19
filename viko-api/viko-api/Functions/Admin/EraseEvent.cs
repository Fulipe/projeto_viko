using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Admin;

public class EraseEvent
{
    private readonly ILogger<EraseEvent> _logger;
    private readonly IEventsService _eventsService;

    public EraseEvent(ILogger<EraseEvent> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("EraseEvent")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin");
        if (roleCheck != null)
            return roleCheck;

        var guid = req.Query["guid"];

        if (string.IsNullOrEmpty(guid))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        var eraseEvent = await _eventsService.EraseEvent(guid);

        if (eraseEvent.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync(eraseEvent.msg);
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { status = eraseEvent.status, msg = eraseEvent.msg });
        return res;
    }
}