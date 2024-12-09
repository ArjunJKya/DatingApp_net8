using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimPrincipalExtensions
{
    public static string GetUserName(this ClaimsPrincipal user)
    {
        var username = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get username form token");
        return username;
    }
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userId = int.Parse (user.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new Exception("Cannot get userId form token"));
        return userId;
    }

}
