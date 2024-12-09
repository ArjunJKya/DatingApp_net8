using System;
using API.Extensions;
using API.Interface;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Healper;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resutlContext = await next();

        if(context.HttpContext.User.Identity?.IsAuthenticated != true) return;

        var userId = resutlContext.HttpContext.User.GetUserId();
        
        var repo = resutlContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

        var user = await repo.GetUserByIdAsync(userId);

        if(user == null)return;
        user.LastActive = DateTime.Now;
        await repo.SaveAllAsync();

    }
}
