using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UtilityLayer.Interfaces;
using UtilityLayer.Model;

namespace UtilityLayer.Services
{
    internal class JWTService : IJWTService
    {
        private readonly IConfiguration configuration;
        public JWTService(IConfiguration config)
        {
            configuration = config;
        }

        public string GenerateToken(ClaimsModel claimsmodel,int ExpireTimeInMinutes)
        {
            var issuer = configuration["Jwt:Issuer"];
            var audience = configuration["Jwt:Audience"];
            
            var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);

            var claims = new[]
            {
             new Claim(JwtRegisteredClaimNames.Sub,claimsmodel.User_Name??"Guest"),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim(ClaimTypes.NameIdentifier, claimsmodel?.User_Id.ToString() ?? "1"),
             new Claim(ClaimTypes.Name, claimsmodel?.User_Name?.ToString() ?? "Admin"),
             new Claim("User_Id",claimsmodel?.User_Id.ToString() ?? "1"),
             new Claim("User_Name",claimsmodel?.User_Name?.ToString() ?? "Guest"),
             new Claim("Company_Id",claimsmodel?.Company_Id.ToString() ?? "1"),
             new Claim("Company_Name",claimsmodel?.Company_Name ?? "Inventia"),
             new Claim("Email_Id",claimsmodel?.Email_Id?.ToString() ?? "guest@gmail.com"),
             new Claim("IP_Address",claimsmodel?.Ip_Address?.ToString() ?? "192.168.1.1"),
             new Claim("Is_SuperAdmin",claimsmodel?.Is_Superadmin.ToString() ?? "false"),
             new Claim("Is_Company_Owner",claimsmodel?.Is_Company_Owner.ToString() ?? "false"),
             new Claim("Token_Expiry_Time",claimsmodel?.Token_Expiry_Time?.ToString() ?? "0"),

            };


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToInt32(ExpireTimeInMinutes)),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
