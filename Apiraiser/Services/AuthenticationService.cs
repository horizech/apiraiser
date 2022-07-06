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
    public class AuthenticationService : BaseService
    {

        public AuthenticationService(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<APIResult> Signup(SignupRequest user)
        {
            try
            {
                if (user.Username == null || user.Username.Length < 6)
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Username should be at least 6 characters!",
                        Data = null
                    };
                }

                if (user.Username == null || user.Username.Length < 6 || !Validations.IsValidEmail(user.Email))
                {
                    return new APIResult()
                    {
                        Success = false,
                        Message = "Email address is not valid!",
                        Data = null
                    };
                }

                List<Dictionary<string, object>> usernameCheckResult = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Administration, table: TableNames.Users.ToString())
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
                    .CreateDesigner(schema: Schemas.Administration, table: TableNames.Users.ToString())
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

                APIResult allSettings = await ServiceManager
                    .Instance
                    .GetService<TableService>().GetRows(Schemas.Administration, TableNames.Settings.ToString());

                Dictionary<string, object> userDefaultRole = ((List<Dictionary<string, object>>)(allSettings.Data)).First(x => (x["Key"] as string) == Constants.Settings.DefaultRoleOnSignup);

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
                    .CreateDesigner(schema: Schemas.Administration, table: TableNames.Users.ToString())
                    .AddRow(userData)
                    .RunInsertQuery();

                if ((ids?.Count ?? 0) < 1)
                {
                    return APIResult.GetSimpleFailureResult("An error occured while signing up!");
                }

                Dictionary<string, object> userRoleData = new Dictionary<string, object>{
                    { "User", ids[0] },
                    { "Role", Int32.Parse(userDefaultRole["Value"].ToString()) }
                };

                Columns.AppendCreatedInfo(userRoleData, ids[0]);

                await QueryDesigner
                    .CreateDesigner(schema: Schemas.Administration, table: TableNames.UserRoles.ToString())
                    .AddRow(userRoleData)
                    .RunInsertQuery();

                return await Login(user.Username, null, user.Password);

            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

        public async Task<APIResult> Login(string username, string email, string password)
        {
            try
            {
                List<Dictionary<string, object>> result = await QueryDesigner
                    .CreateDesigner(schema: Schemas.Administration, table: TableNames.Users.ToString())
                    .WhereEquals(
                        (username?.Length ?? 0) > 0 ? "Username" : "Email",
                        (username?.Length ?? 0) > 0 ? username : email
                    )
                    .RunSelectQuery();

                if ((result?.Count ?? 0) > 0)
                {
                    if ((result[0]["Password"]?.ToString().Length ?? 0) > 0)
                    {
                        if (Hash.Validate(password, result[0]["Password"].ToString()) == true)
                        {
                            return await GetUserInfo(result[0]);
                        }
                        else
                        {
                            return new APIResult()
                            {
                                Success = false,
                                Message = "Password is invalid!",
                                Data = null
                            };
                        }
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Password not set up!",
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

        public async Task<APIResult> AuthLogin(int Id)
        {
            try
            {
                List<Dictionary<string, object>> result = await QueryDesigner
                .CreateDesigner(schema: Schemas.Administration, table: TableNames.Users.ToString())
                .WhereEquals("Id", Id)
                .RunSelectQuery();

                if ((result?.Count ?? 0) > 0)
                {
                    if ((result[0]["Password"]?.ToString().Length ?? 0) > 0)
                    {
                        return await GetUserInfo(result[0]);
                    }
                    else
                    {
                        return new APIResult()
                        {
                            Success = false,
                            Message = "Password not set up!",
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

        public async Task<APIResult> GetUserInfo(Dictionary<string, object> result)
        {
            try
            {
                result.Remove("Password");
                result.Remove("password");

                Dictionary<string, object> jwtTokenData = new Dictionary<string, object>();
                jwtTokenData.Add("Users", result);

                int Id = Int32.Parse(result["Id"]?.ToString() ?? null);

                APIResult roles = await ServiceManager.Instance.GetService<UsersService>().GetUserRoles(Id);
                List<int> roleIds = new List<int>();
                if (roles.Success == true && roles.Data is not null)
                {
                    roleIds = ((List<Dictionary<string, object>>)(roles.Data)).Select(x => (int)x["Id"]).ToList();
                    result.Add("Roles", roles.Data);
                    jwtTokenData.Add("Roles", ((List<Dictionary<string, object>>)(roles.Data)));
                }

                // APIResult rolePermissionGroupMappings = await ServiceManager.Instance.GetService<UsersService>().GetRolePermissionGroupMappings(roleId);
                // if (rolePermissionGroupMappings.Success == true && rolePermissionGroupMappings.Data is not null)
                // {
                //     result.Add("RolePermissionGroupMappings", (List<Dictionary<string, object>>)(rolePermissionGroupMappings.Data));
                //     jwtTokenData.Add("RolePermissionGroupMappings", (List<Dictionary<string, object>>)(rolePermissionGroupMappings.Data));

                //     List<int> permissionGroupIds = ((List<Dictionary<string, object>>)(rolePermissionGroupMappings.Data)).Select(x => Int32.Parse(x["PermissionGroup"].ToString())).Distinct().ToList();

                //     APIResult permissionGroupMappings = await ServiceManager.Instance.GetService<UsersService>().GetPermissionGroupMappings(permissionGroupIds);

                //     if (permissionGroupMappings.Success == true && permissionGroupMappings.Data is not null)
                //     {
                //         result.Add("PermissionGroupMappings", (List<Dictionary<string, object>>)(permissionGroupMappings.Data));
                //     }

                //     APIResult permissionGroups = await ServiceManager.Instance.GetService<PermissionGroupsService>().GetPermissionGroups();

                //     if (permissionGroups.Success == true && permissionGroups.Data is not null)
                //     {
                //         result.Add("PermissionGroups", ((List<Dictionary<string, object>>)(permissionGroups.Data)).Where(x => permissionGroupIds.Contains((int)x["Id"])).ToList());
                //     }

                //     List<int> permissionIds = ((List<Dictionary<string, object>>)(permissionGroupMappings.Data))
                //         .Select(x => Int32.Parse(x["Permission"].ToString()))
                //         .ToList();

                //     APIResult permissions = await ServiceManager.Instance.GetService<UsersService>().GetPermissionsByIds(permissionIds);
                //     if (permissions.Success == true && permissions.Data is not null)
                //     {
                //         result.Add("Permissions", (List<Dictionary<string, object>>)(permissions.Data));
                //         jwtTokenData.Add("Permissions", (List<Dictionary<string, object>>)(permissions.Data));
                //     }
                // }

                result.Add("Token", JWT.GenerateJSONWebToken(_configuration, jwtTokenData));

                return new APIResult()
                {
                    Success = true,
                    Message = "User logged in Successfully!",
                    Data = result
                };
            }
            catch (Exception e)
            {
                return APIResult.GetExceptionResult(e);
            }
        }

    }
}