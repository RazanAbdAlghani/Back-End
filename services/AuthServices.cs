using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using secondVersionFlowSync.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace secondVersionFlowSync.services
{
    public class AuthServices
    {
        private readonly IConfiguration configuration;

        public AuthServices(IConfiguration configration)
        {
            this.configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager)
        {
            var authClaims = new List<Claim>()
            {
                new Claim (ClaimTypes.GivenName , user.UserName),
                 new Claim (ClaimTypes.Email , user.Email),
            };

            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("jwt")["secretKey"]));

            var token = new JwtSecurityToken(
                audience: "FlowSync users",
                issuer: "FlowSyncSystem",
                claims: authClaims,
                signingCredentials: new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256Signature),
                expires: DateTime.UtcNow.AddDays(1)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
