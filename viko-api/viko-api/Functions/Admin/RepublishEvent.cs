using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Admin;

public class RepublishEvent
{
    private readonly ILogger<RepublishEvent> _logger;
    private readonly IEventsService _eventsService;

    public RepublishEvent(ILogger<RepublishEvent> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("RepublishEvent")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin");
        if (roleCheck != null)
            return roleCheck;

        var guid = req.Query["guid"];

        if (string.IsNullOrEmpty(guid))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Guid is required.");
            return badResponse;
        }

        var republish = await _eventsService.RepublishEvent(guid);

        if (republish.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.NotModified);
            await badResponse.WriteStringAsync(republish.msg);
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { status = republish.status, msg = republish.msg });
        return res;


    }
}