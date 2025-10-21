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

        //Checks if request is empty
        if (body == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteStringAsync("Request is empty.");
            return badResponse;
        }

        //Password confirmation
        if (body.Password != body.ConfirmPassword)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new
            {
                Message = "Passwords confirmations do not match."
            });
            return badResponse;
        }

        if (body.BirthDate == null)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new
            {
                Message = "Please insert your birth date."
            });
            return badResponse;
        }

        //Validates fields according to Data Annotations
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

        if (user.status == false)
        {
            var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
            await badResponse.WriteAsJsonAsync(new {
                Status = user.status,
                Message = user.msg
            });
            return badResponse;
        }
        else { 
            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(new
            {
                Status = user.status,
                Message = user.msg
            });
            return response;
        }
    }
}