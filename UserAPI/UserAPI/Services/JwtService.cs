using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserAPI.Data.Entities;

namespace UserAPI.Services
{
    public interface IJwtService
    {
        string Generate(AppUser user, IList<string> roles, IConfiguration _configuration);
    }
    public class JwtService : IJwtService
    {
        public string Generate(AppUser user, IList<string> roles, IConfiguration _configuration)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim("FullName",user.FullName)
            };

            claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList());

            string ketStr = _configuration.GetSection("JWT:secret").Value;

            SymmetricSecurityKey key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(ketStr));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: creds,
                    expires: DateTime.Now.AddDays(3),
                    issuer: _configuration.GetSection("JWT:issuer").Value,
                    audience: _configuration.GetSection("JWT:audience").Value
                );

            string tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenStr;
        }
    }
}
