using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using viko_api.Functions;
using viko_api.Models.Dto;

namespace viko_api.Services
{
    public class JWTService
    {
        private readonly ILogger<JWTService> _logger;
        private static IConfiguration _config;

        public JWTService(IConfiguration configuration, ILogger<JWTService> logger)
        {
            //pairs initiated fields with values in configuration 
            _logger = logger;
            _config = configuration;
        }

        public string GenerateJwtToken(UserDto user)
        {
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("userId", user.Id.ToString()),
                //new Claim("username", user.Username.ToString()),
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
        public static bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityKey = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // JWT must be validated by signature key  
                    IssuerSigningKey = new SymmetricSecurityKey(securityKey), // Defines hash used to validate JWT signature 
                    ValidateIssuer = false, // Validates issuer
                    ValidateAudience = false, // Validates audience
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true; // Token is Valid
            }
            catch
            {
                return false; // Invalid token
            }
        }

        public ResponseDto DetachId(HttpRequestData req)
        {
            //Get token from Headers
            if (!req.Headers.TryGetValues("Authorization", out var authHeaders))
            {
                return new ResponseDto()
                {
                    status = false,
                    msg = "Missing Authorization header"
                };
            };

            //Checks if incoming token has Bearer validation
            var bearer = authHeaders.FirstOrDefault();
            if (string.IsNullOrEmpty(bearer) || !bearer.StartsWith("Bearer "))
            {
                return new ResponseDto()
                {
                    status = false,
                    msg = "Invalid token format"
                };
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

                //Gets User ID from claims
                var userIdClaim = principal.FindFirst("userId")?.Value;
                if (userIdClaim == null)
                {
                    return new ResponseDto
                    {
                        status = false,
                        msg = "Missing userId in token"
                    };
                }

                int userId = int.Parse(userIdClaim);
                return new ResponseDto
                {
                    status = true,
                    msg = "User Id successfully detached",
                    value = userId
                };

            }
            catch (SecurityTokenException)
            {
                return new ResponseDto
                {
                    status = false,
                    msg = "Invalid or expired token"
                };
            }
        }
    }
}
