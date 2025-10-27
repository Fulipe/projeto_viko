using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions;

public class GetStudentEvents
{
    private readonly ILogger<GetStudentEvents> _logger;
    private readonly JWTService _jwtService;
    private readonly IEventsService _eventService;

    public GetStudentEvents(ILogger<GetStudentEvents> logger, JWTService jwtService, IEventsService eventsService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _eventService = eventsService;
    }

    [Function("GetStudentEvents")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Student");
        if (roleCheck != null)
            return roleCheck;

        var detachid = _jwtService.DetachInfo(req);
        if (detachid.status == true)
        {
            var userid = detachid.valueLong;

            ////Sends user ID to event Service
            var userEvent = await _eventService.GetStudentEvents(userid); //deactivate to API Test
            var eventsFetched = userEvent.Item2;

            //API Test
            //var eventsFetched = new List<EventsDto>
            //{
            //    new EventsDto
            //    {
            //        Title = "Tech Summit 2025",
            //        Image = "https://example.com/image.jpg",
            //        Description = "Um evento de tecnologia e inovação.",
            //        Category = "Technology",
            //        Location = "Lisboa",
            //        Language = "Portuguese, English",
            //        StartDate = DateTime.UtcNow.AddDays(5),
            //        EndDate = DateTime.UtcNow.AddDays(6),
            //        RegistrationDeadline = DateTime.UtcNow.AddDays(3),
            //        EventStatus = 3
            //    },
            //    new EventsDto
            //    {
            //        Title = "Bread Summit 2025",
            //        Image = "https://example.com/image.jpg",
            //        Description = "Um evento de tecnologia e inovação.",
            //        Category = "Technology",
            //        Location = "Lisboa",
            //        Language = "Portuguese, English",
            //        StartDate = DateTime.UtcNow.AddDays(5),
            //        EndDate = DateTime.UtcNow.AddDays(6),
            //        RegistrationDeadline = DateTime.UtcNow.AddDays(3),
            //        EventStatus = 1
            //    },
            //    new EventsDto
            //    {
            //        Title = "Wine Summit 2025",
            //        Image = "https://example.com/image.jpg",
            //        Description = "Um evento de tecnologia e inovação.",
            //        Category = "Technology",
            //        Location = "Lisboa",
            //        Language = "Portuguese, English",
            //        StartDate = DateTime.UtcNow.AddDays(5),
            //        EndDate = DateTime.UtcNow.AddDays(6),
            //        RegistrationDeadline = DateTime.UtcNow.AddDays(3),
            //        EventStatus = 2
            //    },
            //    new EventsDto
            //    {
            //        Title = "Juice Summit 2025",
            //        Image = "https://example.com/image.jpg",
            //        Description = "Um evento de tecnologia e inovação.",
            //        Category = "Technology",
            //        Location = "Lisboa",
            //        Language = "Portuguese, English",
            //        StartDate = DateTime.UtcNow.AddDays(5),
            //        EndDate = DateTime.UtcNow.AddDays(6),
            //        RegistrationDeadline = DateTime.UtcNow.AddDays(3),
            //        EventStatus = 1
            //    }
            //};

            if (userEvent.Item1.status == false)
            {
                var res = req.CreateResponse(HttpStatusCode.OK);
                await res.WriteAsJsonAsync(new { userEvent.Item1.msg });
                return res;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { eventsFetched, userEvent.Item1.msg});
            return response;

        }
        else
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteStringAsync(detachid.msg);
            return badresponse;
        }
    }
}