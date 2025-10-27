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

public class GetAllEvents
{
    private readonly ILogger<GetAllEvents> _logger;
    private readonly JWTService _jwtService;
    private readonly IEventsService _eventService;

    public GetAllEvents(ILogger<GetAllEvents> logger, JWTService jwtService, IEventsService eventsService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _eventService = eventsService;
    }

    [Function("GetAllEvents")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher", "Student");
        if (roleCheck != null)
            return roleCheck;

        var events = await _eventService.GetAllPublicEvents();
        var eventsFetched = events.Item2;
        ////API Test
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

        if (events.Item1.status == false)
        {
            var res = req.CreateResponse(HttpStatusCode.OK);
            await res.WriteAsJsonAsync(new { events.Item1.msg });
            return res;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { eventsFetched, events.Item1.msg });
        return response;    

    }
}