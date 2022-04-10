using JWTAuthDemoWebApi.Models;
using JWTAuthDemoWebApi.Repository;
using JWTAuthDemoWebApi.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWTAuthDemoWebApi.Controllers
{
    [Authorize(Policy = "FullAccess")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IOptions<JWTConfiguration> _JWTConfiguration;

        public UsersController(IUserRepository jWTManager, IOptions<JWTConfiguration> jwtConfiguration)
        {
            _userRepository = jWTManager;
            _JWTConfiguration = jwtConfiguration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var users = _userRepository.GetAllUsers();

            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("authenticate")]
        public IActionResult Authenticate(AuthenticationRequestModel usersdata)
        {
            var user = _userRepository.Authenticate(usersdata);
            if (user == null)
                return Unauthorized();

            var claims = new List<Claim>();
            claims.Add(new Claim("username", user.UserName));
            claims.Add(new Claim("displayname", user.Name));

            // Add roles as multiple claims
            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }
            // Optionally add other app specific claims as needed
            //claims.Add(new Claim("UserState", UserState.ToString()));

            JwtSecurityToken token = JwtHelper.GetJwtToken(
                    user.UserName,
                    _JWTConfiguration.Value.SigningKey,
                    _JWTConfiguration.Value.Issuer,
                    _JWTConfiguration.Value.Audience,
                    TimeSpan.FromMinutes(_JWTConfiguration.Value.TokenLifeTimeInMinutes),
                    claims.ToArray());

            return Ok(new TokenModel
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                TokenType = "Bearer",
                IssuedAt = token.IssuedAt,
                ExpiresAt = token.ValidTo
            });
        }
    }
}
