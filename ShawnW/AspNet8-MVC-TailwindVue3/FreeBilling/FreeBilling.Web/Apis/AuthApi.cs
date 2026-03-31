using FreeBilling.Web.Data.Entities;
using FreeBilling.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FreeBilling.Web.Apis;

public static class AuthApi
{
    public static void Register(WebApplication app)
    {
        app.MapPost("/api/auth/token", PostToken);
    }

    public static async Task<IResult> PostToken(TokenModel model,
      UserManager<TimeBillUser> userMgr,
      SignInManager<TimeBillUser> signinMgr,
      IConfiguration config)
    {
        var user = await userMgr.FindByEmailAsync(model.UserName);
        if (user is not null)
        {
            var signInResult = await signinMgr.CheckPasswordSignInAsync(user, model.Password, false);
            if (signInResult.Succeeded)
            {
                var tokenKey = config["Token:Key"];
                var issuer = config["Token:Issuer"];
                var audience = config["Token:Audience"];

                if (tokenKey is not null && issuer is not null && audience is not null)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!)
                    };

                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
                    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                      issuer,
                      audience,
                      claims: claims,
                      expires: DateTime.UtcNow.AddMinutes(20),
                      signingCredentials: creds);

                    return Results.Created("/api/auth", new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }

            }
        }

        return Results.BadRequest("Bad username or password");
    }
}
