using System.Diagnostics;
using System;
using System.Web;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Apiraiser.Enums;
using Apiraiser.Constants;
using Apiraiser.Services;
using Apiraiser.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Apiraiser.Filters
{
    public class SystemPermissionAttribute : TypeFilterAttribute
    {
        public SystemPermissionAttribute(string schema = "", string permission = "") : base(typeof(SystemPermissionFilter))
        {
            Arguments = new object[] { schema, permission };
        }
    }

    public class SystemPermissionFilter : IAsyncActionFilter
    {
        string schema;
        string permission;

        public SystemPermissionFilter(string schema = "", string permission = "")
        {
            this.schema = schema;
            this.permission = permission;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (string.IsNullOrEmpty(permission))
            {
                context.Result = new ForbidResult(); return;
            }

            if (string.IsNullOrEmpty(schema))
            {
                object schemaArgument = context.ActionArguments.ContainsKey("Schema") ?
                    context.ActionArguments["Schema"] :
                    context.ActionArguments.ContainsKey("schema") ?
                    context.ActionArguments["schema"] : null;

                if (schemaArgument == null)
                {
                    context.Result = new ForbidResult(); return;
                }
                else
                {
                    schema = (string)schemaArgument;
                }
            }

            string roleIdsString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleIdsString))
            {
                context.Result = new ForbidResult(); return;
            }
            List<int> roleIds = roleIdsString.Split(",").Select(x => Int32.Parse(x)).ToList();

            APIResult allSystemPermissions = await ServiceManager.Instance.GetService<TableService>().GetRows(Schemas.Administration, TableNames.SystemPermissions.ToString());

            List<Dictionary<string, object>> systemPermissions = ((List<Dictionary<string, object>>)allSystemPermissions.Data)
                .Where(x => (schema == x["Schema"] as string) && roleIds.Contains((int)x["Role"]))
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
