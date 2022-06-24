using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Apiraiser.Services;
using Apiraiser.Constants;
using Apiraiser.Models;
using Apiraiser.Mappings;
using Apiraiser.Enums;

namespace Apiraiser.Helpers
{
    public static class JWT
    {
        public static UserDefinition GetUserDefinition()
        {
            string userDefinitionPath = FileSystem.GetPathInConfigurations($"User/Definition.json");
            string userDefinitionJson = FileSystem.ReadFile(userDefinitionPath);
            UserDefinition result = FileSystem.ReadJsonString<UserDefinition>(userDefinitionJson);
            return result;
        }

        public static string GenerateJSONWebToken(IConfiguration Configuration, Dictionary<string, object> data)
        {
            List<Claim> claims = new List<Claim>();

            JWTDefinition jwt = JWT.GetUserDefinition().JWT;

            // Load claims
            jwt.JWTClaims.ForEach(claim =>
            {
                Type dataType = data[claim.Table].GetType();
                if (claim.IncludeAs == Enums.JWTClaimIncludeTypes.SingleValue && dataType == typeof(Dictionary<string, object>))
                {
                    Dictionary<string, object> row = (Dictionary<string, object>)(data[claim.Table]);
                    claims.Add(
                        new Claim(
                            (string)(typeof(ClaimTypes).GetField(claim.ClaimType.ToString()).GetValue(null)),
                            row[claim.Column]?.ToString() ?? ""
                        )
                    );
                }
                else if (claim.IncludeAs == Enums.JWTClaimIncludeTypes.MultipleValues && dataType == typeof(List<Dictionary<string, object>>))
                {
                    List<Dictionary<string, object>> rows = (List<Dictionary<string, object>>)(data[claim.Table]);
                    claims.Add(
                        new Claim(
                            (string)(typeof(ClaimTypes).GetField(claim.ClaimType.ToString()).GetValue(null)),
                            string.Join(",", rows.Select(x => x[claim.Column].ToString()).ToArray())
                        )
                    );
                }
                else
                {
                    ServiceManager.Instance.GetService<LogService>().Print("Invalid JWT data configuration!", Enums.LoggingLevel.Errors);
                }
            });

            // Load token expiration time
            DateTime tokenExpiryDate = DateTime.UtcNow;
            if (jwt.JWTExpirationTime != null)
            {
                switch (jwt.JWTExpirationTime.TimeType)
                {
                    case JWTExpirationTimeTypes.Seconds:
                        tokenExpiryDate = tokenExpiryDate.AddSeconds(jwt.JWTExpirationTime.Value);
                        break;
                    case JWTExpirationTimeTypes.Minutes:
                        tokenExpiryDate = tokenExpiryDate.AddMinutes(jwt.JWTExpirationTime.Value);
                        break;
                    default:
                    case JWTExpirationTimeTypes.Hours:
                        tokenExpiryDate = tokenExpiryDate.AddHours(jwt.JWTExpirationTime.Value);
                        break;
                    case JWTExpirationTimeTypes.Days:
                        tokenExpiryDate = tokenExpiryDate.AddDays(jwt.JWTExpirationTime.Value);
                        break;
                    case JWTExpirationTimeTypes.Months:
                        tokenExpiryDate = tokenExpiryDate.AddMonths(jwt.JWTExpirationTime.Value);
                        break;
                    case JWTExpirationTimeTypes.Years:
                        tokenExpiryDate = tokenExpiryDate.AddYears(jwt.JWTExpirationTime.Value);
                        break;
                }
            }

            string issuer = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.JwtTokenIssuer, Config.DefaultJwtTokenIssuer);
            string key = (string)ServiceManager.Instance.GetService<EnvironmentService>().GetEnvironmentVariable(Config.JwtTokenSecretKey, Config.DefaultJwtTokenSecretKey);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(issuer,
                issuer,
                claims,
                expires: tokenExpiryDate,
                signingCredentials: credentials);

            // Debug.Print("UTC Time: " + DateTime.UtcNow.ToString());
            // Debug.Print("UTC Time + 120m: " + DateTime.UtcNow.AddMinutes(120).ToString());
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}