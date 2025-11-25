using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions;

public class ChangePassword
{
    private readonly ILogger<ChangePassword> _logger;
    private readonly JWTService _jwtService;
    private readonly IUserService _userService;
    public ChangePassword(ILogger<ChangePassword> logger, JWTService jwtService, IUserService userService)
    {
        _logger = logger;
        _jwtService = jwtService;
        _userService = userService;
    }

    [Function("ChangePassword")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {

        var detachid = _jwtService.DetachInfo(req);
        if (detachid.status == true)
        {
            var userid = detachid.valueLong;
            var body = await req.ReadFromJsonAsync<PasswordChangeDto>();

            if (body == null)
            {
                var badresponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badresponse.WriteStringAsync("Body is null or invalid");
                return badresponse;
            }
            var updatePassword = await _userService.ChangePassword(userid, body);

            if (updatePassword.status == true)
            {
                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(new
                {
                    status = updatePassword.status,
                    msg = updatePassword.msg
                });
                return response;
            }
            else
            {
                var response = req.CreateResponse(HttpStatusCode.BadRequest);
                await response.WriteAsJsonAsync(new
                {
                    status = updatePassword.status,
                    msg = updatePassword.msg
                });
                return response;
            }
        }
        else
        {
            var badresponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            await badresponse.WriteAsJsonAsync(new
            {
                status = detachid.status,
                msg = detachid.msg
            });
            return badresponse;
        }
    }
}