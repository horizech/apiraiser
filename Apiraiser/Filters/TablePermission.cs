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
    public class TablePermissionAttribute : TypeFilterAttribute
    {
        public TablePermissionAttribute(string schema = "", string table = "", string permission = "", string checkPropertyName = "", string insertPropertyName = "", string updatePropertyName = "") : base(typeof(TablePermissionFilter))
        {
            Arguments = new object[] { schema, table, permission, checkPropertyName, insertPropertyName, updatePropertyName };
        }
    }

    public class TablePermissionFilter : IAsyncActionFilter
    {
        string schema;
        string table;
        string permission;
        string checkPropertyName;
        string insertPropertyName;
        string updatePropertyName;

        public TablePermissionFilter(string schema = "", string table = "", string permission = "", string checkPropertyName = "", string insertPropertyName = "", string updatePropertyName = "")
        {
            this.schema = schema;
            this.table = table;
            this.permission = permission;
            this.checkPropertyName = checkPropertyName;
            this.insertPropertyName = insertPropertyName;
            this.updatePropertyName = updatePropertyName;
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

            if (string.IsNullOrEmpty(table))

            {
                object tableArgument = context.ActionArguments.ContainsKey("Table") ?
                    context.ActionArguments["Table"] :
                    context.ActionArguments.ContainsKey("table") ?
                    context.ActionArguments["table"] : null;

                if (tableArgument == null)
                {
                    context.Result = new ForbidResult(); return;
                }
                else
                {
                    table = (string)tableArgument;
                }
            }

            string userIdString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                context.Result = new ForbidResult(); return;
            }
            int userId = Int32.Parse(userIdString);

            string roleIdsString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleIdsString))
            {
                context.Result = new ForbidResult(); return;
            }
            List<int> roleIds = roleIdsString.Split(",").Select(x => Int32.Parse(x)).ToList();

            APIResult allTablePermissions = await ServiceManager.Instance.GetService<TableService>().GetRows(Schemas.Administration, TableNames.TablePermissions.ToString());

            List<Dictionary<string, object>> tablePermissions = ((List<Dictionary<string, object>>)allTablePermissions.Data)
                .Where(x => schema == x["Schema"].ToString() && table == x["Table"].ToString() && roleIds.Contains((int)x["Role"]))
                .ToList();

            if (tablePermissions.Count == 0)
            {
                context.Result = new ForbidResult(); return;
            }

            bool canPerformAction = tablePermissions.Any(x => bool.Parse(x[permission].ToString()) == true);

            if (!canPerformAction)
            {
                context.Result = new ForbidResult(); return;
            }

            int minUserLevel = tablePermissions.Where(x => bool.Parse(x[permission].ToString()) == true).Select(x => (int)x["UserAccessLevel"]).ToList().Min();

            if (minUserLevel != 1)
            {
                if (!string.IsNullOrEmpty(checkPropertyName))
                {
                    List<string> checkPropertyNameParts = checkPropertyName.Split('.').ToList();
                    if (checkPropertyNameParts.Count > 1)
                    {
                        object checkPropertyNameParam = context.ActionArguments[checkPropertyNameParts[0]];
                        for (int i = 1; i < checkPropertyNameParts.Count; i++)
                        {
                            if (checkPropertyNameParam is List<QuerySearchItem>)
                            {
                                if (((List<QuerySearchItem>)checkPropertyNameParam).Any(x => x.Name.ToLower() == checkPropertyNameParts[i].ToLower()))
                                {
                                    List<QuerySearchItem> parameterItemsToDelete = ((List<QuerySearchItem>)checkPropertyNameParam).Where(x => x.Name.ToLower() == checkPropertyNameParts[i].ToLower()).ToList();
                                    for (int j = 0; j < parameterItemsToDelete.Count; j++)
                                    {
                                        ((List<QuerySearchItem>)checkPropertyNameParam).Remove(parameterItemsToDelete[j]);
                                    }
                                }
                                ((List<QuerySearchItem>)checkPropertyNameParam).Add(
                                    new QuerySearchItem()
                                    {
                                        Name = checkPropertyNameParts[i],
                                        Value = userId,
                                        Condition = Enums.ColumnCondition.Equal,
                                        CaseSensitive = false
                                    }
                                );
                            }
                            else if (checkPropertyNameParam is UpdateRequest)
                            {
                                if (checkPropertyNameParts[i].ToLower() == "parameters")
                                {
                                    checkPropertyNameParam = ((UpdateRequest)checkPropertyNameParam).Parameters;
                                }
                                else if (checkPropertyNameParts[i].ToLower() == "data")
                                {
                                    checkPropertyNameParam = ((UpdateRequest)checkPropertyNameParam).Data;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(checkPropertyName) && context.ActionArguments.Any(x => x.Key.ToLower() == checkPropertyName.ToLower()))
                        {
                            Dictionary<string, object> argumentsToDelete = context.ActionArguments.Where(x => x.Key.ToLower() == checkPropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach (string keyToRemove in argumentsToDelete.Select(x => x.Key))
                            {
                                context.ActionArguments.Remove(keyToRemove);
                            }
                        }
                        context.ActionArguments.Add(checkPropertyName, userId);
                    }
                }

                string insertUpdatePropertyName = !string.IsNullOrEmpty(updatePropertyName) ? updatePropertyName : (!string.IsNullOrEmpty(insertPropertyName) ? insertPropertyName : "");

                if (!string.IsNullOrEmpty(insertUpdatePropertyName))
                {
                    List<string> updatePropertyNameParts = insertUpdatePropertyName.Split('.').ToList();
                    if (updatePropertyNameParts.Count > 1)
                    {
                        object updatePropertyNameParam = context.ActionArguments[updatePropertyNameParts[0]];
                        for (int i = 1; i < updatePropertyNameParts.Count; i++)
                        {
                            if (updatePropertyNameParam is Dictionary<string, object>)
                            {
                                if (((Dictionary<string, object>)updatePropertyNameParam).Any(x => x.Key.ToLower() == updatePropertyNameParts[i].ToLower()))
                                {
                                    Dictionary<string, object> dataItemsToDelete = ((Dictionary<string, object>)updatePropertyNameParam).Where(x => x.Key.ToLower() == updatePropertyNameParts[i].ToLower()).ToDictionary(x => x.Key, x => x.Value);
                                    foreach (string keyToRemove in dataItemsToDelete.Select(x => x.Key))
                                    {
                                        ((Dictionary<string, object>)updatePropertyNameParam).Remove(keyToRemove);
                                    }
                                }
                                ((Dictionary<string, object>)updatePropertyNameParam).Add(updatePropertyNameParts[i], userId);
                            }
                            else if (updatePropertyNameParam is List<Dictionary<string, object>>)
                            {
                                for (int j = 0; j < ((List<Dictionary<string, object>>)updatePropertyNameParam).Count; j++)
                                {
                                    if (((List<Dictionary<string, object>>)updatePropertyNameParam)[j].Any(x => x.Key.ToLower() == updatePropertyNameParts[i].ToLower()))
                                    {
                                        Dictionary<string, object> dataItemsToDelete = ((List<Dictionary<string, object>>)updatePropertyNameParam)[j].Where(x => x.Key.ToLower() == updatePropertyNameParts[i].ToLower()).ToDictionary(x => x.Key, x => x.Value);
                                        foreach (string keyToRemove in dataItemsToDelete.Select(x => x.Key))
                                        {
                                            ((List<Dictionary<string, object>>)updatePropertyNameParam)[j].Remove(keyToRemove);
                                        }
                                    }
                                    ((List<Dictionary<string, object>>)updatePropertyNameParam)[j].Add(updatePropertyNameParts[i], userId);
                                }
                            }
                            else if (updatePropertyNameParam is UpdateRequest)
                            {
                                if (updatePropertyNameParts[i].ToLower() == "parameters")
                                {
                                    updatePropertyNameParam = ((UpdateRequest)updatePropertyNameParam).Parameters;
                                }
                                else if (updatePropertyNameParts[i].ToLower() == "data")
                                {
                                    updatePropertyNameParam = ((UpdateRequest)updatePropertyNameParam).Data;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(insertUpdatePropertyName) && context.ActionArguments.Any(x => x.Key.ToLower() == insertUpdatePropertyName.ToLower()))
                        {
                            Dictionary<string, object> argumentsToDelete = context.ActionArguments.Where(x => x.Key.ToLower() == insertUpdatePropertyName.ToLower()).ToDictionary(x => x.Key, x => x.Value);
                            foreach (string keyToRemove in argumentsToDelete.Select(x => x.Key))
                            {
                                context.ActionArguments.Remove(keyToRemove);
                            }
                        }
                        context.ActionArguments.Add(insertUpdatePropertyName, userId);
                    }
                }
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
