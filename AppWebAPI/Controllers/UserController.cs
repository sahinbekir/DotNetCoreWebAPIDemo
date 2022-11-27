using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.EntityFramework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AppWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        UserManager um = new UserManager(new EfUserRepository());

        [AllowAnonymous]
        [HttpGet("userprofile")]
        public async Task<ActionResult> Get()
        {


            var name = User.Identity.Name;
            Context c = new Context();
            var userid = c.Users.Where(x => x.UserName == name).Select(y => y.Id).FirstOrDefault();

            var values = um.GetById(userid);
            return Ok(values);


        }
        [Authorize(Roles = "Admin")]
        [HttpGet("userlist")]
        public async Task<ActionResult> GetAll()
        {


            

            var values = um.GetListAll();
            return Ok(values);


        }
        [Authorize(Roles ="Admin")]
        [HttpGet("useronly")]
        public async Task<ActionResult> GetUserOnly()
        {


            var name = User.Identity.Name;
            Context c = new Context();
            var userid = c.Users.Where(x => x.UserName == name).Select(y => y.Id).FirstOrDefault();

            var values = um.GetUserList();
            return Ok(values);


        }
    }
}
