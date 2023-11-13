﻿using Microsoft.IdentityModel.Tokens;
using shushi_shop_api.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace shushi_shop_api.Middleware
{
    public class GenerateJWT
    {
        private readonly IConfiguration _config;
        public GenerateJWT(IConfiguration config)
        {
            _config = config;
        }
        public string NewToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.Name),
                new Claim(ClaimTypes.Role,user.role)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
