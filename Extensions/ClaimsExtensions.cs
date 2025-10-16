using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace api.Extensions
{
    public static class ClaimsExtensions
    {
        #region Reach to the CLAIMS 

        //- this is an extension(that it's why both are statics) it means that add new funcionalities without changing the main funcionality of ClaimsPrincipal
        public static string GetUserName(this ClaimsPrincipal user)//ClaimsPrincipal is the representation of the user authenticated we use it to get the UserName when it's logged with the bearer
        {
            return user.Claims.SingleOrDefault(c => c.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")).Value;//Implementation by default to reach to the CLAIMS and the atributes inside
        }        
        #endregion
    }
}