using System.Security.Claims;

namespace Books_Store.Models.ClamisModels
{
    public class UserClaimsModel
    {
        public UserClaimsModel()
        {
            Cliams = new List<UserClaim>();
        }

        //holds the ID of the user for whom we are adding or removing a claim
        public string UserId { get; set; }
        public List<UserClaim> Cliams { get; set; }
    }
}
