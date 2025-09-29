using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions;

public class Signup
{
    private readonly ILogger<Signup> _logger;
    private readonly IUserService _userService;

    public Signup(ILogger<Signup> logger, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [Function("Signup")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var body = await req.ReadFromJsonAsync<SignUpRequestDto>();

        if (body == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        var validationContext = new ValidationContext(body);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(body, validationContext, validationResults, true))
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new
            {
                message = "Validation failed",
                errors = validationResults.Select(r => r.ErrorMessage).ToList()
            });
            return badResponse;
        }

        var user = await _userService.RegisterUser(body);

        if (user == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new
            {
                message = "Username or email is unavailable"
            });
            return badResponse;
        }
        else { 
            _logger.LogInformation(user.ToString());

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(user);
            return response;
        }
    }
}