using System.Net;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions.Admin;

public class UpdateUserRole
{
    private readonly ILogger<UpdateUserRole> _logger;
    private readonly IUserService _userService;

    public UpdateUserRole(ILogger<UpdateUserRole> logger, IUserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    [Function("UpdateUserRole")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin");
        if (roleCheck != null)
            return roleCheck;

        var body = await req.ReadFromJsonAsync<UpdateRoleDto>();

        if (body == null)
        {
            var res = req.CreateResponse(HttpStatusCode.BadRequest);
            await res.WriteStringAsync("Body is null");
            return res;
        }

        var update = await _userService.UpdateUserRole(body.Username, body.Role);

        if (update.status == false)
        {
            var res = req.CreateResponse(HttpStatusCode.NotModified);
            await res.WriteAsJsonAsync(new {status = update.status, msg = update.msg});
            return res;
        }

        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { status = update.status, msg = update.msg });
        return response;

    }

    public class UpdateRoleDto
    {
        public string Username { get; set; }
        public string Role { get; set; }
    }
}