using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            // check of the user is authenticated, if the token was send from the user
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return; // if no authenticated, do nothing with this filter
            }
            // if authenticated
            var userId = resultContext.HttpContext.GetUserId();
            //var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var repo = resultContext.HttpContext.RequestServices.GetService<IUnitOfWork>();
            var user = await repo.UserRepository.GetUserByIdAsync(userId);
            user.LastActive = DateTime.Now;
            await repo.Complete();
        }
    }
}
