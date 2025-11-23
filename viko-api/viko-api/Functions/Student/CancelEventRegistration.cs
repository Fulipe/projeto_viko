using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Student;

public class CancelEventRegistration
{
    private readonly ILogger<CancelEventRegistration> _logger;
    private readonly JWTService _jwtService;
    private readonly IEventsService _eventsService;
    public CancelEventRegistration(ILogger<CancelEventRegistration> logger, JWTService jwtService, IEventsService eventsService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _eventsService = eventsService;
    }

    [Function("CancelEventRegistration")]
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

            var registration = await _eventsService.CancelEventRegistration(userid, request);

            if (registration.status == false)
            {
                var badRes = req.CreateResponse(HttpStatusCode.NotFound);
                await badRes.WriteStringAsync(registration.msg);
                return badRes;
            }

            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { Status = registration.status, Msg = registration.msg });
            return res;
        }
        else
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteStringAsync(detachid.msg);
            return badresponse;
        }
    }
}