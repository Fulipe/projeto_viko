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

public class GetTeacherEvents
{
    private readonly ILogger<GetTeacherEvents> _logger;
    private readonly JWTService _jwtService;
    private readonly IEventsService _eventService;

    public GetTeacherEvents(ILogger<GetTeacherEvents> logger, JWTService jwtService, IEventsService eventsService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _eventService = eventsService;
    }

    [Function("GetTeacherEvents")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var detachid = _jwtService.DetachInfo(req);
        if (detachid.status == true)
        {
            var userid = detachid.valueLong;

            ////Sends user ID to event Service
            var userEvent = await _eventService.GetTeacherEvents(userid);

            var responseDto = userEvent.Item1;
            var eventsFetched = userEvent.Item2;

            if (responseDto.status == false)
            {
                var res = req.CreateResponse(HttpStatusCode.OK);
                await res.WriteAsJsonAsync(new { status = responseDto.status, msg = responseDto.msg, eventsFetched});
                return res;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { status = responseDto.status, msg = responseDto.msg, eventsFetched });
            return response;

        }
        else
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteAsJsonAsync(new { status = detachid.status, msg = detachid.msg });
            return badresponse;
        }
    }
}