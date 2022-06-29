using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fasetto.Word.Web.Server.IoC;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Fasetto.Word.Web.Server.Controllers
{
    /// <summary>
	/// manages web api calls
	/// </summary>
    public class ApiController : Controller
    {
        // GET: /<controller>/
        [Route("api/login")]
        public IActionResult LogIn()//[FromBody] LoginModel model)
        {
            //Todo: get users login info and confirm its correct
            var username = "angelsix";
            var email = "contact@angelxix.com";

            //gset our tokens claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.NameId,"unknownUser"),
                new Claim(JwtRegisteredClaimNames.Email,email),
                new Claim("my key", "my value")
            };

            //create the credentials used to generate the token
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    IocContainer.Configuration["Jwt:SecretKey"])),SecurityAlgorithms.HmacSha256);

            //Generate Jwt Token
            var token = new JwtSecurityToken(
                issuer: IocContainer.Configuration["Jwt:Issuer"],
                audience: IocContainer.Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(3),
                signingCredentials: credentials
                );

            //return token to user
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

    }
}
