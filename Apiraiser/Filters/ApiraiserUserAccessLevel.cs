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

    public class ApiraiserUserAccessLevelAttribute : TypeFilterAttribute
    {
        public ApiraiserUserAccessLevelAttribute(string checkPropertyName = "", string insertPropertyName = "", string updatePropertyName = "") : base(typeof(ApiraiserUserAccessLevelFilter))
        {
            Arguments = new object[] { checkPropertyName, insertPropertyName, updatePropertyName };
        }
    }

    public class ApiraiserUserAccessLevelFilter : IAsyncActionFilter
    {
        string checkPropertyName;
        string insertPropertyName;
        string updatePropertyName;

        public ApiraiserUserAccessLevelFilter(string checkPropertyName, string insertPropertyName, string updatePropertyName)
        {
            this.checkPropertyName = checkPropertyName;
            this.insertPropertyName = insertPropertyName;
            this.updatePropertyName = updatePropertyName;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string userIdString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(userIdString))
            {
                context.Result = new ForbidResult(); return;
            }
            int userId = Int32.Parse(userIdString);

            string roleIdString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (string.IsNullOrEmpty(roleIdString))
            {
                context.Result = new ForbidResult(); return;
            }
            int roleId = Int32.Parse(roleIdString);

            string permissionGroupString = context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication)?.Value;
            if (string.IsNullOrEmpty(permissionGroupString))
            {
                context.Result = new ForbidResult(); return;
            }
            List<int> permissionGroupIds = permissionGroupString.Split(',').Select(x => Int32.Parse(x)).ToList();

            APIResult rolePermissionGroupMappings = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().GetRolePermissionGroupMappings();
            List<Dictionary<string, object>> userRolePermissionGroupMapping = ((List<Dictionary<string, object>>)rolePermissionGroupMappings.Data).Where(x => (int)x["Role"] == roleId).ToList();
            if (userRolePermissionGroupMapping.Count == 0)
            {
                context.Result = new ForbidResult(); return;
            }

            APIResult permissionGroupMappings = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();
            List<Dictionary<string, object>> userPermissionGroupMappings = ((List<Dictionary<string, object>>)permissionGroupMappings.Data).Where(x => permissionGroupIds.Contains((int)x["PermissionGroup"])).ToList();
            if (userPermissionGroupMappings.Count == 0)
            {
                context.Result = new ForbidResult(); return;
            }
            List<int> userPermissionIds = userPermissionGroupMappings.Select(x => Int32.Parse(x["Permission"].ToString())).ToList();

            APIResult permissions = await ServiceManager.Instance.GetService<PermissionsService>().GetPermissions();
            List<Dictionary<string, object>> userPermissions = ((List<Dictionary<string, object>>)permissions.Data).Where(x => userPermissionIds.Contains(Int32.Parse(x["Id"].ToString()))).ToList();
            if (userPermissions.Count == 0)
            {
                context.Result = new ForbidResult(); return;
            }

            var action = context.HttpContext.Request.RouteValues["action"];
            var controller = context.HttpContext.Request.RouteValues["controller"];
            var method = context.HttpContext.Request.Method;

            string permission = string.Format("{0}.{1}.{2}", controller, action, method);
            if (!userPermissions.Any(x => x["Name"].ToString() == permission))
            {
                context.Result = new ForbidResult(); return;
            }

            Dictionary<string, object> actionPermission = userPermissions.First(x => x["Name"].ToString() == permission);
            Dictionary<string, object> actionPermissionGroupMapping = userPermissionGroupMappings.First(x => (int)x["Permission"] == (int)actionPermission["Id"]);
            Dictionary<string, object> actionRolePermissionGroupMapping = userRolePermissionGroupMapping.First(x => (int)x["PermissionGroup"] == (int)actionPermissionGroupMapping["PermissionGroup"]);
            if (((int)actionRolePermissionGroupMapping["UserAccessLevel"]) != 1)
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
        }
    }

}
