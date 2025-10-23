//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Functions.Worker;
//using Microsoft.Azure.Functions.Worker.Http;
//using Microsoft.Extensions.Logging;
//using viko_api.Helpers;
//using viko_api.Services;

//namespace viko_api.Functions;

//public class GetUserEvents
//{
//    private readonly ILogger<GetUserEvents> _logger;
//    private readonly JWTService _jwtService;

//    public GetUserEvents(ILogger<GetUserEvents> logger, JWTService jwtService)
//    {
//        _logger = logger;
//        _jwtService = jwtService;
//    }

//    [Function("GetUserEvents")]
//    public async Task<HttpResponseData>  Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
//    {
//        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher", "Student");
//        if (roleCheck != null)
//            return roleCheck;

//        var detachid = _jwtService.DetachInfo(req);

//        if (detachid.status == null)
//        {
//            var userid = detachid.valueLong;
//        }



//    }
//}