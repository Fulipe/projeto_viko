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

public class CreateEvent
{
    private readonly ILogger<CreateEvent> _logger;
    private readonly IEventsService _eventsService;
    private readonly JWTService _jwtService;

    public CreateEvent(ILogger<CreateEvent> logger, IEventsService eventsService, JWTService jwtService)
    {
        _jwtService = jwtService;
        _eventsService = eventsService;
        _logger = logger;
    }

    [Function("CreateEvent")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        //Receive a request with corresponding with EventCreationDto

        var request = await req.ReadFromJsonAsync<EventCreationDto>();

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

            //Sends user ID to User Service
            var eventCreated = await _eventsService.CreateEvent(teacherid, request);

            //if (user.Item1.status == false)
            //{
            //    var res = req.CreateResponse(HttpStatusCode.NotFound);
            //    await res.WriteStringAsync(user.Item1.msg);
            //    return res;
            //}

            //var userLogged = user.Item2;

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { eventCreated });
            return response;

        //Return full EventDto
        }
        else
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteStringAsync(detachid.msg);
            return badresponse;
        }


    }
}