using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using viko_api.Models;
using viko_api.Models.Dto;
using viko_api.Services;

namespace viko_api.Functions;

public class Login
{
    private readonly ILogger<Login> _logger;
    private readonly VikoDbContext _dbContext;
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public Login(ILogger<Login> logger, VikoDbContext dbContext, IConfiguration config, IUserService userService)
    {
        _logger = logger;
        _userService = userService;
        _dbContext = dbContext;
        _config = config;
    }

    [Function("Login")]

    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
    {
        var body = await req.ReadFromJsonAsync<LoginRequest>();

        var user = await _userService.Authenticate(body.Username, body.Password);

        if (user == null)
        {
            _logger.LogInformation("USER NULL!");

            var nullResponse = req.CreateResponse(HttpStatusCode.Unauthorized);
            return nullResponse;
        }

        _logger.LogInformation("SUCCESS!!!");
        
        var token = GenerateJwtToken(user);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(new { token });
        return response;

    }
    private string GenerateJwtToken(UserDto user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim("userId", user.Id.ToString()),
            new Claim("entityId", user.EntityId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    private record LoginRequest(string Username, string Password);
}