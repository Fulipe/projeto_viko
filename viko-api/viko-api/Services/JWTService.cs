using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using viko_api.Functions;
using viko_api.Models.Dto;

namespace viko_api.Services
{
    public class JWTService
    {
        //private readonly string _issuer;
        //private readonly string _audience;
        private readonly ILogger<JWTService> _logger;
        private static IConfiguration _config;
        public JWTService(IConfiguration configuration, ILogger<JWTService> logger)
        {
            //pairs initiated fields with values in configuration 
            _logger = logger;
            _config = configuration;
            //_issuer = configuration["Jwt:Issuer"];
            //_audience = configuration["Jwt:Audience"]
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
                expires: DateTime.UtcNow.AddSeconds(20),
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
    }
}
