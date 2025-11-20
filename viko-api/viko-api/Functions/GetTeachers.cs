using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Helpers;
using viko_api.Services;

namespace viko_api.Functions;

public class GetTeachers
{
    private readonly ILogger<GetTeachers> _logger;
    private readonly IUserService _userService;

    public GetTeachers(ILogger<GetTeachers> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [Function("GetTeachers")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext context)
    {
        var roleCheck = await RoleValidator.RequireRole(context, req, "Admin", "Teacher");
        if (roleCheck != null)
            return roleCheck;

        var getTeacher = await _userService.GetAllTeachers();

        var responseDto = getTeacher.Item1;
        var teachersList = getTeacher.Item2;

        if (responseDto.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.NotFound);
            await badResponse.WriteStringAsync(responseDto.msg);
            return badResponse;
        }

        var res = req.CreateResponse(HttpStatusCode.OK);
        await res.WriteAsJsonAsync(new {responseDto, teachers = teachersList});
        return res;
    }
}