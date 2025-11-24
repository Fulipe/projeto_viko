using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Admin;

public class GetAllUsers
{
    private readonly ILogger<GetAllUsers> _logger;
    private readonly IUserService _userService;

    public GetAllUsers(ILogger<GetAllUsers> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [Function("GetAllUsers")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin");
        if (roleCheck != null)
            return roleCheck;

        var getUsers = await _userService.GetAllUsers();

        var responseDto = getUsers.Item1;
        var usersList = getUsers.Item2;

        if (responseDto.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await badResponse.WriteAsJsonAsync(new { status = responseDto.status, msg = responseDto.msg, users = usersList });
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new { status = responseDto.status, msg = responseDto.msg, users = usersList });
        return res;
    }
}