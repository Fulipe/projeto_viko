using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions;

public class GetUserViewUser
{
    private readonly ILogger<GetUserViewUser> _logger;
    private readonly IUserService _userService;

    public GetUserViewUser(ILogger<GetUserViewUser> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [Function("GetUserViewUser")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var guid = req.Query["guid"];

        if (string.IsNullOrEmpty(guid))
        {
            var res = req.CreateResponse(HttpStatusCode.BadRequest);
            await res.WriteStringAsync("Guid is null");
            return res;
        }

        var getUser = await _userService.GetUserByGUID(guid);
        var responseDto = getUser.Item1;
        var userFetched = getUser.Item2; 

        if (responseDto.status == false)
        {
            var res = req.CreateResponse(HttpStatusCode.NotFound);
            await res.WriteAsJsonAsync(new{
                status = responseDto.status,
                msg = responseDto.msg,
                userFetched = userFetched
            });
            return res;
        }


        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(
            new
            {
                status = responseDto.status,
                msg = responseDto.msg,
                userFetched = userFetched
            });
        return response;

    }
}