using System.Diagnostics;
using System;
using System.Web;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Apiraiser.Services;
using Apiraiser.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Apiraiser.Filters
{
    public class SystemPermissionAttribute : TypeFilterAttribute
    {
        public SystemPermissionAttribute(string permission = "") : base(typeof(SystemPermissionFilter))
        {
            Arguments = new object[] { permission };
        }
    }

    public class SystemPermissionFilter : IAsyncActionFilter
    {
        string permission;

        public SystemPermissionFilter(string permission = "")
        {
            this.permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (string.IsNullOrEmpty(permission))
            {
                context.Result = new ForbidResult(); return;
            }


            string roleIdString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleIdString))
            {
                context.Result = new ForbidResult(); return;
            }
            int roleId = Int32.Parse(roleIdString);

            APIResult allSystemPermissions = await ServiceManager.Instance.GetService<SystemPermissionsService>().GetSystemPermissions();

            List<Dictionary<string, object>> systemPermissions = ((List<Dictionary<string, object>>)allSystemPermissions.Data)
                .Where(x => roleId == Int32.Parse(x["Role"].ToString()))
                .ToList();

            if (systemPermissions.Count == 0)
            {
                context.Result = new ForbidResult(); return;
            }

            bool canPerformAction = systemPermissions.Any(x => bool.Parse(x[permission].ToString()) == true);

            if (!canPerformAction)
            {
                context.Result = new ForbidResult(); return;
            }

            await next();
            // var action = context.HttpContext.Request.RouteValues["action"];
            // var controller = context.HttpContext.Request.RouteValues["controller"];
            // var method = context.HttpContext.Request.Method;

            // string permission = string.Format("{0}.{1}.{2}",controller, action, method);

            // var hasClaim = context.HttpContext.User.Claims.Any(c => 
            //     c.Type == ClaimTypes.Authentication &&
            //     c.Value.Contains(permission));
            // if (!hasClaim)
            // {
            //     context.Result = new ForbidResult();

            //     // context.Result = new RedirectToRouteResult(
            //     //     new RouteValueDictionary {
            //     //         {"controller", "Errors"}, {"action", "Error"}, { "errorCode", "AUTH001"}, {"errorMessage", "Unauthorized! " }
            //     //     }
            //     // );

            //     // context.Result = new JsonResult( new 
            //     // {
            //     //     Success = false,
            //     //     Data = (string)null,
            //     //     ErrorCode = "AUTH001",
            //     //     Message = "Unauthorized: " + permission
            //     // });
            // }
        }
    }
}
