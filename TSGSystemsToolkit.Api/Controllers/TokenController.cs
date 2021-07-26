﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TsgSystems.Api.Data;
using TsgSystemsToolkit.DataManager.DataAccess;
using TsgSystemsToolkit.DataManager.Models;

namespace TsgSystems.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserData _userData;
        private readonly ILogger<TokenController> _logger;

        public TokenController(ApplicationDbContext context, UserManager<IdentityUser> userManager, 
            IUserData userData, ILogger<TokenController> logger)
        {
            _context = context;
            _userManager = userManager;
            _userData = userData;
            _logger = logger;
        }

        [Route("/api/token")]
        [Consumes("application/x-www-form-urlencoded")]
        [HttpPost]
        public async Task<IActionResult> Create([FromForm]string username, [FromForm] string password, [FromForm] string grant_type)
        {
            if (await IsValidCredentials(username, password))
            {
                return new ObjectResult(await GenerateToken(username));
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<bool> IsValidCredentials(string username, string password)
        {
            var user = await _userManager.FindByEmailAsync(username);

            bool output = await _userManager.CheckPasswordAsync(user, password);

            if (output == true)
            {
                var userExists = await _userData.GetUserById(user.Id);

                if (userExists.Count == 0)
                {
                    await AddUserToApplicationDb(user);
                }
            }

            return output;
        }

        private async Task AddUserToApplicationDb(IdentityUser user)
        {
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };

            UserModel newUser = new()
            {
                EmailAddress = user.Email,
                Id = user.Id,
                CreatedDate = DateTime.UtcNow
            };

            foreach (var role in roles)
            {
                newUser.Roles += $"{role.Name},";
            }

            newUser.Roles = newUser.Roles.Trim(',');

            await _userData.AddUser(newUser);
        }

        private async Task<dynamic> GenerateToken(string username)
        {
            var user = await _userManager.FindByEmailAsync(username);
            var roles = from ur in _context.UserRoles
                        join r in _context.Roles on ur.RoleId equals r.Id
                        where ur.UserId == user.Id
                        select new { ur.UserId, ur.RoleId, r.Name };

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(7)).ToUnixTimeSeconds().ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("StkfDL6E8uxnz6VQebwcs4vGqYPORe08mVbaLIPyLQKcoFDQkd3zg61vCxBAMZI"));

            var token = new JwtSecurityToken(
                new JwtHeader(
                    new SigningCredentials(securityKey,
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            var output = new
            {
                Access_Token = new JwtSecurityTokenHandler().WriteToken(token),
                UserName = username
            };

            return output;
        }
    }
}