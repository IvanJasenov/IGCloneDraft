using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
    // retives data from the token recieved in the Request header
    public static class HttpContextExtention
    {
        public static string GetUsername(this HttpContext httpContext)
        {
            var username = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            return username;
        }
        
        public static int GetUserId(this HttpContext httpContext)
        {
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return int.Parse(userId);
        }
    }
}
