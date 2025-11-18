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

public class UpdateEventStatus
{
    private readonly ILogger<UpdateEventStatus> _logger;
    private readonly IEventsService _eventsService;
    public UpdateEventStatus(ILogger<UpdateEventStatus> logger, IEventsService eventsService)
    {
        _logger = logger;
        _eventsService = eventsService;
    }

    [Function("UpdateEventStatus")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var request = await req.ReadFromJsonAsync<UpdateStatusRequest>();

        if (request == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        var updateStatus = await _eventsService.UpdateEventStatus(request.guid, request.eventStatus);

        if (updateStatus.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await badResponse.WriteStringAsync(updateStatus.msg);
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { status = updateStatus.status, msg = updateStatus.msg });
        return res;

    }
}
public class UpdateStatusRequest
{
    public string guid { get; set; }
    public int eventStatus { get; set; }
}