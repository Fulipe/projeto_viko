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
using viko_api.Services;

namespace viko_api.Functions;

public class UserFunction
{
    private readonly ILogger<UserFunction> _logger;
    private readonly JWTService _jwtService;
    private readonly IConfiguration _config;
    private readonly IUserService _userService;

    public UserFunction(ILogger<UserFunction> logger, JWTService jwtservice, IUserService userService, IConfiguration config)
    {
        _logger = logger;
        _jwtService = jwtservice;
        _config = config;
        _userService = userService;        
    }

    [Function("Profile")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
    {
        if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
        {
            var res = req.CreateResponse(HttpStatusCode.Unauthorized);
            await res.WriteStringAsync("Missing Authorization header");
            return res;
        }

        var bearer = authHeaders.FirstOrDefault();
        if (string.IsNullOrEmpty(bearer) || !bearer.StartsWith("Bearer "))
        {
            var res = req.CreateResponse(HttpStatusCode.Unauthorized);
            await res.WriteStringAsync("Invalid token format");
            return res;
        }

        var token = bearer.Substring("Bearer ".Length).Trim();

        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out _);

            var userIdClaim = principal.FindFirst("userId")?.Value;
            if (userIdClaim == null)
            {
                var res = req.CreateResponse(HttpStatusCode.Unauthorized);
                await res.WriteStringAsync("Missing userId in token");
                return res;
            }

            int userId = int.Parse(userIdClaim);

            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                var res = req.CreateResponse(HttpStatusCode.NotFound);
                await res.WriteStringAsync("User not found");
                return res;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(user);
            return response;
        }
        catch (SecurityTokenException)
        {
            var res = req.CreateResponse(HttpStatusCode.Unauthorized);
            await res.WriteStringAsync("Invalid or expired token");
            return res;
        }
    }
}