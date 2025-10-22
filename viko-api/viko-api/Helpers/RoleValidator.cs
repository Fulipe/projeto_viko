using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker;

namespace viko_api.Helpers
{
    public static class RoleValidator
    {
        public static async Task<HttpResponseData?> RequireRole(
            FunctionContext context, 
            HttpRequestData req, 
            params string[] allowedRoles)
        {
            if (!context.Items.TryGetValue("UserRole", out var roleObj))
            {
                var unauthorized = req.CreateResponse(HttpStatusCode.Unauthorized);
                await unauthorized.WriteStringAsync("Unauthorized: Missing user role.");
                return unauthorized;
            }

            var role = roleObj?.ToString();
            if (role == null || !allowedRoles.Contains(role))
            {
                var forbidden = req.CreateResponse(HttpStatusCode.Forbidden);
                await forbidden.WriteStringAsync("Access denied: insufficient permissions.");
                return forbidden;
            }

            return null;
        }
    }
}
