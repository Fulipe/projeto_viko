using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using viko_api.Services;

namespace viko_api.Middleware
{
    public class JWTValidationMiddleware : IFunctionsWorkerMiddleware
    {
        private readonly JWTService _jwtService;
        public JWTValidationMiddleware(JWTService jwtservice)
        {
            _jwtService = jwtservice;
        }
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpRequest = await context.GetHttpRequestDataAsync(); //gets inbound request
            var tokenWithBearer = httpRequest?.Headers.GetValues("Authorization").FirstOrDefault(); //fetch from req header value of "Authorization"
            var token = tokenWithBearer?.StartsWith("Bearer ") == true
                ? tokenWithBearer.Substring("Bearer ".Length).Trim()
                : tokenWithBearer;

            // Verifies if req or token exists; 
            if (httpRequest == null || string.IsNullOrEmpty(token))
            {
                //If not (req/token is null), then, token is invalid responds with 401 - Unauthorized
                var response = httpRequest?.CreateResponse(HttpStatusCode.Unauthorized);

                //if response init wasn't succesfull
                if (response != null)
                {
                    //Sends Unauthorized anyways
                    await response.WriteStringAsync("Unauthorized: Missing token.");
                    context.GetInvocationResult().Value = response;
                }
                return;
            }

            // Validates token with JWTService
            var tokenValid = _jwtService.ValidateToken(token);

            if (!tokenValid)
            {
                var response = httpRequest.CreateResponse(HttpStatusCode.Unauthorized);
                await response.WriteStringAsync("Unauthorized: Invalid token.");
                context.GetInvocationResult().Value = response;
                return;
            }

            // Detach user data (id e role)
            var responseDto = _jwtService.DetachInfo(httpRequest);
            if (!responseDto.status)
            {
                var response = httpRequest.CreateResponse(HttpStatusCode.Unauthorized);
                await response.WriteStringAsync(responseDto.msg);
                context.GetInvocationResult().Value = response;
                return;
            }

            var userId = responseDto.valueInt;
            var userRole = responseDto.valueString;

            context.Items["UserId"] = userId;
            context.Items["UserRole"] = userRole!;

            await next(context);
        }
    }
}
