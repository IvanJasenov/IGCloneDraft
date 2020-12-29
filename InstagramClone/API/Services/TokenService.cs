using API.Entities;
using API.Interfaces;
using AutoMapper.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        // Symmetric Encription is where same key is used for encrypt and decript information
        private readonly SymmetricSecurityKey _key;
        // import the Configuration service
        private readonly Microsoft.Extensions.Configuration.IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(Microsoft.Extensions.Configuration.IConfiguration config, UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Token:Key"]));
        }

        public async Task<string> CreateToken(AppUser user)
        {
            // adding claims
            var claims = new List<Claim>
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, user.UserName)    
            };
            // get the roles
            var roles = await _userManager.GetRolesAsync(user);
            // add roles to the claims
            // we dont use JwtRegisteredClaimNames because they dont have option for Role 
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // creating credentials
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
         
            // lets now define what goes into the token
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
