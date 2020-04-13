using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Glue.Delivery.Core.Configuration;
using Glue.Delivery.Models.ApiModels.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Glue.Delivery.WebApi.Controllers
{
    [ApiController]
    [Route( "api/v{version:apiVersion}/{controller}" )]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationConfiguration _configuration;

        private List<ApplicationUser> _users = new List<ApplicationUser>
        { 
            new ApplicationUser { Id = 1, FirstName = "Test", LastName = "System", Username = "system", Password = "password1", Claims = new List<string>{ AuthorizationConstants.Claims.SystemClaim }}, 
            new ApplicationUser { Id = 2, FirstName = "Test", LastName = "User", Username = "user", Password = "password1", Claims = new List<string>{ AuthorizationConstants.Claims.UserClaim } }, 
            new ApplicationUser { Id = 2, FirstName = "Test", LastName = "Partner", Username = "partner", Password = "password1", Claims = new List<string>{ AuthorizationConstants.Claims.PartnerClaim } } 
        };
        
        public AuthenticationController(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Authenticate(AuthenticationRequest request)
        {
            var user = _users.SingleOrDefault(x => x.Username == request.Username && x.Password == request.Password);

            if (user == null)
                return NotFound();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new [] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            foreach(var claim in user.Claims)
                tokenDescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, claim));
            
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return Ok(new AuthenticationResponse
            {
                Id = user.Id,
                Token = user.Token,
                Username = user.Username
            });
        }
        
    }
}