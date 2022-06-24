using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Apiraiser.Models;

namespace Apiraiser.Helpers
{
    public class Users
    {
        public static int GetUserId(ClaimsPrincipal User)
        {
            if (User.HasClaim(c => c.Type == ClaimTypes.SerialNumber)) //ClaimsPrincipal.Current.Identities.First().Claims.ToList();
            {
                Claim claim = User.Claims.First(c => c.Type == ClaimTypes.SerialNumber);
                return Int32.Parse("" + claim.Value);
            }
            return 0;
        }
    }
}