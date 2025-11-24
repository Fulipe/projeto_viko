using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Student;

public class EventRegistration
{
    private readonly ILogger<EventRegistration> _logger;
    private readonly JWTService _jwtService;
    private readonly IEventsService _eventsService;

    public EventRegistration(ILogger<EventRegistration> logger, JWTService jwtService, IEventsService eventsService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _eventsService = eventsService;
    }

    [Function("EventRegistration")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Student");
        if (roleCheck != null)
            return roleCheck;

        //get guid from request
        var request = await req.ReadAsStringAsync();

        if (request == null)
        {
            var res = req.CreateResponse(HttpStatusCode.BadRequest);
            await res.WriteStringAsync("Guid is null");
            return res;
        }

        var detachid = _jwtService.DetachInfo(req);
        if (detachid.status == true)
        {
            var userid = detachid.valueLong;

            var registration = await _eventsService.EventRegistration(userid, request);

            if (registration.status == false)
            {
                var badRes = req.CreateResponse(HttpStatusCode.NotFound);
                await badRes.WriteAsJsonAsync(new { status = registration.status, msg = registration.msg });
                return badRes;
            }

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { status = registration.status, msg = registration.msg });
            return res;
        }
        else 
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteAsJsonAsync(new { status = detachid.status, msg = detachid.msg });
            return badresponse;
        }
    }
}