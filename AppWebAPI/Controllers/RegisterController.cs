using AppWebAPI.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using EntityLayer.Concrete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class RegisterController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        public RegisterController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        
        [HttpPost("user")]
        public async Task<ActionResult> AddU(RegisterModel p)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser()
                {
                    Email = p.Email,
                    UserName = p.UserName,
                    FullName = p.FullName,
                    PhoneNumber = p.PhoneNumber,
                    ImgUrl = p.ImgUrl,
                    MovieUrl = p.MovieUrl,
                };
                var result = await _userManager.CreateAsync(user, p.Password);
                
                if (result.Succeeded)
                {
                    var aa = user.UserName;
                    return Ok(aa);

                }
                else
                {
                    var aa = user.Email;
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                    return Ok(aa);
                }
            }

            return Ok(p);
            
        }
    }
}
