using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

using Apiraiser.Databases;
using Apiraiser.Models;
using Apiraiser.Helpers;
using Apiraiser.Constants;
using Apiraiser.Enums;
using Apiraiser.Mappings;

using Microsoft.Extensions.Configuration;

namespace Apiraiser.Services
{
    public class UsersService : BaseService
    {

        public UsersService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> GetUserRole(int Id)
        {
            try
            {
                List<Dictionary<string, object>> userRoles = await QueryDesigner
                    .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                    .WhereEquals("User", Id)
                    .RunSelectQuery();

                if ((userRoles?.Count ?? 0) > 0)
                {
                    if ((userRoles[0]["Role"]?.ToString().Length ?? 0) > 0)
                    {
                        APIResult rolesResult = await ServiceManager.Instance.GetService<RolesService>().GetRoles();

                        List<Dictionary<string, object>> roles = ((List<Dictionary<string, object>>)rolesResult.Data).Where(x => (int)x["Id"] == (int)userRoles[0]["Role"]).ToList();

                        if ((roles?.Count ?? 0) > 0)
                        {
                            if ((roles[0]["Name"]?.ToString().Length ?? 0) > 0)
                            {
                                return new APIResult()
                                {
                                    Success = true,
                                    Message = "Role found successfully!",
                                    Data = roles[0]
                                };
                            }
                            else
                            {
                                return new APIResult()
                                {
                                    Success = false,
                                    Message = "Role not found!",
                                    Data = null
                                };
                            }
                        }
                        else
                        {
                            return new APIResult()
                            {
                                Success = false,
                                Message = "User not found!",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Role not found!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetRolePermissionGroupMappings(int RoleId)
        {
            try
            {
                APIResult result = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().GetRolePermissionGroupMappings();

                List<Dictionary<string, object>> rolePermissionGroupMappings = ((List<Dictionary<string, object>>)result.Data).Where(x => (int)x["Role"] == RoleId).ToList();

                if ((rolePermissionGroupMappings?.Count ?? 0) > 0)
                {
                    return new APIResult()
                    {
                        Success = true,
                        Message = "Role Permission Group mappings found successfully!",
                        Data = rolePermissionGroupMappings
                    };
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Role Permission Group mappings not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetPermissionGroupMappings(List<int> permissionGroupIds)
        {
            try
            {
                APIResult result = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();

                List<Dictionary<string, object>> rolePermissionsGroups = ((List<Dictionary<string, object>>)result.Data).Where(x => permissionGroupIds.Contains((int)x["PermissionGroup"])).ToList();

                if ((rolePermissionsGroups?.Count ?? 0) > 0)
                {
                    return new APIResult()
                    {
                        Success = true,
                        Message = "Permission Groups found successfully!",
                        Data = rolePermissionsGroups
                    };
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Permission Groups not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetPermissionGroupsByIds(List<int> Ids)
        {
            try
            {
                APIResult result = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();

                List<Dictionary<string, object>> rolePermissionsGroups = ((List<Dictionary<string, object>>)result.Data).Where(x => Ids.Contains((int)x["Id"])).ToList();

                if ((rolePermissionsGroups?.Count ?? 0) > 0)
                {
                    return new APIResult()
                    {
                        Success = true,
                        Message = "Permission Groups found successfully!",
                        Data = rolePermissionsGroups
                    };
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Permission Groups not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetPermissionsByIds(List<int> Ids)
        {
            try
            {
                APIResult result = await ServiceManager.Instance.GetService<PermissionsService>().GetPermissions();

                List<Dictionary<string, object>> permissions = ((List<Dictionary<string, object>>)result.Data).Where(x => Ids.Contains((int)x["Id"])).ToList();

                if ((permissions?.Count ?? 0) > 0)
                {
                    return new APIResult()
                    {
                        Success = true,
                        Message = "Permissions found successfully!",
                        Data = permissions
                    };
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Permissions not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetUserPermissions(int Id)
        {
            try
            {
                List<Dictionary<string, object>> userRoles = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .WhereEquals("User", Id)
                .RunSelectQuery();

                if ((userRoles?.Count ?? 0) > 0)
                {
                    if ((userRoles[0]["Role"]?.ToString().Length ?? 0) > 0)
                    {
                        APIResult permissionGroupsResult = await ServiceManager.Instance.GetService<RolePermissionGroupMappingsService>().GetRolePermissionGroupMappings();

                        List<Dictionary<string, object>> permissionGroups = ((List<Dictionary<string, object>>)permissionGroupsResult.Data).Where(x => (int)x["Role"] == (int)userRoles[0]["Role"]).ToList();

                        List<int> permissionGroupIds = permissionGroups.Select(x => Int32.Parse(x["PermissionGroup"].ToString())).ToList();

                        APIResult rolePermissionsResult = await ServiceManager.Instance.GetService<PermissionGroupMappingsService>().GetPermissionGroupMappings();

                        List<Dictionary<string, object>> rolePermissions = ((List<Dictionary<string, object>>)rolePermissionsResult.Data).Where(x => permissionGroupIds.Contains((int)x["PermissionGroup"])).ToList();

                        if ((rolePermissions?.Count ?? 0) > 0)
                        {
                            int[] permissionIds = new int[rolePermissions.Count];
                            for (int i = 0; i < rolePermissions.Count; i++)
                            {
                                permissionIds[i] = Int32.Parse(rolePermissions[i]["Permission"].ToString());
                            }


                            APIResult permissionsResult = await ServiceManager.Instance.GetService<PermissionsService>().GetPermissions();

                            List<Dictionary<string, object>> permissions = ((List<Dictionary<string, object>>)permissionsResult.Data).Where(x => permissionIds.Contains((int)x["Id"])).ToList();

                            if ((permissions?.Count ?? 0) > 0)
                            {
                                return new APIResult()
                                {
                                    Success = true,
                                    Message = "Permissions found successfully!",
                                    Data = permissions
                                };
                            }
                            else
                            {
                                return new APIResult()
                                {
                                    Success = false,
                                    Message = "Permissions not found!",
                                    Data = null
                                };
                            }
                        }
                        else
                        {
                            return new APIResult()
                            {
                                Success = false,
                                Message = "Permissions not found!",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Role not found!",
                            Data = null
                        };
                    }
                }
                else
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "User not found!",
                        Data = null
                    };
                }
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> GetUser(int Id)
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id, true)
                .RunSelectQuery();

            if ((result?.Count ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = true,
                    Message = "User loaded successfully!",
                    Data = result[0]
                };
            }
            else
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "User not found!",
                    Data = null
                };
            }
        }

        public async Task<APIResult> GetUsers()
        {
            List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .RunSelectQuery();

            return new APIResult()
            {
                Success = true,
                Message = "Users loaded successfully!",
                Data = result
            };
        }

        public async Task<APIResult> AddUser(SignupRequest user)
        {
            if (user == null || string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                return APIResult.GetSimpleFailureResult("User must contain Username and Password!");
            }

            if (user.Username == null || user.Username.Length < 6)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Username should be at least 6 characters!",
                    Data = null
                };
            }

            if (user.Email == null || user.Email.Length < 6 || !Validations.IsValidEmail(user.Email))
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Email address is not valid!",
                    Data = null
                };
            }

            List<Dictionary<string, object>> usernameCheckResult = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .WhereEquals("Username", user.Username)
                .RunSelectQuery();

            if ((usernameCheckResult?.Count ?? 0) > 0 && (usernameCheckResult[0]?["Username"]?.ToString().Length ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "User already exists!",
                    Data = null
                };
            }

            List<Dictionary<string, object>> emailCheckResult = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .WhereEquals("Email", user.Email)
                .RunSelectQuery();

            if ((emailCheckResult?.Count ?? 0) > 0 && (emailCheckResult[0]?["Email"]?.ToString().Length ?? 0) > 0)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "User already exists!",
                    Data = null
                };
            }

            int role = 0;
            if ((user.Role ?? 0) > 0)
            {
                role = user.Role.Value;
            }
            else
            {
                APIResult userDefaultRoleUser = await ServiceManager
                    .Instance
                    .GetService<SettingsService>()
                    .GetSetting(Constants.Settings.DefaultRoleOnSignup);

                Dictionary<string, object> userDefaultRole = (Dictionary<string, object>)(userDefaultRoleUser.Data);
                role = Int32.Parse(userDefaultRole["Value"].ToString());
            }

            // Create User
            Dictionary<string, object> userData = new Dictionary<string, object>
                {
                    { "Username", user.Username },
                    { "Password", Hash.Create(user.Password) },
                    { "Email", user.Email ?? "" },
                    { "Fullname", user.Fullname ?? "" },
                    { "CreatedOn", DateTime.UtcNow }
                };

            List<int> ids = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .AddRow(userData)
                .RunInsertQuery();

            if ((ids?.Count ?? 0) < 1)
            {
                return APIResult.GetSimpleFailureResult("An error occured while signing up!");
            }

            Dictionary<string, object> userRoleData = new Dictionary<string, object>{
                    { "User", ids[0] },
                    { "Role", role }
                };

            Columns.AppendCreatedInfo(userRoleData, ids[0]);

            await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .AddRow(userRoleData)
                .RunInsertQuery();

            return new APIResult
            {
                Data = null,
                Success = true,
                Message = "User added successfully"
            };
        }

        public async Task<APIResult> UpdateUser(int Id, Dictionary<string, object> data)
        {
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id)
                .AddRow(data)
                .RunUpdateQuery();

            return new APIResult()
            {
                Success = true,
                Message = "User updated successfully!",
                Data = result
            };
        }

        public async Task<APIResult> DeleteUser(int Id)
        {
            // Delete User Role
            bool result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.UserRoles.ToString())
                .WhereEquals("User", Id)
                .RunDeleteQuery();

            if (!result)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while deleting user!",
                    Data = result
                };
            }

            // Delete User
            result = await QueryDesigner
                .CreateDesigner(schema: Schemas.System, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id)
                .RunDeleteQuery();
            if (!result)
            {
                return new APIResult()
                {
                    Success = false,
                    Message = "Error occured while deleting user!",
                    Data = result
                };
            }

            return new APIResult()
            {
                Success = true,
                Message = "User deleted successfully!",
                Data = result
            };
        }
    }
}