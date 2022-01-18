using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using DatingApp.Repository.Interface;
using System;

namespace DatingApp.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            //var userId = int.Parse(resultContext.HttpContext.User
            //    .FindFirst(ClaimTypes.NameIdentifier).Value);

            var userId = resultContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier).Value;

            var repo = resultContext.HttpContext.RequestServices.GetService<IDattingRepository>();

            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;
            repo.SaveLogUserActivity(user);

        }
    }
}
