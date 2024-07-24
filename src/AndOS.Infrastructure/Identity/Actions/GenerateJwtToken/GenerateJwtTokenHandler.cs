using MediatR;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AndOS.Infrastructure.Identity.Actions.GenerateJwtToken;

public class GenerateJwtTokenHandler(IConfiguration configuration) : IRequestHandler<GenerateJwtTokenRequest, string>
{
    public Task<string> Handle(GenerateJwtTokenRequest request, CancellationToken cancellationToken)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Email, request.User.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Name, request.User.UserName),
            new Claim(ClaimTypes.NameIdentifier, request.User.Id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}
