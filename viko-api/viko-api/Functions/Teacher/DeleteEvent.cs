using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Teacher;

public class DeleteEvent
{
    private readonly ILogger<DeleteEvent> _logger;
    private readonly IEventsService _eventsService;
    public DeleteEvent(ILogger<DeleteEvent> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("DeleteEvent")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "delete")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var guid = req.Query["guid"];

        if (string.IsNullOrEmpty(guid))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        var hideEvent = await _eventsService.DeleteEvent(guid);

        if (hideEvent.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.NotModified);
            await badResponse.WriteAsJsonAsync(new { status = hideEvent.status, msg = hideEvent.msg });
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new {status = hideEvent.status, msg= hideEvent.msg });
        return res;
    }
}