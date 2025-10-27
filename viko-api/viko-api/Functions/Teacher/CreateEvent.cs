using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;

namespace viko_api.Functions.Teacher;

//public class CreateEvent
//{
//    private readonly ILogger<CreateEvent> _logger;

//    public CreateEvent(ILogger<CreateEvent> logger)
//    {
//        _logger = logger;
//    }

//    [Function("CreateEvent")]
//    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
//    {
//        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
//        if (roleCheck != null)
//            return roleCheck;

        //Receive a request with corresponding with EventCreationDto
        //Send to CreateEvent() function in IEventService
        //Return full EventDto

//    }
//}