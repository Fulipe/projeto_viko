using Microsoft.AspNetCore.Http;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Models.Dto;
using viko_api.Services;
using static viko_api.Services.IUserService;

namespace viko_api.Functions.Teacher;

public class CreateEventTeacher
{
    private readonly ILogger<CreateEventTeacher> _logger;
    private readonly IEventsService _eventsService;
    private readonly IUserService _userService;
    private readonly JWTService _jwtService;

    public CreateEventTeacher(ILogger<CreateEventTeacher> logger, IEventsService eventsService, JWTService jwtService, IUserService userService)
    {
        _jwtService = jwtService;
        _eventsService = eventsService;
        _userService = userService;
        _logger = logger;
    }

    [Function("CreateEventTeacher")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Teacher");
        if (roleCheck != null)
            return roleCheck;

        //Receive a request with corresponding with EventCreationDto

        var request = await req.ReadFromJsonAsync<TeacherEventCreationDto>();

        if (request == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        var detachid = _jwtService.DetachInfo(req);

        if (detachid.status == true)
        {
            //Detach ID logged in from token 

            var teacherid = detachid.valueLong;

            //Send EventCreationDto and ID to CreateEvent() function in IEventService

            var eventCreated = await _eventsService.TeacherCreateEvent(teacherid, request);

            if (eventCreated.status == false)
            {
                var res = req.CreateResponse(HttpStatusCode.Conflict);
                await res.WriteAsJsonAsync(new { status = eventCreated.status, msg = eventCreated.msg });
                return res;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { status = eventCreated.status, msg = eventCreated.msg });
            return response;

            //Return full EventDto
        }
        else
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteAsJsonAsync(new { status = detachid.status, msg = detachid.msg });
            return badresponse;
        }
    }
}