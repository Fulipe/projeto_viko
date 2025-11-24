using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using viko_api.Models.Dto;
using viko_api.Services;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace viko_api.Functions;

public class UpdateUser
{
    private readonly ILogger<UpdateUser> _logger;
    private readonly JWTService _jwtService;
    private readonly IConfiguration _config;
    private readonly IUserService _userService;

    public UpdateUser(ILogger<UpdateUser> logger, JWTService jwtservice, IUserService userService, IConfiguration config)
    {
        _logger = logger;
        _jwtService = jwtservice;
        _config = config;
        _userService = userService;
    }

    [Function("UpdateUser")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {

        var detachid = _jwtService.DetachInfo(req);

        if (detachid.status == true)
        {
            var userid = detachid.valueLong;
            var body = await req.ReadFromJsonAsync<UserInfoDto>();

            if (body == null)
            {
                var badresponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badresponse.WriteStringAsync("Body is null or invalid");
                return badresponse;
            }
            var update = await _userService.UpdateUser(userid, body);

            if (update.status == true)
            {
                var response = req.CreateResponse(HttpStatusCode.Created);
                await response.WriteAsJsonAsync(new
                {
                    status = update.status,
                    msg = update.msg
                });
                return response;
            }
            else
            {
                var response = req.CreateResponse(HttpStatusCode.NotModified);
                await response.WriteAsJsonAsync(new
                {
                    status = update.status,
                    msg = update.msg
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