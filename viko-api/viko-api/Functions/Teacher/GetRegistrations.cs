using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Teacher;

public class GetRegistrations
{
    private readonly ILogger<GetRegistrations> _logger;
    private readonly IEventsService _eventsService;

    public GetRegistrations(ILogger<GetRegistrations> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("GetRegistrations")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        //get guid from request
        var guid = req.Query["guid"];

        if (guid == null)
        {
            var badreq = req.CreateResponse(HttpStatusCode.BadRequest);
            await badreq.WriteStringAsync("Guid is null");
            return badreq;
        }

        var registrations = await _eventsService.RegistrationsList(guid);

        if (registrations.Item1.status == false)
        {
            var badres = req.CreateResponse(HttpStatusCode.NotFound);
            await badres.WriteStringAsync(registrations.Item1.msg);
            return badres;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { msg = registrations.Item1.msg, registrationsList = registrations.Item2 });
        return res;
    }
}