using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Apiraiser.Models;

namespace Apiraiser.Helpers
{
    public class Permissions
    {
        public static List<string> GetUserPermissions(ClaimsPrincipal User)
        {
            if (User.HasClaim(c => c.Type == ClaimTypes.Authentication))
            {
                Claim claim = User.Claims.Where(c => c.Type == ClaimTypes.Authentication).Take(1).ToList()[0];
                return claim.Value.Split(",").ToList();
            }
            return new List<string>();
        }
    }

}