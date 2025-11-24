using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions;

public class GetEventOfUser
{
    private readonly ILogger<GetEventOfUser> _logger;
    private readonly IEventsService _eventsService;

    public GetEventOfUser(ILogger<GetEventOfUser> logger, IEventsService eventsService)
    {
        _eventsService = eventsService;
        _logger = logger;
    }

    [Function("GetEventOfUser")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var guid = req.Query["guid"];

        if (guid == null)
        {
            var badreq = req.CreateResponse(HttpStatusCode.BadRequest);
            await badreq.WriteStringAsync("Guid is null");
            return badreq;
        }

        var getEventOfUser = await _eventsService.GetEventOfUser(guid);

        var responseDto = getEventOfUser.Item1;
        var eventsList = getEventOfUser.Item2;

        if (getEventOfUser.Item1.status == false)
        {
            var badres = req.CreateResponse(HttpStatusCode.NotFound);
            await badres.WriteAsJsonAsync(new { status = responseDto.status, msg = responseDto.msg, eventsList = eventsList });
            return badres;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { status = responseDto.status, msg = responseDto.msg, eventsList = eventsList });
        return res;
    }
}