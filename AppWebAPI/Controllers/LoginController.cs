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
                    
                    var a = User.Identity.Name;
                    
                    var e = "";
                    var d = 0;
                    
                    
                    if (a != p.UserName)
                    { a = p.UserName;
                        
                    }
                    Context c = new Context();
                    var b = c.Users.Where(x => x.UserName == a).Select(y => y.Id).FirstOrDefault();
                    if (b != 0)
                    {
                        d = c.UserRoles.Where(x => x.UserId == b).Select(y => y.RoleId).FirstOrDefault();
                    }

                    if (d != 0)
                    {
                        e = c.Roles.Where(x => x.Id == d).Select(y => y.Name).FirstOrDefault();
                    }
                    
                    //if (a.IsNullOrEmpty() && e.IsNullOrEmpty()) { e = "def"; a = "def"; }
                    string token = CreateToken(e, a);
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


            return BadRequest("Hayır");

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
        private string CreateToken(string s, string u)
        {


            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                //"jdfksdfsdfjdfksdfsdfjdfksdfsdf"));
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, u),
                new Claim(ClaimTypes.Role, s)
            };
            var token = new JwtSecurityToken(
                claims: claims,
                //signingCredentials:credentials,
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
            var a = User.Identity.Name;
            var b = c.Users.Where(x => x.UserName == a).Select(y => y.Id).FirstOrDefault();
            var d = 0;
            if (b != 0)
            {
                d = c.UserRoles.Where(x => x.UserId == b).Select(y => y.RoleId).FirstOrDefault();
            }
            var e = "";
            if (d != 0)
            {
                e = c.Roles.Where(x => x.Id == d).Select(y => y.Name).FirstOrDefault();
            }
            //if (e == "") { e = "bos," + d.ToString(); } 
            //if (a.IsNullOrEmpty() && e.IsNullOrEmpty()) { e = "def"; a = "def"; }
            var refreshToken = Request.Cookies["refreshToken"];
            string token = CreateToken(e, a);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
            //return Ok(value);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogOut()
        {
            var sout= _signInManager.SignOutAsync();
            if (sout.IsCompleted)
            {
                return Ok(sout);
            }
            else return BadRequest("Çıkış yapılamadı.");
        }


        /*
        private readonly SignInManager<AppUser> _signInManager;
        public LoginController(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index(SignInModel p)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(p.UserName, p.Password, false, true);
                if (result.Succeeded)
                {
                    var a = p.UserName;
                    // You can try add AppUserRoleTable with UserRole and you can true control for login...
                    if (a == "bekirsahin")
                    {
                        
                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    
                }
            }
            return BadRequest("sald");
        }
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }*/
    }
}
