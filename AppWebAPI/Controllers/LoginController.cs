using AppWebAPI.Models;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AppWebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class LoginController : ControllerBase
    {

        private readonly IConfiguration _configuration;
        private readonly SignInManager<AppUser> _signInManager;
        public LoginController(IConfiguration configuration, SignInManager<AppUser> signInManager)
        {
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(SignInModel p)
        {

            if (ModelState.IsValid)
            {


                var result = await _signInManager.PasswordSignInAsync(p.UserName, p.Password, false, true);


                if (result.Succeeded)
                {
                    
                    var username = User.Identity.Name;
                    
                    var role = "";
                    var roleid = 0;
                    
                    
                    if (username != p.UserName)
                    { username = p.UserName;
                        
                    }
                    Context c = new Context();
                    var userid = c.Users.Where(x => x.UserName == username).Select(y => y.Id).FirstOrDefault();
                    if (userid != 0)
                    {
                        roleid = c.UserRoles.Where(x => x.UserId == userid).Select(y => y.RoleId).FirstOrDefault();
                    }

                    if (roleid != 0)
                    {
                        role = c.Roles.Where(x => x.Id == roleid).Select(y => y.Name).FirstOrDefault();
                    }
                    
                    
                    string token = CreateToken(role, username);
                    var newRefreshToken = GenerateRefreshToken();
                    SetRefreshToken(newRefreshToken);

                    return Ok(token);

                }
                else
                {
                    var aaa = p.UserName;
                    return Ok(aaa);
                }
            }


            return BadRequest("User Not Found");

        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddMinutes(15),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);


        }
        private string CreateToken(string role, string user)
        {


            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        [HttpGet("reftoken")]
        public async Task<ActionResult> Refresh()
        {
            Context c = new Context();
            var username = User.Identity.Name;
            var userid = c.Users.Where(x => x.UserName == username).Select(y => y.Id).FirstOrDefault();
            var roleid = 0;
            if (userid != 0)
            {
                roleid = c.UserRoles.Where(x => x.UserId == userid).Select(y => y.RoleId).FirstOrDefault();
            }
            var role = "";
            if (roleid != 0)
            {
                role = c.Roles.Where(x => x.Id == roleid).Select(y => y.Name).FirstOrDefault();
            }
            var refreshToken = Request.Cookies["refreshToken"];
            string token = CreateToken(role, username);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            var sout= _signInManager.SignOutAsync();
            if (sout.IsCompleted)
            {
                return Ok(sout);
            }
            else return BadRequest("You didnt signout.");
        }

    }
}
