using BLL.Interfaces;
using BLL.Services;
using DAL.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Presentaion.Controllers
{
    public class CustomAuth : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }

        public CustomAuth (string roles)
        {
            Roles = roles;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // if is not authenticated
            if (context.HttpContext.User?.Identity == null || !context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" }
                });
                return;
            }

            var requestedUrl = context.HttpContext.Request.Path.Value;
            if (requestedUrl == null || !IsAuthorizedAsync(context.HttpContext.User, requestedUrl))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "controller", "Home" },
                    { "action", "AccessDenied" }
                });
            }
        }

        private bool IsAuthorizedAsync(ClaimsPrincipal user, string requestedUrl)
        {
            var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim == null)
            {
                return false;
            }

            if (Roles.Contains(roleClaim.Value))
            {
                return true;
            }
            var role = roleClaim.Value;
            return false;
        }
    }
}