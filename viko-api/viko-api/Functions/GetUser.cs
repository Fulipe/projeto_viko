using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using viko_api.Helpers;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions;

public class GetUser
{
    private readonly ILogger<GetUser> _logger;
    private readonly JWTService _jwtService;
    private readonly IConfiguration _config;
    private readonly IUserService _userService;

    public GetUser(ILogger<GetUser> logger, JWTService jwtservice, IUserService userService, IConfiguration config)
    {
        _logger = logger;
        _jwtService = jwtservice;
        _config = config;
        _userService = userService;        
    }

    [Function("GetUser")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher", "Student");
        if (roleCheck != null)
            return roleCheck; 

        var detachid = _jwtService.DetachInfo(req);

        var userRole = context.Items["UserRole"];

        if(detachid.status == true)
        {
            var userid = detachid.valueLong;

            //Sends user ID to User Service
            var user = await _userService.GetUserById(userid);

            if (user.Item1.status == false)
            {
                var res = req.CreateResponse(HttpStatusCode.NotFound);
                await res.WriteStringAsync(user.Item1.msg);
                return res;
            }

            var userLogged = user.Item2;

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(new { userLogged, Role = userRole });
            return response;

        } else {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteStringAsync(detachid.msg);
            return badresponse;
        } 
    }
}