using System.Diagnostics;
using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

using Apiraiser.Helpers;
using Apiraiser.Enums;
using Apiraiser.Databases;
using Apiraiser.Exceptions;
using Apiraiser.Mappings;
using Apiraiser.Models;
using Apiraiser.Services;
using Apiraiser.Constants;
using Apiraiser.Interfaces;

namespace Apiraiser.Filters
{
    public class ApiraiserSelectDataAttribute : TypeFilterAttribute
    {
        public ApiraiserSelectDataAttribute() : base(typeof(ApiraiserSelectDataFilter))
        {
        }
    }

    public class ApiraiserSelectDataFilter : IAuthorizationFilter
    {

        public ApiraiserSelectDataFilter()
        {
        }

        public async void OnAuthorization(AuthorizationFilterContext context)
        {
            var action = context.HttpContext.Request.RouteValues["action"];
            var controller = context.HttpContext.Request.RouteValues["controller"];
            var method = context.HttpContext.Request.Method;

            string permission = string.Format("{0}.{1}.{2}", controller, action, method);

            List<ColumnInfo> columnDefinitions = await ServiceManager.Instance.GetService<DatabaseService>().GetDatabaseDriver().GetTableColumns(Schemas.Administration, TableNames.InsertionOverrides.ToString());

            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Administration, table: TableNames.InsertionOverrides.ToString())
                .WhereEquals("Action", permission)
                .WhereEquals("Type", "INSERT_COLUMN_VALUE")
                .AddColumnDefinitions(columnDefinitions)
                .RunSelectQuery();


            var hasClaim = context.HttpContext.User.Claims.Any(c =>
                c.Type == ClaimTypes.Authentication &&
                c.Value.Contains(permission));
            if (!hasClaim)
            {
                context.Result = new ForbidResult();

                // context.Result = new RedirectToRouteResult(
                //     new RouteValueDictionary {
                //         {"controller", "Errors"}, {"action", "Error"}, { "errorCode", "AUTH001"}, {"errorMessage", "Unauthorized! " }
                //     }
                // );

                // context.Result = new JsonResult( new 
                // {
                //     Success = false,
                //     Data = (string)null,
                //     ErrorCode = "AUTH001",
                //     Message = "Unauthorized: " + permission
                // });
            }
        }
    }
}
