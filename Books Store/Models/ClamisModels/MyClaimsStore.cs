using System.Security.Claims;

namespace Books_Store.Models.ClamisModels
{
    public class MyClaimsStore
    {
        //ClaimType comparison is case in-sensitive where as ClaimValue comparison is case sensitive.
        public static List<Claim> AllClaims = new List<Claim>()
    {
        new Claim("CT1-Create Role", "CV1-Create Role"),
        new Claim("CT2-Edit Role","CV2-Edit Role"),
        new Claim("CT3-Delete Role","CV3-Delete Role"),
        new Claim("Delete Role","True"),
        new Claim("Edit Role","True")
    };
    }
}
