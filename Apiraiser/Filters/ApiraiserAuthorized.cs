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
    public class ApiraiserAuthorizedAttribute : TypeFilterAttribute
    {
        public ApiraiserAuthorizedAttribute() : base(typeof(ApiraiserAuthorizedFilter))
        {
        }
    }

    public class ApiraiserAuthorizedFilter : IAsyncAuthorizationFilter
    {

        public ApiraiserAuthorizedFilter()
        {
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            string permissionGroupString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication)?.Value;
            if (string.IsNullOrEmpty(permissionGroupString))
            {
                context.Result = new ForbidResult();
            }
            else
            {
                APIResult permissions = await ServiceManager.Instance.GetService<PermissionsService>().GetPermissions();
                APIResult permissionGroupMapping = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();

                List<int> permissionGroupIds = permissionGroupString.Split(',').Select(x => Int32.Parse(x)).ToList();
                List<Dictionary<string, object>> userPermissionGroupMappings = ((List<Dictionary<string, object>>)permissionGroupMapping.Data).Where(x => permissionGroupIds.Contains(Int32.Parse(x["PermissionGroup"].ToString()))).ToList();
                if (userPermissionGroupMappings.Count == 0)
                {
                    context.Result = new ForbidResult();
                }
                else
                {
                    List<int> userPermissionIds = userPermissionGroupMappings.Select(x => Int32.Parse(x["Permission"].ToString())).ToList();
                    List<Dictionary<string, object>> userPermissions = ((List<Dictionary<string, object>>)permissions.Data).Where(x => userPermissionIds.Contains(Int32.Parse(x["Id"].ToString()))).ToList();
                    if (userPermissions.Count == 0)
                    {
                        context.Result = new ForbidResult();
                    }
                    else
                    {
                        var action = context.HttpContext.Request.RouteValues["action"];
                        var controller = context.HttpContext.Request.RouteValues["controller"];
                        var method = context.HttpContext.Request.Method;

                        string permission = string.Format("{0}.{1}.{2}", controller, action, method);
                        if (!userPermissions.Any(x => x["Name"].ToString() == permission))
                        {
                            context.Result = new ForbidResult();
                        }
                    }
                }
            }

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
